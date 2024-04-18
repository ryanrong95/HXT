using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models.Views
{
    public class DriversView : UniqueView<Models.Driver, ScCustomsReponsitory>
    {
        public DriversView()
        {

        }

        public DriversView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.Driver> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Drivers>()
                   select new Models.Driver
                   {
                       ID = entity.ID,
                       CarrierID = entity.CarrierID,
                       Name = entity.Name,
                       Code = entity.Code,
                       Mobile = entity.Mobile,
                       License = entity.License,
                       Status = (Enums.Status)entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       HSCode = entity.HSCode,
                       DriverCardNo = entity.DriverCardNo,
                       HKMobile = entity.HKMobile,
                       PortElecNo = entity.PortElecNo,
                       LaoPaoCode = entity.LaoPaoCode,
                       IsChcd = entity.IsChcd
                   };
        }
    }
}
