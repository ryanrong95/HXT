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
    /// 送货人View
    /// </summary>
    public class DeliverView : UniqueView<Models.Deliver, ScCustomsReponsitory>
    {
        public DeliverView()
        {
        }

        internal DeliverView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Deliver> GetIQueryable()
        {
            var driverView = new DriverView(this.Reponsitory);
            var vehicleView = new VehicleView(this.Reponsitory);

            return from deliver in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Delivers>()
                   join driver in driverView on deliver.DriverID equals driver.ID
                   join vehicle in vehicleView on deliver.VehicleID equals vehicle.ID
                   where deliver.Status == (int)Enums.Status.Normal
                   select new Models.Deliver
                   {
                       ID = deliver.ID,
                       Driver = driver,
                       Vehicle = vehicle,
                       Contact = deliver.Contact,
                       Mobile = deliver.Mobile,
                       Address = deliver.Address,
                       Status = (Enums.Status)deliver.Status,
                       CreateDate = deliver.CreateDate,
                       UpdateDate = deliver.UpdateDate
                   };
        }
    }
}
