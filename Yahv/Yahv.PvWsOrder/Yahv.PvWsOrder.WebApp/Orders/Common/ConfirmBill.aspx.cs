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
    public partial class ConfirmBill : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        
        protected object data()
        {
            var orderid = Request.QueryString["ID"];
            var query = new VouchersStatisticsRoll(orderid).AsEnumerable();
            var linq = query.Select(t => new
            {
                ID = t.ReceivableID,
                OrderID = t.OrderID,
                CouponID = t.CouponID,
                PayeeName = t.PayeeName,
                PayerName = t.PayerName,
                Payee = t.Payee,
                Payer = t.Payer,
                Catalog = t.Catalog,
                Subject = t.Subject,
                Currency = t.Currency.GetCurrency().ShortName,
                //应收金额
                LeftPrice = t.LeftPrice,
                //减免金额
                ReducePrice = t.ReduceTotalPrice,
                //优惠金额
                CouponPrice = t.CouponTotalPrice,
                //实际应收
                RealLeftPrice = t.LeftPrice - t.ReduceTotalPrice - t.CouponTotalPrice,
                //实际已收
                RealRightPrice = (t.RightPrice == null ? 0m : t.RightPrice) - t.CouponTotalPrice,
                //未收金额
                Remains = t.Remains - t.ReduceTotalPrice,
                OriginDate = t.LeftDate.ToString("yyyy-MM-dd"),
            });
            return new
            {
                rows = linq.OrderBy(item => item.OriginDate).ToArray(),
                total = linq.Count()
            };
        }

        /// <summary>
        /// 优惠券数据
        /// </summary>
        /// <returns></returns>
        protected object vouchersData()
        {
            var orderid = Request.QueryString["ID"];
            var query = new VouchersStatisticsRoll(orderid).FirstOrDefault();
            //收款公司分配给客户的优惠券
            var linq = CouponManager.Current[query?.Payee, query?.Payer].Where(t => t.Balance > 0);
            var row = linq.OrderBy(item => item.Subject).OrderBy(item => item.Catalog).Select(s => new
            {
                ID = s.ID,
                Name = s.Name,
                Type = ((CouponType)Enum.Parse(typeof(CouponType), s.Type.ToString())).GetDescription(),
                Catalog = s.Catalog,
                Subject = s.Subject,
                Price = s.Price,
                TypeValue = s.Type,
            });

            return new
            {
                rows = row.ToArray(),
                total = linq.Count(),
            };
        }

        protected void Submit()
        {
            try
            {
                var orderId = Request.Form["orderId"];
                var receivables = Request.Form["receivables"].Replace("&quot;", "'").Replace("amp;", "");
                var itemList = receivables.JsonTo<List<VoucherStatistic>>();
                Order order = Erp.Current.WsOrder.Orders.Where(item => item.ID == orderId).FirstOrDefault();

                //调用优惠券接口记录优惠券对应的应收。
                var maps = from receive in itemList
                           select new UsedMap
                           {
                               ReceivableID = receive.ID,
                               CouponID = receive.CouponID,
                               Quantity = 1,//默认一次使用一张
                           };
                var receivable = itemList.FirstOrDefault();
                CouponManager.Current[receivable.Payee, receivable.Payer].Confirm(Erp.Current.ID, maps.ToArray());

                //修改订单支付状态
                order.OperatorID = Erp.Current.ID;
                order.PaymentStatus = OrderPaymentStatus.ToBePaid;
                order.StatusLogUpdate();

                //发货订单需强制结算仓储费
                if (order.Type == OrderType.Delivery)
                {
                    PaymentManager.Npc[receivable.Payer, receivable.Payee][ConductConsts.代仓储]
                        .Receivable.Confirm(orderId, Currency.CNY, true);
                }

                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }
    }
}