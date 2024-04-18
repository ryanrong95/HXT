using Needs.Ccs.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 订单变更：
    ///     修改数量 拆分的时候没有修改数量，修改数量在库房
    ///     删除型号 拆分的时候不会删除型号
    ///     新增型号 有新增型号 - 无通知产品录入
    ///     重新归类引起的关税，增值税变更，不在这里插入OrderChange,是在重新归类那里插入OrderChange
    /// </summary>
    public class SCOrderChangeHandler : SCHandler
    {
        public SCOrderChangeHandler(List<MatchViewModel> selectedItems,  Order currentOrder)
        {
            SelectedItems = selectedItems;           
            CurrentOrder = currentOrder;          
        }
        public override void handleRequest()
        {
            try
            {
                var newOrderItems = SelectedItems.Where(t => !string.IsNullOrEmpty(t.NewOrderItemID)).ToList();
                if (newOrderItems.Count() > 0)
                {
                    string orderChangeNoticeID = "";

                    OrderChangeNotice orderChangeNotice = new OrderChangeNotice();
                    orderChangeNotice.ID = ChainsGuid.NewGuidUp();
                    orderChangeNotice.OrderID = CurrentOrder.ID;
                    orderChangeNotice.Type = OrderChangeType.ArrivalChange;
                    orderChangeNotice.processState = ProcessState.Processing;
                    orderChangeNotice.Status = Status.Normal;
                    orderChangeNotice.CreateDate = DateTime.Now;
                    orderChangeNotice.UpdateDate = DateTime.Now;
                    orderChangeNotice.Enter();

                    orderChangeNoticeID = orderChangeNotice.ID;


                    var admin = CurrentOrder.Client.Merchandiser;

                    foreach (var item in newOrderItems)
                    {
                        OrderChangeNoticeLog orderChangeNoticeLog = new OrderChangeNoticeLog();
                        orderChangeNoticeLog.ID = ChainsGuid.NewGuidUp();
                        orderChangeNoticeLog.OrderChangeNoticeID = orderChangeNoticeID;
                        orderChangeNoticeLog.OrderID = CurrentOrder.ID;
                        orderChangeNoticeLog.OrderItemID = item.NewOrderItemID;
                        orderChangeNoticeLog.AdminID = admin.ID;
                        orderChangeNoticeLog.CreateDate = DateTime.Now;
                        orderChangeNoticeLog.Summary = "跟单员【" + admin.RealName + "】,新增了型号【" + item.Model + "】,产地【" + item.Origin + "】";

                        orderChangeNoticeLog.Enter();
                    }
                }

                if (next != null)
                {
                    next.handleRequest();
                }
            }
            catch(Exception ex)
            {
                ex.CcsLog("订单变更出错:CurrentOrderID:" + CurrentOrder.ID );
            }
        }
    }
}
