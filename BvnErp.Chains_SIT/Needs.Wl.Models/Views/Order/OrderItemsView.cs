using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    /// <summary>
    /// 订单的产品名称
    /// 不包含归类、税费信息
    /// </summary>
    public class OrderItemsView : View<Needs.Wl.Models.OrderItem, ScCustomsReponsitory>
    {
        private string OrderID;

        public OrderItemsView(string orderID)
        {
            this.OrderID = orderID;
            this.AllowPaging = false;
        }

        protected override IQueryable<Needs.Wl.Models.OrderItem> GetIQueryable()
        {
            return from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>()
                   where item.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                            && item.OrderID == this.OrderID
                   select new Needs.Wl.Models.OrderItem
                   {
                       ID = item.ID,
                       OrderID = item.OrderID,
                       ProductUniqueCode = item.ProductUniqueCode,
                       Name = item.Name,
                       Model = item.Model,
                       Manufacturer = item.Manufacturer,
                       Batch = item.Batch,
                       Origin = item.Origin,
                       Quantity = item.Quantity,
                       DeclaredQuantity = item.DeclaredQuantity,
                       Unit = item.Unit,
                       UnitPrice = item.UnitPrice,
                       TotalPrice = item.TotalPrice,
                       GrossWeight = item.GrossWeight,
                       IsSampllingCheck = item.IsSampllingCheck,
                       ClassifyStatus = (Enums.ClassifyStatus)item.ClassifyStatus,
                       Status = (int)item.Status,
                       CreateDate = item.CreateDate,
                       UpdateDate = item.UpdateDate,
                       Summary = item.Summary,
                   };
        }
    }
}