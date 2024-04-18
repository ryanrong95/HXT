using System;
using System.Collections.Generic;
using System.Linq;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.Underly;
using Layers.Data.Sqls;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    public class OrderItemOrigin : UniqueView<OrderItem, PvWsOrderReponsitory>
    {
        protected OrderItemOrigin()
        {

        }

        public OrderItemOrigin(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        public OrderItemOrigin(PvWsOrderReponsitory reponsitory, IQueryable<OrderItem> items) : base(reponsitory, items)
        {

        }

        protected override IQueryable<OrderItem> GetIQueryable()
        {
            return from item in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItems>()
                   select new OrderItem
                   {
                       ID = item.ID,
                       OrderID = item.OrderID,
                       InputID = item.InputID,
                       ProductID = item.ProductID,
                       TinyOrderID = item.TinyOrderID,
                       Name = item.CustomName,
                       Origin = item.Origin,
                       DateCode = item.DateCode,
                       Quantity = item.Quantity,
                       Currency = (Currency)item.Currency,
                       UnitPrice = item.UnitPrice,
                       Unit = (LegalUnit)item.Unit,
                       TotalPrice = item.TotalPrice,
                       CreateDate = item.CreateDate,
                       ModifyDate = item.ModifyDate,
                       GrossWeight = item.GrossWeight,
                       Volume = item.Volume,
                       Conditions = item.Conditions,
                       Status = (OrderItemStatus)item.Status,
                       IsAuto = item.IsAuto,
                       WayBillID = item.WayBillID,
                       Type = (OrderItemType)item.Type,
                       OutputID = item.OutputID,
                       StorageID = item.StorageID,
                       CustomName = item.CustomName,
                   };
        }
    }
}
