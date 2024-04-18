using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 订单国际快递的视图
    /// </summary>
    public class OrderWaybillViewBase : UniqueView<Models.OrderWaybill, ScCustomsReponsitory>
    {
        public OrderWaybillViewBase()
        {
        }

        internal OrderWaybillViewBase(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.OrderWaybill> GetIQueryable()
        {
            var carrierView = new CarriersView(this.Reponsitory);

            return from waybill in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderWaybills>()
                   join carrier in carrierView on waybill.CarrierID equals carrier.ID
                   where waybill.Status == (int)Enums.Status.Normal
                   orderby waybill.ArrivalDate
                   select new Models.OrderWaybill
                   {
                       ID = waybill.ID,
                       OrderID = waybill.OrderID,
                       Carrier = carrier,
                       WaybillCode = waybill.WaybillCode,
                       //ArrivalDate = waybill.ArrivalDate,
                       AdminID = waybill.AdminID,
                       HKDeclareStatus = (Enums.HKDeclareStatus)waybill.HKDeclareStatus,
                       Status = (Enums.Status)waybill.Status,
                       CreateDate = waybill.CreateDate
                   };
        }
    }

    /// <summary>
    /// 运单
    /// </summary>
    /// </summary>
    public class OrderWaybillView : OrderWaybillViewBase
    {
        public OrderWaybillView()
        {
        }

        internal OrderWaybillView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.OrderWaybill> GetIQueryable()
        {
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var waybillItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderWaybillItems>();
            var sortingView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>().Where(item => item.Status == (int)Enums.Status.Normal);
            var orderItemView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>().Where(item => item.Status == (int)Enums.Status.Normal);

            var ItemResult = from waybillItem in waybillItems
                             join sorting in sortingView on waybillItem.SortingID equals sorting.ID
                             join orderItem in orderItemView on sorting.OrderItemID equals orderItem.ID
                             select new
                             {
                                 OrderWaybillID = waybillItem.OrderWaybillID,
                                 Quantity = sorting.Quantity,
                                 UnitPrice = orderItem.UnitPrice,
                             };
            var TotalResult = from item in ItemResult
                              group item by item.OrderWaybillID into g
                              select new
                              {
                                  OrderWaybillID = g.Key,
                                  TotalCount = g.Sum(t => t.Quantity),
                                  TotalPrice = g.Sum(t => t.Quantity * t.UnitPrice),
                              };

            return from waybill in base.GetIQueryable()
                   join order in orders on waybill.OrderID equals order.ID
                   join result in TotalResult on waybill.ID equals result.OrderWaybillID into results
                   from result in results.DefaultIfEmpty()
                   select new Models.OrderWaybill
                   {
                       ID = waybill.ID,
                       OrderID = waybill.OrderID,
                       Carrier = waybill.Carrier,
                       WaybillCode = waybill.WaybillCode,
                       ArrivalDate = waybill.ArrivalDate,
                       AdminID = waybill.AdminID,
                       HKDeclareStatus = waybill.HKDeclareStatus,
                       Status = waybill.Status,
                       CreateDate = waybill.CreateDate,
                       Currency = order.Currency,
                       TotalCount = result == null ? 0M : result.TotalCount,
                       TotalPrice = result == null ? 0M : result.TotalPrice,
                   };
        }
    }

}
