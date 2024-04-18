using Layers.Data.Sqls;
using System.Linq;
using Yahv.Linq;

namespace Yahv.PsWms.SzMvc.Services.Views.Alls
{

    public class OrderItemsAll : UniqueView<Models.Origin.OrderItem, PsOrderRepository>
    {
        #region 构造函数
        public OrderItemsAll()
        {
        }

        public OrderItemsAll(PsOrderRepository reponsitory) : base(reponsitory)
        {
        }

        #endregion

        protected override IQueryable<Models.Origin.OrderItem> GetIQueryable()
        {
            var items = new Origins.OrderItemsOrigin(this.Reponsitory).Where(t => t.Status == Underly.GeneralStatus.Normal);
            var products = new Origins.ProductsOrigin(this.Reponsitory);
            var view = from entity in items
                       join product in products on entity.ProductID equals product.ID
                       select new Models.Origin.OrderItem
                       {
                           ID = entity.ID,
                           OrderID = entity.OrderID,
                           ProductID = entity.ProductID,
                           Supplier = entity.Supplier,
                           Origin = entity.Origin,
                           CustomCode = entity.CustomCode,
                           StocktakingType = entity.StocktakingType,
                           Mpq = entity.Mpq,
                           PackageNumber = entity.PackageNumber,
                           Total = entity.Total,
                           Currency = entity.Currency,
                           UnitPrice = entity.UnitPrice,
                           StorageID = entity.StorageID,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,
                           Status = entity.Status,

                           Product = product,
                       };
            return view;
        }
    }
}
