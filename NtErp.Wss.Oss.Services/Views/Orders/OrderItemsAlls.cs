using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Views
{
    /// <summary>
    /// 订单项视图
    /// </summary>
    public class OrderItemsAlls : UniqueView<Models.OrderItem, CvOssReponsitory>
    {
        internal OrderItemsAlls()
        {

        }
        internal OrderItemsAlls(CvOssReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.OrderItem> GetIQueryable()
        {
            var suppliersView = new SuppliersView(this.Reponsitory);
            var productsView = new StandardProductsView(this.Reponsitory);

            var linq = from entity in Reponsitory.ReadTable<Layer.Data.Sqls.CvOss.OrderItems>()
                       join supplier in suppliersView on entity.SupplierID equals supplier.ID
                       join product in productsView on entity.ProductID equals product.ID
                       select new Models.OrderItem
                       {
                           ID = entity.ID,
                           OrderID = entity.OrderID,
                           ServiceID = entity.ServiceID,
                           CustomerCode = entity.CustomerCode,
                           From = (OrderItemFrom)entity.From,
                           Origin = entity.Origin,
                           Quantity = entity.Quantity,
                           UnitPrice = entity.UnitPrice,
                           Weight = entity.Weight,
                           Status = (OrderItemStatus)entity.Status,
                           Supplier = supplier,
                           Product = product,
                           CreateDate = entity.CreateDate,
                           Leadtime = entity.Leadtime,
                           Note = entity.Note,
                           UpdateDate = entity.UpdateDate
                       };

            return linq;
        }

    }
}
