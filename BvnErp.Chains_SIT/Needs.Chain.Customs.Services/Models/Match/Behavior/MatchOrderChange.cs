using Needs.Ccs.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class MatchOrderChange
    {
        public List<OrderItemChangeCauseOrderChange> OrderChangeItems { get; set; }

        public Order OriginOrder { get; set; }
        public MatchOrderChange(List<OrderItemChangeCauseOrderChange> orderChangeItems, Order originOrder)
        {
            this.OrderChangeItems = orderChangeItems;
            this.OriginOrder = originOrder;
        }

        public void Change()
        {
            if (this.OrderChangeItems.Count() > 0)
            {
                var orderChangeNotice = new Ccs.Services.Views.OrderChangeView().GetTop(1, item => item.OrderID == OriginOrder.ID && item.Type == OrderChangeType.ArrivalChange && item.processState == ProcessState.Processing).FirstOrDefault();
                if (orderChangeNotice != null)
                {
                    orderChangeNotice.EnterSuccess += ChangeItems;
                    orderChangeNotice.Enter();
                }
                else
                {
                    orderChangeNotice = new OrderChangeNotice();
                    orderChangeNotice.ID = ChainsGuid.NewGuidUp();
                    orderChangeNotice.OrderID = OriginOrder.ID;
                    orderChangeNotice.Type = OrderChangeType.ArrivalChange;
                    orderChangeNotice.processState = ProcessState.Processing;
                    orderChangeNotice.Status = Status.Normal;
                    orderChangeNotice.CreateDate = DateTime.Now;
                    orderChangeNotice.UpdateDate = DateTime.Now;
                    orderChangeNotice.EnterSuccess += ChangeItems;
                    orderChangeNotice.Enter();
                }
            }
        }

        private void ChangeItems(object sender, Needs.Linq.SuccessEventArgs e)
        {
            var admin = OriginOrder.Client.Merchandiser;

            foreach (var orderItem in this.OrderChangeItems)
            {
                OrderChangeNoticeLog orderChangeNoticeLog = new OrderChangeNoticeLog();
                orderChangeNoticeLog.ID = ChainsGuid.NewGuidUp();
                orderChangeNoticeLog.OrderChangeNoticeID = e.Object;
                orderChangeNoticeLog.OrderID = OriginOrder.ID;
                orderChangeNoticeLog.OrderItemID = orderItem.OriginalOrderItemID;
                orderChangeNoticeLog.AdminID = admin.ID;
                orderChangeNoticeLog.CreateDate = DateTime.Now;

                switch (orderItem.ReasonType)
                {
                    case OrderChangeCasuedReason.AddOrderItem:
                        orderChangeNoticeLog.Summary = "跟单员【" + admin.RealName + "】,新增了型号【" + orderItem.OriginalModel + "】,产地【" + orderItem.Origin + "】";
                        break;

                    case OrderChangeCasuedReason.DeleteOrderItem:
                        orderChangeNoticeLog.Summary = "跟单员【" + admin.RealName + "】,删除了型号【" + orderItem.OriginalModel + "】";
                        break;

                    case OrderChangeCasuedReason.ChangeQty:
                        orderChangeNoticeLog.Summary = "跟单员【" + admin.RealName + "】,修改了型号【" + orderItem.OriginalModel + "】,数量【" + orderItem.OriginalQty + "】改为【" + orderItem.NowQty + "】";
                        break;

                    default:
                        orderChangeNoticeLog.Summary = "";
                        break;
                }

                orderChangeNoticeLog.Enter();
            }
        }
    }
}
