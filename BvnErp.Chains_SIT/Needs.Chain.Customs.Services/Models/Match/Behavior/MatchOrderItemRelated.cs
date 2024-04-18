using Needs.Ccs.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class MatchOrderItemRelated
    {
        public List<Models.OrderItemAssitant> OrderItems { get; private set; }
        public string CurrentOrderID { get; set; }
        public MatchOrderItemRelated(List<Models.OrderItemAssitant> orderItems,string currentOrderID)
        {
            this.OrderItems = orderItems;
            this.CurrentOrderID = currentOrderID;
        }

        public List<Models.OrderItemAssitant> Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory(false))
            {
                foreach (var orderitem in this.OrderItems)
                {
                    switch (orderitem.PersistenceType)
                    {                       
                        case PersistenceType.Update:
                            if (!string.IsNullOrEmpty(orderitem.OrderID))
                            {
                                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItems>(new
                                {
                                    OrderID = orderitem.OrderID,
                                    Origin = orderitem.Origin,
                                    Quantity = orderitem.Quantity,
                                    TotalPrice = orderitem.TotalPrice,
                                    Name = orderitem.Name,
                                    Model = orderitem.Model,
                                    Manufacturer = orderitem.Manufacturer,
                                    Batch = orderitem.Batch
                                }, item => item.ID == orderitem.ID);
                            }
                            else
                            {
                                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItems>(new
                                {
                                    Quantity = orderitem.Quantity,
                                    TotalPrice = orderitem.TotalPrice
                                }, item => item.ID == orderitem.ID);
                            }
                            break;

                        case PersistenceType.Insert:
                            var prefix = System.Configuration.ConfigurationManager.AppSettings["Purchaser"];
                            OrderItem newOrderItem = new OrderItem();
                            string singleOrderItemID = prefix + Needs.Overall.PKeySigner.Pick(PKeyType.OrderItem);
                            orderitem.ID = singleOrderItemID;
                            newOrderItem.ID = singleOrderItemID;
                            newOrderItem.OrderID = this.CurrentOrderID;
                            newOrderItem.Origin = orderitem.Origin;
                            newOrderItem.Quantity = orderitem.Quantity;
                            newOrderItem.Unit = orderitem.Unit;
                            newOrderItem.UnitPrice = orderitem.UnitPrice;
                            newOrderItem.TotalPrice = orderitem.TotalPrice;
                            newOrderItem.IsSampllingCheck = false;
                            newOrderItem.ClassifyStatus = Enums.ClassifyStatus.Unclassified;
                            newOrderItem.Status = Enums.Status.Normal;
                            newOrderItem.CreateDate = DateTime.Now;
                            newOrderItem.UpdateDate = DateTime.Now;
                            newOrderItem.Name = orderitem.Name;
                            newOrderItem.Model = orderitem.Model;
                            newOrderItem.Manufacturer = orderitem.Manufacturer;
                            newOrderItem.Batch = orderitem.Batch;
                            reponsitory.Insert(newOrderItem.ToLinq());
                            break;

                        default:
                            break;
                    }
                }

                reponsitory.Submit();
            }

            return this.OrderItems;
        }
    }
}
