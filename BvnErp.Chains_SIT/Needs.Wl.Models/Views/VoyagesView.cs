using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models.Views
{
    public class VoyagesView : UniqueView<Models.Voyage, ScCustomsReponsitory>
    {
        public VoyagesView()
        {

        }

        public VoyagesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.Voyage> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Voyages>()
                   select new Models.Voyage
                   {
                       ID = entity.ID,
                       HKLicense = entity.HKLicense,
                       DriverName = entity.DriverName,
                       DriverCode = entity.DriverCode,
                       CarrierCode = entity.CarrierCode,
                       TransportTime = entity.TransportTime,
                       Type = (Enums.VoyageType)entity.Type,
                       CutStatus = (Enums.CutStatus)entity.CutStatus,
                       HKDeclareStatus = entity.HKDeclareStatus,
                       Status = (Enums.Status)entity.Status,
                       CreateTime = entity.CreateTime,
                       UpdateDate = entity.UpdateDate,
                       Summary = entity.Summary,
                       CarrierType = entity.CarrierType,
                       CarrierName = entity.CarrierName,
                       CarrierQueryMark = entity.CarrierQueryMark,
                       ContactMobile = entity.ContactMobile,
                       CarrierAddress = entity.CarrierAddress,
                       ContactName = entity.ContactName,
                       ContactFax = entity.ContactFax,
                       VehicleType = entity.VehicleType,
                       VehicleLicence = entity.VehicleLicence,
                       VehicleWeight = entity.VehicleWeight,
                       VehicleSize = entity.VehicleSize,
                       DriverMobile = entity.DriverMobile,
                       DriverHSCode = entity.DriverHSCode,
                       DriverHKMobile = entity.DriverHKMobile,
                       DriverCardNo = entity.DriverCardNo,
                       DriverPortElecNo = entity.DriverPortElecNo,
                       DriverLaoPaoCode = entity.DriverLaoPaoCode,
                   };
        }
    }
}
