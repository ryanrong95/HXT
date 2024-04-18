using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Origins
{
    public class OrderItemsOrigin : UniqueView<Models.OrderItem, ScCustomsReponsitory>
    {
        public OrderItemsOrigin()
        {
        }

        public OrderItemsOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.OrderItem> GetIQueryable()
        {
            //var productsView = new ProductsViews(this.Reponsitory);
            return from orderItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>()
                   //join product in productsView on orderItem.ProductID equals product.ID
                   select new Models.OrderItem
                   {
                       ID = orderItem.ID,
                       OrderID = orderItem.OrderID,
                       Model=orderItem.Model,
                       Name=orderItem.Name,
                       Manufacturer=orderItem.Manufacturer,
                       Batch=orderItem.Batch,
                       //Product = product,
                       Origin = orderItem.Origin,
                       Quantity = orderItem.Quantity,
                       DeclaredQuantity = orderItem.DeclaredQuantity,
                       Unit = orderItem.Unit,
                       UnitPrice = orderItem.UnitPrice,
                       TotalPrice = orderItem.TotalPrice,
                       GrossWeight = orderItem.GrossWeight,
                       IsSampllingCheck = orderItem.IsSampllingCheck,
                       ClassifyStatus = (Enums.ClassifyStatus)orderItem.ClassifyStatus,
                       ProductUniqueCode = orderItem.ProductUniqueCode,
                       Status = (Enums.Status)orderItem.Status,
                       CreateDate = orderItem.CreateDate,
                       UpdateDate = orderItem.UpdateDate,
                       Summary = orderItem.Summary,
                   };
        }
    }
}
