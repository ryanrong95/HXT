using Layers.Data.Sqls;
using System.Linq;
using Yahv.Linq;

namespace Yahv.PsWms.SzMvc.Services.Views.Origins
{

    public class OrderItemsOrigin : UniqueView<Models.Origin.OrderItem, PsOrderRepository>
    {
        #region 构造函数
        public OrderItemsOrigin()
        {
        }

        public OrderItemsOrigin(PsOrderRepository reponsitory) : base(reponsitory)
        {
        }

        #endregion

        protected override IQueryable<Models.Origin.OrderItem> GetIQueryable()
        {
            var view = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PsOrder.OrderItems>()
                       select new Models.Origin.OrderItem
                       {
                           ID = entity.ID,
                           OrderID = entity.OrderID,
                           ProductID = entity.ProductID,
                           Supplier = entity.Supplier,
                           Origin = entity.Origin,
                           CustomCode = entity.CustomCode,
                           StocktakingType = (Enums.StocktakingType)entity.StocktakingType,
                           Mpq = entity.Mpq,
                           PackageNumber = entity.PackageNumber,
                           Total = entity.Total,
                           Currency = (Underly.Currency)entity.Currency,
                           UnitPrice = entity.UnitPrice,
                           StorageID = entity.StorageID,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,
                           Status = (Underly.GeneralStatus)entity.Status,
                       };
            return view;
        }
    }
}
