using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Views.Origins
{
    /// <summary>
    /// 订单项视图
    /// </summary>
    public class OrderItemsOrigin : UniqueView<OrderItem, PvWsOrderReponsitory>
    {
        public OrderItemsOrigin()
        {

        }

        internal OrderItemsOrigin(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<OrderItem> GetIQueryable()
        {
            var linq = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItems>()
                       select new OrderItem
                       {
                           ID = entity.ID,
                           OrderID = entity.OrderID,
                           InputID = entity.InputID,
                           OutputID = entity.OutputID,
                           ProductID = entity.ProductID,
                           CustomName = entity.CustomName,
                           //Origin = entity.Origin,
                           Origin = (Origin)Enum.Parse(typeof(Origin), entity.Origin),
                           DateCode = entity.DateCode,
                           Quantity = entity.Quantity,
                           Currency = (Currency)entity.Currency,
                           UnitPrice = entity.UnitPrice,
                           Unit = (LegalUnit)entity.Unit,
                           TotalPrice = entity.TotalPrice,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,
                           GrossWeight = entity.GrossWeight,
                           Volume = entity.Volume,
                           Conditions = entity.Conditions,
                           Status = (OrderItemStatus)entity.Status,
                           IsAuto = entity.IsAuto,
                           WayBillID = entity.WayBillID,
                           Type = (OrderItemType)entity.Type,
                       };
            return linq;
        }
    }

    /// <summary>
    /// 归类结果视图
    /// </summary>
    public class OrderItemTermsOrigin : UniqueView<OrderItemsTerm, PvWsOrderReponsitory>
    {
        public OrderItemTermsOrigin()
        {

        }

        internal OrderItemTermsOrigin(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<OrderItemsTerm> GetIQueryable()
        {
            var linq = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItemsTerm>()
                       select new OrderItemsTerm
                       {
                           ID = entity.ID,
                           OriginRate = entity.OriginRate,
                           FVARate = entity.FVARate,
                           Ccc = entity.Ccc,
                           Embargo = entity.Embargo,
                           HkControl = entity.HkControl,
                           Coo = entity.Coo,
                           CIQ = entity.CIQ,
                           CIQprice = entity.CIQprice,
                           IsHighPrice = entity.IsHighPrice,
                           IsDisinfected = entity.IsDisinfected
                       };
            return linq;
        }
    }

    /// <summary>
    /// 归类视图
    /// </summary>
    public class OrderItemsChcdOrigin : UniqueView<OrderItemsChcd, PvWsOrderReponsitory>
    {
        public OrderItemsChcdOrigin()
        {

        }

        internal OrderItemsChcdOrigin(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<OrderItemsChcd> GetIQueryable()
        {
            var linq = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItemsChcd>()
                       select new OrderItemsChcd
                       {
                           ID = entity.ID,
                           AutoHSCodeID = entity.AutoHSCodeID,
                           AutoDate = entity.AutoDate,
                           FirstAdminID = entity.FirstAdminID,
                           FirstHSCodeID = entity.FirstHSCodeID,
                           FirstDate = entity.FirstDate,
                           SecondAdminID = entity.SecondAdminID,
                           SecondHSCodeID = entity.SecondHSCodeID,
                           SecondDate = entity.SecondDate,
                           CustomHSCodeID = entity.CustomHSCodeID,
                           CustomTaxCode = entity.CustomTaxCode,
                           SysPriceID = entity.SysPriceID,
                           CustomsPriceID = entity.CustomsPriceID,
                           VATaxedPriceID = entity.VATaxedPriceID,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,
                       };
            return linq;
        }
    }
}
