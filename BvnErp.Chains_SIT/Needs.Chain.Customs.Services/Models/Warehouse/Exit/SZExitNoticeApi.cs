using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Views;
using System.Collections.Generic;
using System.Data;

using System.Linq;


namespace Needs.Ccs.Services.Models
{
    public class ExitNoticeRequest
    {
        public List<OrderIdLinkQty> OrderIdLinkQty { get; set; }

        public string AdminID { get; set; }
    }

    public class OrderIdLinkQty
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderID { get; set; }
        /// <summary>
        /// 出库通知下 ,此订单已出库的数量
        /// </summary>
        public int Qty { get; set; }

    }


    /// <summary>
    /// 深圳库房的出库通知
    /// </summary>
    public class SZExitNoticeApi
    {
        public void OutStock(ExitNoticeRequest exitNoticeRequest)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var originAmin = new Needs.Ccs.Services.Views.AdminsTopView2().FirstOrDefault(x => x.ErmAdminID == exitNoticeRequest.AdminID);
                var admin = new Admin
                {
                    ID = originAmin.OriginID,
                    UserName = originAmin.UserName,
                    RealName = originAmin.RealName,

                };
                foreach (var orderidLinkQty in exitNoticeRequest.OrderIdLinkQty)
                {
                    //已出库数量
                    var exitedQty = 0M;
                    var lists = new Needs.Ccs.Services.Views.SubOrderExitQtyView(orderidLinkQty.OrderID).ToList();
                    if (lists.Count != 0)
                    {
                        exitedQty = lists.Sum(t => t.Quantity);
                    }
                    //全部出库完成
                    if (orderidLinkQty.Qty == exitedQty)
                    {
                        //上报出库动作
                        ///更新订单状态
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { OrderStatus = OrderStatus.WarehouseExited }, item => item.ID == orderidLinkQty.OrderID);
                        // 记录订单日志
                        OrderLog log = new OrderLog();
                        log.OrderID = orderidLinkQty.OrderID;
                        log.Admin = admin;
                        log.OrderStatus = OrderStatus.WarehouseExited;
                        log.Summary = "仓库人员[" + admin.RealName + "]完成订单出库，等待客户收货。";
                        log.Enter();
                    }
                }

            }
        }

    }
}