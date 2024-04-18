using Needs.Ccs.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 调整订单特殊类型
    /// </summary>
    public class SCAdjustOrderVoyages:SCHandler
    {
        public SCAdjustOrderVoyages(Order originOrder, Order currentOrder)
        {
            OriginOrder = originOrder;
            CurrentOrder = currentOrder;
        }
        public override void handleRequest()
        {
            if (CurrentOrder != null)
            {
                string CurrentOrderID = CurrentOrder.ID;
                Order currentOrder = new Needs.Ccs.Services.Views.Orders2View().Where(t => t.ID == CurrentOrderID).FirstOrDefault();
                if (currentOrder.Items.Where(t => t.Category.Type == ItemCategoryType.HighValue).Count() > 0)
                {
                    UpdateOrderVoyages(CurrentOrderID, OrderSpecialType.HighValue, Status.Normal);
                }

                if (currentOrder.Items.Where(t => t.Category.Type == ItemCategoryType.Inspection).Count() > 0)
                {
                    UpdateOrderVoyages(CurrentOrderID, OrderSpecialType.Inspection, Status.Normal);
                }

                if (currentOrder.Items.Where(t => t.Category.Type == ItemCategoryType.Quarantine).Count() > 0)
                {
                    UpdateOrderVoyages(CurrentOrderID, OrderSpecialType.Quarantine, Status.Normal);
                }

                if (currentOrder.Items.Where(t => t.Category.Type == ItemCategoryType.CCC).Count() > 0)
                {
                    UpdateOrderVoyages(CurrentOrderID, OrderSpecialType.CCC, Status.Normal);
                }
            }

            if (OriginOrder != null)
            {
                string OriginOrderID = OriginOrder.ID;
                Order originOrder = new Needs.Ccs.Services.Views.Orders2View().Where(t => t.ID == OriginOrderID).FirstOrDefault();
                if (originOrder.Items.Where(t => t.Category.Type == ItemCategoryType.HighValue).Count() == 0)
                {
                    UpdateOrderVoyages(OriginOrderID, OrderSpecialType.HighValue, Status.Delete);
                }
                if (originOrder.Items.Where(t => t.Category.Type == ItemCategoryType.Inspection).Count() == 0)
                {
                    UpdateOrderVoyages(OriginOrderID, OrderSpecialType.Inspection, Status.Delete);
                }
                if (originOrder.Items.Where(t => t.Category.Type == ItemCategoryType.Quarantine).Count() == 0)
                {
                    UpdateOrderVoyages(OriginOrderID, OrderSpecialType.Quarantine, Status.Delete);
                }
                if (originOrder.Items.Where(t => t.Category.Type == ItemCategoryType.CCC).Count() == 0)
                {
                    UpdateOrderVoyages(OriginOrderID, OrderSpecialType.CCC, Status.Delete);
                }
            }
            
            if (next != null)
            {
                next.handleRequest();
            }
        }

        private void UpdateOrderVoyages(string OrderID, OrderSpecialType Type, Status status)
        {
            var order = new Order
            {
                ID = OrderID
            };
            OrderVoyage orderVoyage = new OrderVoyage();
            orderVoyage.Order = order;
            orderVoyage.Type = Type;
            orderVoyage.Status = status;
            if (status == Status.Normal)
            {
                orderVoyage.Enter();
            }
            else
            {
                orderVoyage.Abandon();
            }
        }
    }
}
