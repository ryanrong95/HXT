using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Alls
{
    /// <summary>
    /// 等待自动归类产品的视图
    /// </summary>
    public class AutoClassifyProductsAll : Needs.Linq.View<Models.ClassifyProduct, ScCustomsReponsitory>
    {
        public AutoClassifyProductsAll()
        {
        }
        internal AutoClassifyProductsAll(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ClassifyProduct> GetIQueryable()
        {
            var orders = new Origins.OrdersOrigin(this.Reponsitory).Where(item => item.OrderStatus == Enums.OrderStatus.Confirmed);
            var orderItems = new Origins.OrderItemsOrigin(this.Reponsitory).
                Where(item => item.ClassifyStatus == Enums.ClassifyStatus.Unclassified);
            var linq = from entity in orderItems
                       join order in orders on entity.OrderID equals order.ID
                       select new Models.ClassifyProduct
                       {
                           ID = entity.ID,
                           OrderID = entity.OrderID,
                           OrderType = order.Type,
                           ClientID = order.ClientID,
                           Name = entity.Name,
                           Model = entity.Model,
                           Manufacturer = entity.Manufacturer,
                           Batch = entity.Batch,
                           Origin = entity.Origin,
                           Quantity = entity.Quantity,
                           Unit = entity.Unit,
                           UnitPrice = entity.UnitPrice,
                           Currency = order.Currency,
                       };

            return linq;
        }
    }
}
