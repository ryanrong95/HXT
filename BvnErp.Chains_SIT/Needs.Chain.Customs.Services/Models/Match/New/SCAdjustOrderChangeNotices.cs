using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 拆分之后调整订单变更，没拆之前OrderItemA有关税变更，然后在OrderChangeNotices表中的OrderID是-01
    /// 拆分之后，OrderItemA的订单是-02，然后在OrderChangeNotices表中的OrderID是
    /// </summary>
    public class SCAdjustOrderChangeNotices : SCHandler
    {
        public SCAdjustOrderChangeNotices(List<MatchViewModel> selectedItems,  Order currentOrder)
        {
            SelectedItems = selectedItems;
            CurrentOrder = currentOrder;
        }
        public override void handleRequest()
        {
            List<string> orderItemIDs = SelectedItems.Where(t => !string.IsNullOrEmpty(t.OrderItemID)).Select(t => t.OrderItemID).ToList();
            string CurrentOrderID = CurrentOrder.ID;

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var changedOrderItem = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>().Where(t => t.OrderID == CurrentOrderID && orderItemIDs.Contains(t.ID)).Select(t => t.ID).ToList();
                foreach (var item in changedOrderItem)
                {
                    var orderChangeNoticeLog = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderChangeNoticeLogs>().Where(t => t.OrderItemID == item).FirstOrDefault();
                    if (orderChangeNoticeLog != null)
                    {
                        if (orderChangeNoticeLog.OrderID.Trim() != CurrentOrderID.Trim())
                        {
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderChangeNoticeLogs>(new { OrderID = CurrentOrderID }, t => t.ID == orderChangeNoticeLog.ID);
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderChangeNotices>(new { OderID = CurrentOrderID }, t => t.ID == orderChangeNoticeLog.OrderChangeNoticeID);
                        }
                    }
                }

            }

            if (next != null)
            {
                next.handleRequest();
            }
        }
    }
}
