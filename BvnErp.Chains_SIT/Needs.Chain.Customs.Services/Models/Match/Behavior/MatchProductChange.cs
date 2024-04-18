using Needs.Ccs.Services.Enums;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{   
    public class MatchProductChange
    {
        public List<Models.OrderItemAssitant> OrderItems { get; private set; }
        public Order OriginOrder { get; set; }
        public MatchProductChange(List<Models.OrderItemAssitant> orderItems, Order originOrder)
        {
            this.OrderItems = orderItems;
            this.OriginOrder = originOrder;
        }

        public void Change()
        {
            var orderItems = this.OrderItems.Where(t => t.ChangeType == Enums.MatchChangeType.ProductChange);
            foreach (var t in orderItems)
            {
                foreach (var p in t.OrderItemChanges)
                {
                    OrderItemChangeNotice orderItemChangeNotice = new OrderItemChangeNotice();
                    orderItemChangeNotice.Type = p.OrderItemChangeType;
                    orderItemChangeNotice.TriggerSource = TriggerSource.CheckDecListMan;
                    orderItemChangeNotice.OrderItemID = t.ID;
                    orderItemChangeNotice.ProcessState = ProcessState.UnProcess;
                    orderItemChangeNotice.Status = Status.Normal;
                    orderItemChangeNotice.CreateDate = DateTime.Now;
                    orderItemChangeNotice.UpdateDate = DateTime.Now;
                    orderItemChangeNotice.OldValue = p.OldValue;
                    orderItemChangeNotice.NewValue = p.NewValue;
                    orderItemChangeNotice.IsSplited = false;
                    orderItemChangeNotice.Sorter = OriginOrder.Client.Merchandiser;

                    orderItemChangeNotice.Enter();

                    OrderItemChangeLog orderItemChangeLog = new OrderItemChangeLog();
                    orderItemChangeLog.OrderID = OriginOrder.ID;
                    orderItemChangeLog.OrderItemID = t.ID;
                    orderItemChangeLog.Admin = OriginOrder.Client.Merchandiser;
                    orderItemChangeLog.Type = p.OrderItemChangeType;
                    orderItemChangeLog.Summary = "跟单员[" + OriginOrder.Client.Merchandiser.RealName + "]做了" + p.OrderItemChangeType.GetDescription() + "操作,从[" + p.OldValue + "]变更为[" + p.NewValue + "]";
                    orderItemChangeLog.Enter();
                }
            }
        }
    }
}
