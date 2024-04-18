using System;
using System.Collections.Generic;
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

namespace Yahv.PvOms.WebApp.Orders.Common
{
    /// <summary>
    /// 订单本位币收款核销
    /// </summary>
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
            var order = Erp.Current.WsOrder.Orders.SingleOrDefault(item => item.ID == ID);

            if (order != null)
            {
                var view = new Services.Views.EnterprisesTopView<Layers.Data.Sqls.PvbCrmReponsitory>();
                var payer = view.SingleOrDefault(item => item.ID == order.ClientID);
                var payee = view.SingleOrDefault(item => item.ID == "DBAEAB43B47EB4299DD1D62F764E6B6A");

                this.Model.OrderData = new
                {
                    PayerName = payer.Name,
                    PayeeName = payee.Name,
                };
                //人民币实收款
                this.Model.FlowAccountData = new Yahv.Payments.Views.BankFlowAccountsView()
                    .Where(item => item.Payer == payer.ID && item.Payee == payee.ID)
                    .Where(item => item.Price > 0 && item.Currency == Currency.CNY)
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
            var order = Erp.Current.WsOrder.Orders.SingleOrDefault(item => item.ID == ID);
            var view = new Services.Views.EnterprisesTopView<Layers.Data.Sqls.PvbCrmReponsitory>();
            var payer = view.SingleOrDefault(item => item.ID == order.ClientID);
            var payee = view.SingleOrDefault(item => item.ID == "DBAEAB43B47EB4299DD1D62F764E6B6A");

            //订单的本位币应收账款
            var query = Erp.Current.WsOrder.VouchersCnyStatistics
                .Where(item => item.OrderID == ID)
                .ToArray();
            query = query.Where(item => item.Subject != Payments.SubjectConsts.代付货款 && item.Subject != Payments.SubjectConsts.代收货款).ToArray();
            var linq = query.Select(t => new
            {
                t.ReceivableID,
                t.Catalog,
                t.Subject,
                Payer = t.PayerID,
                Payee = t.PayeeID,
                PayerName = payer.Name,
                PayeeName = payee.Name,
                t.Currency,
                CurrencyName = t.Currency.GetDescription(),
                LeftPrice = t.LeftPrice.ToString("f2"),
                TaxLeftPrice = (t.LeftPrice * 1.06m).ToString("f2"),
                RightPrice = t.RightPrice?.ToString("f2"),
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
            var Ids = Erp.Current.WsOrder.VouchersCnyStatistics.Where(item => item.OrderID == ID).Select(item => item.ReceivableID).ToArray();

            //应收账款核销记录
            var query = new ReceivedsStatisticsView().Where(item => Ids.Contains(item.ReceivableID)).ToArray();
            query = query.Where(item => item.Subject != Payments.SubjectConsts.代付货款 && item.Subject != Payments.SubjectConsts.代收货款).ToArray();

            var linq = query.ToArray().Select(t => new
            {
                t.ReceivableID,
                t.Catalog,
                t.Subject,
                t.PayerName,
                t.PayeeName,
                Price = t.Price1.ToString("f2"),
                Currency = t.Currency1.GetDescription(),
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
                string Price = Request.Form["Price"].Trim();
                string RightPrice = Request.Form["RightPrice"].Trim();
                string IsTax = Request.Form["IsTax"].Trim();

                decimal leftprice = IsTax == "true" ? decimal.Parse(Price) * 1.06m : decimal.Parse(Price);
                decimal rightpirce = string.IsNullOrEmpty(RightPrice) || RightPrice == "null" ? 0m : decimal.Parse(RightPrice);
                decimal price = leftprice > rightpirce ? leftprice - rightpirce : 0M;
                if (price > 0)
                {
                    Yahv.Payments.PaymentManager.Erp(Erp.Current.ID).Received.For(ReceivableID)
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
                var order = Erp.Current.WsOrder.Orders.SingleOrDefault(item => item.ID == ID);

                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }
    }
}