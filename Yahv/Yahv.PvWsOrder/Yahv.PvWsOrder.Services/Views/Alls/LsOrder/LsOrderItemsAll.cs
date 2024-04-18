using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Services.Views;

namespace Yahv.PvWsOrder.Services.Views
{
    public class LsOrderItemsAll : UniqueView<LsOrderItem, PvWsOrderReponsitory>
    {
        public LsOrderItemsAll()
        {

        }
        protected override IQueryable<LsOrderItem> GetIQueryable()
        {
            var orderItemView = new LsOrderItemTopView<PvWsOrderReponsitory>(this.Reponsitory)
                .Where(item => item.Status != Underly.GeneralStatus.Closed || item.Status != Underly.GeneralStatus.Deleted);

            var linq = from entity in orderItemView
                       select new LsOrderItem
                       {
                           ID = entity.ID,
                           OrderID = entity.OrderID,
                           Quantity = entity.Quantity,
                           Currency = entity.Currency,
                           UnitPrice = entity.UnitPrice,
                           ProductID = entity.ProductID,
                           Description = entity.Description,
                           Supplier = entity.Supplier,
                           Status = entity.Status,
                           CreateDate = entity.CreateDate,
                           Lease = entity.Lease,

                           Product = entity.Product,
                       };
            return linq;
        }
    }
}
