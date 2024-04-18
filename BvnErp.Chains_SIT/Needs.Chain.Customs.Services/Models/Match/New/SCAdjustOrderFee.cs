using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 订单费用调整
    /// 拆分后调整订单费用，没拆之前OrderItemA有费用，记录在OrderPremiums表中的OrderID是-01
    /// 拆分之后，OrderItemA的订单是-02，但是OrderPremiums中的OrderID没改
    /// </summary>
    public class SCAdjustOrderFee : SCHandler
    {
        public SCAdjustOrderFee(Order currentOrder)
        {
            CurrentOrder = currentOrder;
        }
        public override void handleRequest()
        {
            string CurrentOrderID = CurrentOrder.ID;           
            var OrderItemIDs = CurrentOrder.Items.Select(t => t.ID).ToList();
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                foreach (var orderItemID in OrderItemIDs)
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPremiums>().Where(t => t.OrderItemID == orderItemID).Count();
                    if (count > 0)
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderPremiums>(new { OrderID = CurrentOrderID }, t => t.OrderItemID == orderItemID);
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
