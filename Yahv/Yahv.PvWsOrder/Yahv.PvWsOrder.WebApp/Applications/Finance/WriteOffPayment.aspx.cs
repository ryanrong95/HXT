using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Payments.Views;
using Yahv.PvWsOrder.Services.Common;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Services.Views;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvOms.WebApp.Applications.Finance
{
    public partial class WriteOffPayment : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        protected void LoadData()
        {
            string ID = Request.QueryString["ID"];
            var application = Erp.Current.WsOrder.Applications.SingleOrDefault(item => item.ID == ID);
            if (application != null)
            {
                this.Model.ApplicationData = new
                {
                    CreateDate = application.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    ClientName = application.Client.Name,
                    ClientCode = application.Client.EnterCode,
                    Currency = application.Currency.GetDescription(),
                    AvailableBalance = 0,

                    PayerName = application.Payers.FirstOrDefault()?.EnterpriseName,
                    PayerBankName = application.Payers.FirstOrDefault()?.BankName,
                    PayerBankAccount = application.Payers.FirstOrDefault()?.BankAccount,
                    PayerMethod = application.Payers.FirstOrDefault()?.Method.GetDescription(),
                    PayerCurrency = application.Payers.FirstOrDefault()?.Currency.GetDescription(),

                    PayeeName = application.Payees.FirstOrDefault()?.EnterpriseName,
                    PayeeBankName = application.Payees.FirstOrDefault()?.BankName,
                    PayeeBankAccount = application.Payees.FirstOrDefault()?.BankAccount,
                    PayeeMethod = application.Payees.FirstOrDefault()?.Method.GetDescription(),
                    PayeeCurrency = application.Payees.FirstOrDefault()?.Currency.GetDescription(),

                    InCompanyName = application.InCompanyName,
                    InBankName = application.InBankName,
                    InBankAccount = application.InBankAccount,

                    OutCompanyName = application.OutCompanyName,
                    OutBankName = application.OutBankName,
                    OutBankAccount = application.OutBankAccount,
                };

                var payer = new PayersTopView().Where(item => item.Account == application.OutBankAccount && item.Status == GeneralStatus.Normal && item.Methord != Methord.Cash)
                    .FirstOrDefault(t => t.RealEnterpriseID == t.EnterpriseID || t.RealEnterpriseID == null || t.RealEnterpriseID == string.Empty)
                    .EnterpriseID; //我方付款人
                var payee = application.Payees.FirstOrDefault().EnterpriseID;//供应商收款人

                this.Model.FlowAccountData = new Yahv.Payments.Views.BankFlowAccountsView()
                    .Where(item => item.Payer == payer && item.Payee == payee)
                    .Where(item => item.Price > 0 && item.Currency == application.Currency)
                    .ToArray().Select(item => new
                    {
                        Value = item.FormCode,
                        Text = item?.FormCode,
                        item?.Bank,
                        item?.Account,
                        item?.Price,
                        item?.Payee,
                        item?.Payer,
                        item?.Currency,
                        CurrencyDec = item?.Currency.GetDescription(),
                    });
            }
        }

        protected object data()
        {
            string ID = Request.QueryString["ID"];
            var application = Erp.Current.WsOrder.Applications.SingleOrDefault(item => item.ID == ID);
            var payer = application.OutCompanyName;
            var payee = application.Payees.FirstOrDefault()?.EnterpriseName;
            //申请的应付账款
            var query = new PaymentsStatisticsView().Where(item => item.ApplicationID == ID).ToArray();

            var linq = query.Select(t => new
            {
                t.PayableID,
                t.Payer,
                t.Payee,
                PayerName = payer,
                PayeeName = payee,
                t.Currency,
                t.CurrencyName,
                LeftPrice = t.LeftPrice.ToString("0.00"),
                RightPrice = t.RightPrice?.ToString("0.00"),
            });
            return new
            {
                rows = linq.ToArray(),
                total = query.Count()
            };
        }

        protected object data2()
        {
            string ID = Request.QueryString["ID"];
            var application = Erp.Current.WsOrder.Applications.SingleOrDefault(item => item.ID == ID);
            var payer = application.OutCompanyName;
            var payee = application.Payees.FirstOrDefault()?.EnterpriseName;

            //应付账款Ids
            var Ids = new PaymentsStatisticsView().Where(item => item.ApplicationID == ID).Select(item => item.PayableID).ToArray();
            //实付账款核销记录
            var query = Erp.Current.WsOrder.Payments.Where(item => Ids.Contains(item.PayableID));
            var currency = application.Currency;

            var linq = query.ToArray().Select(t => new
            {
                t.PayableID,
                PayerName = payer,
                PayeeName = payee,
                Price = t.Price.ToString("0.00"),
                Currency = currency.GetDescription(),
                CreateDate = t.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
            });
            return new
            {
                rows = linq.ToArray(),
                total = query.Count()
            };
        }

        //核销
        protected void WriteOff()
        {
            try
            {
                string FormCode = Request.Form["FormCode"].Trim();
                string PayableID = Request.Form["PayableID"].Trim();
                string Payer = Request.Form["Payer"].Trim();
                string Payee = Request.Form["Payee"].Trim();
                Currency Currency = (Currency)(int.Parse(Request.Form["Currency"].Trim()));
                string Account = Request.Form["Account"].Trim();
                string Bank = Request.Form["Bank"].Trim();

                string Price = Request.Form["Price"].Trim();
                if (string.IsNullOrWhiteSpace(Price) || decimal.Parse(Price) == 0)
                {
                    throw new Exception("核销金额不能为0");
                }
                //核销金额
                decimal price = decimal.Parse(Price);

                //CRM核销
                Yahv.Payments.PaymentManager.Erp(Erp.Current.ID).Payment.For(PayableID)
                    .Confirm(new Yahv.Payments.Models.Rolls.VoucherInput()
                    {
                        CreateDate = DateTime.Now,
                        Type = VoucherType.Payment,
                        Payer = Payer,
                        Payee = Payee,
                        Bank = Bank,
                        Account = Account,
                        CreatorID = Erp.Current.ID,
                        FormCode = FormCode,
                        Business = Yahv.Payments.ConductConsts.供应链,
                        AccountType = AccountType.BankStatement,
                        Currency = Currency,
                        Price = price,
                    });

                //TODO:中心核销

                Response.Write((new { success = true, message = "核销成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "核销失败：" + ex.Message }).Json());
            }
        }
        //核销完成
        protected void Submit()
        {
            try
            {
                string ID = Request.Form["ID"].Trim();
                var application = Erp.Current.WsOrder.Applications.SingleOrDefault(item => item.ID == ID);

                //付款核销完成
                application.PaymentWorkOff();

                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }
    }
}