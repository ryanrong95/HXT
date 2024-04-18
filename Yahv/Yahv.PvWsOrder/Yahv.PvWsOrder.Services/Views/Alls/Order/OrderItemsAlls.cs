using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Models;
using Yahv.PvWsOrder.Services.Views.Origins;
using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace Yahv.PvWsOrder.Services.Views
{
    /// <summary>
    /// 订单项视图
    /// </summary>
    public class OrderItemsAlls : UniqueView<OrderItem, PvWsOrderReponsitory>
    {
        public OrderItemsAlls()
        {

        }

        internal OrderItemsAlls(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<OrderItem> GetIQueryable()
        {
            var productView = new ProductsAll(this.Reponsitory);
            var termView = new OrderItemTermsOrigin(this.Reponsitory);
            var chcdView = new OrderItemsChcdOrigin(this.Reponsitory);

            var linq = from orderItem in Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItems>()
                   join product in productView on orderItem.ProductID equals product.ID
                   join term in termView on orderItem.ID equals term.ID into terms
                   from term in terms.DefaultIfEmpty()
                   join chcd in chcdView on orderItem.ID equals chcd.ID into chcds
                   from chcd in chcds.DefaultIfEmpty()
                   where orderItem.Status != (int)OrderItemStatus.Deleted
                   select new OrderItem
                   {
                       ID = orderItem.ID,
                       OrderID = orderItem.OrderID,
                       TinyOrderID = orderItem.TinyOrderID,
                       InputID = orderItem.InputID,
                       OutputID = orderItem.OutputID,
                       ProductID = orderItem.ProductID,
                       CustomName = orderItem.CustomName,
                       Origin = (Origin)(Enum.Parse(typeof(Origin), orderItem.Origin == null? "Unknown" : orderItem.Origin)),
                       DateCode = orderItem.DateCode,
                       Quantity = orderItem.Quantity,
                       Currency = (Currency)orderItem.Currency,
                       UnitPrice = orderItem.UnitPrice,
                       Unit = (LegalUnit)orderItem.Unit,
                       TotalPrice = orderItem.TotalPrice,
                       CreateDate = orderItem.CreateDate,
                       ModifyDate = orderItem.ModifyDate,
                       GrossWeight = orderItem.GrossWeight,
                       Volume = orderItem.Volume,
                       Conditions = orderItem.Conditions,
                       Status = (OrderItemStatus)orderItem.Status,
                       IsAuto = orderItem.IsAuto,
                       WayBillID = orderItem.WayBillID,
                       Type = (OrderItemType)orderItem.Type,
                       StorageID = orderItem.StorageID,

                       Product = product,
                       OrderItemsTerm = term,
                       OrderItemsChcd = chcd,
                   };

            return linq;
        }
    }
}
