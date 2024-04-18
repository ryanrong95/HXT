using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 订单运单
    /// </summary>
    public static class OrderWaybillExtends
    {
        public static Layer.Data.Sqls.ScCustoms.OrderWaybills ToLinq(this Models.OrderWaybill entity)
        {
            return new Layer.Data.Sqls.ScCustoms.OrderWaybills
            {
                ID = entity.ID,
                OrderID = entity.OrderID,
                CarrierID = entity.Carrier.ID,
                WaybillCode = entity.WaybillCode,
                //ArrivalDate = entity.ArrivalDate,
                HKDeclareStatus = (int)entity.HKDeclareStatus,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                AdminID = entity.AdminID
            };
        }
    }
}
