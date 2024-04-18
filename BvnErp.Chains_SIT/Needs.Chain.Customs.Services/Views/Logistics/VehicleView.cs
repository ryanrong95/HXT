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
    /// /// <summary>
    /// 交通工具View
    /// </summary>
    /// </summary>
    public class VehicleView : UniqueView<Models.Vehicle, ScCustomsReponsitory>
    {
        public VehicleView()
        {
        }

        internal VehicleView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Vehicle> GetIQueryable()
        {
            var carrierView = new CarriersView(this.Reponsitory);

            return from Vehicle in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Vehicles>()
                   join carrier in carrierView on Vehicle.CarrierID equals carrier.ID
                   //where Vehicle.Status==(int)Enums.Status.Normal
                   select new Models.Vehicle
                   {
                       ID = Vehicle.ID,
                       Carrier =carrier,
                       VehicleType = (Enums.VehicleType)Vehicle.Type,
                       License = Vehicle.License,
                       HKLicense = Vehicle.HKLicense,
                       Status = (Enums.Status)Vehicle.Status,
                       CreateDate = Vehicle.CreateDate,
                       UpdateDate = Vehicle.UpdateDate,
                       Weight=Vehicle.Weight,
                       Size = Vehicle.Size
                   };
        }
    }

}
