using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Payments.Views;
using Yahv.PvWsOrder.Services.Common;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvOms.WebApp.Applications.Finance
{
    public partial class WriteOffReceive : ErpParticlePage
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

                    application.InCompanyName,
                    application.InBankName,
                    application.InBankAccount,

                    application.OutCompanyName,
                    application.OutBankName,
                    application.OutBankAccount,
                };

                var payee = new PayeesTopView().SingleOrDefault(item => item.Account == application.InBankAccount && item.Status == GeneralStatus.Normal && item.Methord != Methord.Cash);
                this.Model.PayeeData = new
                {
                    Payee = payee.EnterpriseName,
                };

                //认领的实收
                var accountWorks = Erp.Current.WsOrder.AccountWorks.ToArray();

                var bankFlowAccounts = new BankFlowAccountsView()
                    .Where(item => item.Payer == application.ClientID && item.Payee == payee.EnterpriseID)
                    .Where(item => item.Price > 0 && item.Currency == application.Currency).ToArray();

                this.Model.FlowAccountData = from entity in accountWorks
                                             join bankFlow in bankFlowAccounts on entity.FormCode equals bankFlow.FormCode
                                             select new
                                             {
                                                 Value = bankFlow.FormCode,
                                                 Text = bankFlow.FormCode,
                                                 bankFlow.Bank,
                                                 bankFlow.Account,
                                                 bankFlow.Price,
                                                 bankFlow.Payee,
                                                 bankFlow.Payer,
                                                 bankFlow.Currency,
                                                 entity.PayeeLeftID,
                                                 CurrencyDec = bankFlow.Currency.GetDescription(),
                                             };
            }
        }

        protected object data()
        {
            string ID = Request.QueryString["ID"];
            var application = Erp.Current.WsOrder.Applications.SingleOrDefault(item => item.ID == ID);
            var payer = application.Client.Name;
            var payee = new PayeesTopView().SingleOrDefault(item => item.Account == application.InBankAccount && item.Status == GeneralStatus.Normal && item.Methord != Methord.Cash).EnterpriseName;
            //申请的应收账款
            var query = new VouchersStatisticsView().Where(item => item.ApplicationID == ID).ToArray();

            var linq = query.Select(t => new
            {
                t.ReceivableID,
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

            //应收账款Ids
            var Ids = new VouchersStatisticsView().Where(item => item.ApplicationID == ID).Select(item => item.ReceivableID).ToArray();

            //应收账款核销记录
            var query = new ReceivedsStatisticsView().Where(item => Ids.Contains(item.ReceivableID));

            var linq = query.ToArray().Select(t => new
            {
                t.ReceivableID,
                t.PayerName,
                t.PayeeName,
                Price = t.Price.ToString("0.00"),
                Currency = t.SettlementCurrency.GetDescription(),
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
                string ReceivableID = Request.Form["ReceivableID"].Trim();
                string Payer = Request.Form["Payer"].Trim();
                string Payee = Request.Form["Payee"].Trim();
                Currency Currency = (Currency)(int.Parse(Request.Form["Currency"].Trim()));
                string Account = Request.Form["Account"].Trim();
                string Bank = Request.Form["Bank"].Trim();
                string PayeeLeftID = Request.Form["PayeeLeftID"].Trim();
                string Price = Request.Form["Price"].Trim();

                if (string.IsNullOrWhiteSpace(Price) || decimal.Parse(Price) == 0)
                {
                    throw new Exception("核销金额不能为0");
                }
                //核销金额
                decimal price = decimal.Parse(Price);

                //CRM核销
                Yahv.Payments.Models.Rolls.VoucherResult result = Yahv.Payments.PaymentManager.Erp(Erp.Current.ID).Received.For(ReceivableID)
                    .Confirm(new Yahv.Payments.Models.Rolls.VoucherInput()
                    {
                        CreateDate = DateTime.Now,
                        Type = VoucherType.Receipt,
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
                if (result.Success == true)
                {
                    //资金中心核销
                    var data = new
                    {
                        sender = "FSender002",
                        option = "insert",
                        model = new
                        {
                            SeqNo = FormCode,
                            FeeType = "AccCatType0402",
                            Amount = result.WriteOffPrice,
                            CreatorID = Erp.Current.ID
                        },
                    };
                    var apisetting = new PvWsOrder.Services.PvFinanceApiSetting();
                    var url = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.PayeeRightEnter;
                    Yahv.Utils.Http.ApiHelper.Current.JPost(url, data);
                }

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

                //收款核销完成
                application.ReveiveWorkOff();

                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }
    }
}