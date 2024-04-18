using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class SCAutoClassifyHandler : SCHandler
    {
        public List<ClassifyProduct> ClassifyProducts { get; set; }

        public SCAutoClassifyHandler(List<MatchViewModel> selectedItems,Order currentOrder)
        {
            SelectedItems = selectedItems;
            CurrentOrder = currentOrder;
        }
        public override void handleRequest()
        {
            var orderItems = SelectedItems.Where(t => !string.IsNullOrEmpty(t.NewOrderItemID)).ToList();
            List<ClassifyProduct> ClassifyProducts = new List<ClassifyProduct>();
            foreach (var item in orderItems)
            {
                ClassifyProduct classifyProduct = new ClassifyProduct();
                classifyProduct.ID = item.NewOrderItemID;
                classifyProduct.Client = CurrentOrder.Client;
                classifyProduct.Model = item.Model;
                classifyProduct.Manufacturer = item.Brand;
                classifyProduct.UnitPrice = item.UnitPrice;
                classifyProduct.Origin = item.Origin;
                classifyProduct.Batch = item.BatchNo;
                classifyProduct.Unit = item.Unit;
                classifyProduct.Name = item.Name;
                classifyProduct.Quantity = item.Qty;
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

            this.ClassifyProducts = ClassifyProducts;
        }
    }
}
