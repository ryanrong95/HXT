using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class HKClearingDataNew : UniqueView<Models.OrderWaybill, ScCustomsReponsitory>
    {
        public HKClearingDataNew()
        {
        }

        internal HKClearingDataNew(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.OrderWaybill> GetIQueryable()
        {
            var clearingData = from orderWaybillItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderWaybillItems>()
                               join sorting in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>() on orderWaybillItem.SortingID equals sorting.ID
                               join orderWaybill in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderWaybills>() on orderWaybillItem.OrderWaybillID equals orderWaybill.ID
                               join crmCarrier in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PvbCrmCarriersTopView>() on orderWaybill.CarrierID equals crmCarrier.ID
                               join orderItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>() on sorting.OrderItemID equals orderItem.ID
                               join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on orderWaybill.OrderID equals order.ID
                               where orderWaybill.HKDeclareStatus == (int)Enums.HKDeclareStatus.UnDeclare
                               group new { orderWaybill.CreateDate, orderWaybill.WaybillCode, crmCarrier.Code, order.Currency, orderItem.Quantity, orderItem.TotalPrice,orderWaybill.HKDeclareStatus, orderWaybill.ID } by new { orderWaybill.CreateDate, orderWaybill.WaybillCode, crmCarrier.Code, order.Currency,orderWaybill.HKDeclareStatus,orderWaybill.ID } into g
                               select new Models.OrderWaybill
                               {
                                   ID = g.Key.ID,
                                   HKDeclareStatus = (Enums.HKDeclareStatus)g.Key.HKDeclareStatus,
                                   ArrivalDate = g.Key.CreateDate,
                                   WaybillCode = g.Key.WaybillCode,
                                   Carrier = new Models.Carrier
                                   {
                                       Code = g.Key.Code,
                                   },
                                   Currency = g.Key.Currency,
                                   TotalCount = g.Sum(t => t.Quantity),
                                   TotalPrice = g.Sum(t=>t.TotalPrice)
                               };


            //var data = from c in clearingData
            //           where c.HKDeclareStatus == Enums.HKDeclareStatus.UnDeclare
            //           select new Models.OrderWaybill
            //           {
            //               ArrivalDate = c.ArrivalDate,
            //               WaybillCode = c.WaybillCode,
            //               Carrier = c.Carrier,
            //               TotalCount = c.TotalCount,
            //               TotalPrice = c.TotalPrice,
            //               Currency = c.Currency
            //           };

            return clearingData;
        }
    }

    public class HKClearedDataNew : UniqueView<Models.OrderWaybill, ScCustomsReponsitory>
    {
        public HKClearedDataNew()
        {
        }

        internal HKClearedDataNew(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.OrderWaybill> GetIQueryable()
        {
            var clearingData = from orderWaybillItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderWaybillItems>()
                               join sorting in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>() on orderWaybillItem.SortingID equals sorting.ID
                               join orderWaybill in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderWaybills>() on orderWaybillItem.OrderWaybillID equals orderWaybill.ID
                               join crmCarrier in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PvbCrmCarriersTopView>() on orderWaybill.CarrierID equals crmCarrier.ID
                               join orderItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>() on sorting.OrderItemID equals orderItem.ID
                               join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on orderWaybill.OrderID equals order.ID
                               where orderWaybill.HKDeclareStatus == (int)Enums.HKDeclareStatus.Declared
                               group new { orderWaybill.CreateDate, orderWaybill.WaybillCode, crmCarrier.Code, order.Currency, orderItem.Quantity, orderItem.TotalPrice, orderWaybill.HKDeclareStatus, orderWaybill.ID } by new { orderWaybill.CreateDate, orderWaybill.WaybillCode, crmCarrier.Code, order.Currency, orderWaybill.HKDeclareStatus, orderWaybill.ID } into g
                               select new Models.OrderWaybill
                               {
                                   ID = g.Key.ID,
                                   HKDeclareStatus = (Enums.HKDeclareStatus)g.Key.HKDeclareStatus,
                                   ArrivalDate = g.Key.CreateDate,
                                   WaybillCode = g.Key.WaybillCode,
                                   Carrier = new Models.Carrier
                                   {
                                       Code = g.Key.Code,
                                   },
                                   Currency = g.Key.Currency,
                                   TotalCount = g.Sum(t => t.Quantity),
                                   TotalPrice = g.Sum(t => t.TotalPrice)
                               };


            return clearingData;
        }
    }
}
