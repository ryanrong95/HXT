using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class OrderWaybillOrigin : UniqueView<Models.OrderWaybill, ScCustomsReponsitory>
    {
        public OrderWaybillOrigin()
        {
        }

        internal OrderWaybillOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.OrderWaybill> GetIQueryable()
        {
            var carrierView = new CarriersView(this.Reponsitory);

            return from waybill in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderWaybills>()                  
                   where waybill.Status == (int)Enums.Status.Normal
                   orderby waybill.ArrivalDate
                   select new Models.OrderWaybill
                   {
                       ID = waybill.ID,
                       OrderID = waybill.OrderID,                      
                       WaybillCode = waybill.WaybillCode,                  
                       AdminID = waybill.AdminID,
                       HKDeclareStatus = (Enums.HKDeclareStatus)waybill.HKDeclareStatus,
                       Status = (Enums.Status)waybill.Status,
                       CreateDate = waybill.CreateDate
                   };
        }
    }
}
