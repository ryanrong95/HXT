using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Extends;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvOms.WebApp.Approval.Orders
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 待审批订单详情
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            var orderId = Request.QueryString["ID"];
            var query = new Yahv.PvWsOrder.Services.Views.OrderItemsAlls().Where(o => o.OrderID == orderId).AsEnumerable();

            var linq = query.Select(t => new
            {
                OrderItemID = t.ID,
                Quantity = t.Quantity,
                DateCode = t.DateCode,
                Origin = t.OriginGetDescription,
                Unit = t.Unit.GetDescription(),
                UnitPrice = t.UnitPrice,
                Currency = t.Currency.GetDescription(),
                TotalPrice = t.TotalPrice,
                GrossWeight = t.GrossWeight,
                Volume = t.Volume,
                PartNumber = t.Product.PartNumber,
                Manufacturer = t.Product.Manufacturer,
                Condition = t.OrderItemsTerm == null ? "" : t.Terms
            }).OrderBy(item => item.PartNumber);
            return new
            {
                rows = linq.ToArray(),
                total = query.Count()
            };
        }

        /// <summary>
        /// 审批通过=>归类错误
        /// </summary>
        protected void ApproveOrder()
        {
            try
            {
                string orderID = Request.Form["orderID"];
                Order order = Erp.Current.WsOrder.Orders.Where(item => item.ID == orderID).FirstOrDefault();
                order.OperatorID = Erp.Current.ID;
                order.Approve(CgOrderStatus.待交货);

                //订单审批通过，需修改具体项归类结果信息
                foreach (var item in order.Orderitems)
                {
                    var term = item.OrderItemsTerm;
                    //判断归类结果是特殊类型的
                    if (!string.IsNullOrWhiteSpace(item.Terms))
                    {
                        term.Embargo = false;
                        term.HkControl = false;
                        term.Enter();
                    }
                }
                //订单审批通过，生成入库通知
                string result = order.CgEntryNotice();
                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 驳回=》订单驳回，待修改
        /// </summary>
        protected void UnApproveOrder()
        {
            try
            {
                string orderID = Request.Form["orderID"];
                Order order = Erp.Current.WsOrder.Orders.Where(item => item.ID == orderID).FirstOrDefault();
                order.OperatorID = Erp.Current.ID;
                //订单驳回，待修改后可重新提交
                order.Approve(CgOrderStatus.暂存);
                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }
    }
}