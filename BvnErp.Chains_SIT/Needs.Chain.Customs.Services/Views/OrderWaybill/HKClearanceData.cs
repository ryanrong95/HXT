using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class HKClearingData : UniqueView<Models.OrderWaybill, ScCustomsReponsitory>
    {
        public HKClearingData()
        {
        }

        internal HKClearingData(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.OrderWaybill> GetIQueryable()
        {
            var clearingData = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.HKClearanceData>()
                               group c by new
                               {
                                   c.wbCode,
                                   c.CarrierName,
                                   c.ClearingDate,
                                   c.Currency
                               } into d
                               select new Models.OrderWaybill
                               {
                                   ArrivalDate = Convert.ToDateTime(d.Key.ClearingDate),
                                   WaybillCode = d.Key.wbCode,
                                   Carrier = new Models.Carrier
                                   {
                                       Code = d.Key.CarrierName
                                   },
                                   TotalCount = d.Sum(t => t.total),
                                   TotalPrice = d.Sum(t => t.CargoValue),
                                   Currency = d.Key.Currency
                               };

            var clearedDataID = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderWayBillClearedLogs>()
                                where c.Status == (int)Enums.Status.Normal
                                select new
                                {
                                    WayBillCode = c.WaybillCode,
                                    Currency = c.Currency
                                };


            var data = from c in clearingData
                       where !clearedDataID.Any(t=>t.WayBillCode==c.WaybillCode&&t.Currency==c.Currency)
                       select new Models.OrderWaybill
                       {
                           ArrivalDate = c.ArrivalDate,
                           WaybillCode = c.WaybillCode,
                           Carrier = c.Carrier,
                           TotalCount = c.TotalCount,
                           TotalPrice = c.TotalPrice,
                           Currency = c.Currency
                       };

            return data;
        }
    }

    public class HKClearedData : UniqueView<Models.OrderWaybill, ScCustomsReponsitory>
    {
        public HKClearedData()
        {
        }

        internal HKClearedData(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Models.OrderWaybill> GetIQueryable()
        {
            var clearedDataID = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderWayBillClearedLogs>()
                                where c.Status == (int)Enums.Status.Normal
                                select new
                                {
                                    WayBillCode = c.WaybillCode,
                                    Currency = c.Currency
                                };

            var clearingData = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.HKClearanceData>()
                               group c by new
                               {
                                   c.wbCode,
                                   c.CarrierName,
                                   c.ClearingDate,
                                   c.Currency
                               } into d
                               select new Models.OrderWaybill
                               {
                                   ArrivalDate = Convert.ToDateTime(d.Key.ClearingDate),
                                   WaybillCode = d.Key.wbCode,
                                   Carrier = new Models.Carrier
                                   {
                                       Code = d.Key.CarrierName
                                   },
                                   TotalCount = d.Sum(t => t.total),
                                   TotalPrice = d.Sum(t => t.CargoValue),
                                   Currency = d.Key.Currency
                               };

            var data = from c in clearedDataID
                       join d in clearingData on
                       new
                       {
                           WayBillCode = c.WayBillCode,
                           Currency = c.Currency
                       }
                       equals new
                       {
                           WayBillCode = d.WaybillCode,
                           Currency = d.Currency
                       }
                       select new Models.OrderWaybill
                       {
                           ArrivalDate = d.ArrivalDate,
                           WaybillCode = c.WayBillCode,
                           Carrier = d.Carrier,
                           TotalCount = d.TotalCount,
                           TotalPrice = d.TotalPrice,
                           Currency = d.Currency,
                           HKDeclareStatus = Enums.HKDeclareStatus.Declared
                       };

            return data;
        }
    }

    /// <summary>
    /// 跟单查看国际快递用
    /// </summary>
    public class HKClearanceData : UniqueView<Models.OrderWaybill, ScCustomsReponsitory>
    {
        public HKClearanceData()
        {
        }

        internal HKClearanceData(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<OrderWaybill> GetIQueryable()
        {
            var clearanceData = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.HKClearanceData>()
                              
                               select new Models.OrderWaybill
                               {
                                   OrderID = c.TinyOrderID,
                                   ArrivalDate = Convert.ToDateTime(c.ClearingDate),
                                   WaybillCode = c.wbCode,
                                   Carrier = new Models.Carrier
                                   {
                                       Code = c.CarrierName
                                   }
                               };

            return clearanceData;
        }
    }

}
