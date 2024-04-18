using Needs.Ccs.Services.ApiSettings;
using Needs.Ccs.Services.Enums;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class MatchAutoClassify
    {
        public List<Models.OrderItemAssitant> OrderItems { get; private set; }
        public Order CurrentOrder { get; set; }

        public MatchAutoClassify(List<Models.OrderItemAssitant> orderItems, Order currentOrder)
        {
            this.OrderItems = orderItems;
            this.CurrentOrder = currentOrder;
        }

        public List<ClassifyProduct> Classify()
        {
            //只有订单变更 新增的OrderItem 才需要自动归类
            var orderItems = this.OrderItems.Where(t => t.ChangeType == Enums.MatchChangeType.OrderChange).ToList();
            List<ClassifyProduct> ClassifyProducts = new List<ClassifyProduct>();
            foreach (var item in orderItems)
            {
                ClassifyProduct classifyProduct = new ClassifyProduct();
                classifyProduct.ID = item.ID;
                classifyProduct.Client = CurrentOrder.Client;
                classifyProduct.Model = item.Model;
                classifyProduct.Manufacturer = item.Manufacturer;
                classifyProduct.UnitPrice = item.UnitPrice;
                classifyProduct.Origin = item.Origin;
                classifyProduct.Batch = item.Batch;
                classifyProduct.Unit = item.Unit;
                classifyProduct.Name = item.Name;
                classifyProduct.Quantity = item.Quantity;
                classifyProduct.TotalPrice = item.TotalPrice;
                classifyProduct.OrderID = CurrentOrder.ID;
                classifyProduct.OrderType = CurrentOrder.Type;

                ClassifyProducts.Add(classifyProduct);             
            }

            foreach (var classifyProduct in ClassifyProducts)
            {
                try
                {
                    var autoCategory = new AutoClassify(classifyProduct);
                    autoCategory.DoClassify();
                }
                catch (Exception ex)
                {
                    ex.CcsLog("订单ID【" + classifyProduct.OrderID + "】，订单项ID【" + classifyProduct.ID + "】自动归类异常");
                    continue;
                }
            }

            return ClassifyProducts;
        }
    }
}
