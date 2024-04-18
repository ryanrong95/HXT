using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Payments;
using Yahv.PvWsOrder.Services.Extends;
using Yahv.PvWsOrder.Services.Models;
using Yahv.PvWsOrder.Services.Views.Alls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvOms.WebApp.Orders.Common
{
    public partial class ConfirmBillNew : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected object data()
        {
            var orderid = Request.QueryString["ID"];

            var query = Erp.Current.WsOrder.VouchersCnyStatistics
                .Where(item => item.OrderID == orderid)
                .Where(item => item.Business == ConductConsts.供应链)
                .ToArray();
            query = query.Where(item => item.LeftDate != item.RightDate).ToArray();//过滤现金收款
            query = query.Where(item => item.Subject != SubjectConsts.代付货款 && item.Subject != SubjectConsts.代收货款).ToArray();//过滤代收付货款
            var linq = query.Select(t => new
            {
                t.ReceivableID,
                t.OrderID,
                t.Catalog,
                t.Subject,
                PayeeName = t.Payee.Name,
                PayerName = t.Payer.Name,
                PayeeID = t.Payee.ID,
                PayerID = t.Payer.ID,

                OriginCurrency = t.OriginCurrency.GetCurrency().ShortName,
                OriginPrice = t.OriginPrice.ToString("f2"),
                Rate = t.Rate.ToString("f4"),
                Currency = t.Currency.GetCurrency().ShortName,
                LeftPrice = t.LeftPrice.ToString("f2"),
                RightPrice = t.RightPrice?.ToString("f2"),
                ConfirmPrice = t.LeftPrice.ToString("f2"),
                OriginDate = t.LeftDate.ToString("yyyy-MM-dd"),
            });
            return new
            {
                rows = linq.OrderBy(item => item.OriginDate).ToArray(),
                total = linq.Count()
            };
        }

        protected void Submit()
        {
            try
            {
                var orderId = Request.Form["orderId"];
                Order order = Erp.Current.WsOrder.Orders.SingleOrDefault(item => item.ID == orderId);

                var receivables = Request.Form["receivables"].Replace("&quot;", "'").Replace("amp;", "");
                var receivableList = receivables.JsonTo<List<dynamic>>();

                var PayerID = string.Empty;
                var PayeeID = string.Empty;
                if (receivableList.Count() != 0)
                {
                    PayerID = (string)receivableList.FirstOrDefault().PayerID;
                    PayeeID = (string)receivableList.FirstOrDefault().PayeeID;
                }
                else
                {
                    PayerID = order.ClientID;
                    PayeeID = Erp.Current.Leagues.Current?.EnterpriseID ?? PvWsOrder.Services.Common.Helper.XdtCompanyID;
                }

                foreach (var rec in receivableList)
                {
                    var Subject = (string)rec.Subject;
                    if (Subject != SubjectConsts.代付货款 && Subject != SubjectConsts.代收货款)
                    {
                        //财务要求：出代收付货款外，不管有没有变更应收金额都调用一下
                        var ReceivableID = (string)rec.ReceivableID;
                        var price = (decimal)rec.ConfirmPrice;
                        PaymentManager.Erp(Erp.Current.ID)[PayerID, PayeeID][ConductConsts.供应链].Receivable.For(ReceivableID).ModifyRmb(price);
                    }
                }
                
                //if (order.Type == OrderType.Delivery)
                //{
                //    //发货订单需强制结算仓储费
                //    PaymentManager.Npc[PayerID, PayeeID][ConductConsts.代仓储]
                //        .Receivable.Confirm(orderId, Currency.CNY, true);
                //}

                //修改订单支付状态
                order.OperatorID = Erp.Current.ID;
                order.PaymentStatus = OrderPaymentStatus.ToBePaid;
                order.StatusLogUpdate();

                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }
    }
}