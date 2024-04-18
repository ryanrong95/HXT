using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models.Views
{
    public class VehiclesView : UniqueView<Models.Vehicle, ScCustomsReponsitory>
    {
        public VehiclesView()
        {

        }

        public VehiclesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.Vehicle> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Vehicles>()
                   select new Models.Vehicle
                   {
                       ID = entity.ID,
                       CarrierID = entity.CarrierID,
                       Type = (Enums.VehicleType)entity.Type,
                       License = entity.License,
                       HKLicense = entity.HKLicense,
                       Status = (Enums.Status)entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Weight = entity.Weight,
                       Size = entity.Size
                   };
        }
    }
}
