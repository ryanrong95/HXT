using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;
using System.Linq.Expressions;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 货物提货委托书的视图
    /// </summary>
    public class DeliveryAgentProxiesView : Needs.Linq.Generic.Unique1Classics<Models.DeliveryAgentProxy, ScCustomsReponsitory>
    {
        protected override IQueryable<DeliveryAgentProxy> GetIQueryable(Expression<Func<DeliveryAgentProxy, bool>> expression, params LambdaExpression[] expressions)
        {
            var linq = from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Voyages>()
                       select new Models.DeliveryAgentProxy
                       {
                           ID = entity.ID,
                           CarrierCode = entity.CarrierCode,
                           DriverName = entity.DriverName,
                           HKLicense = entity.HKLicense,
                           TransportTime = entity.TransportTime,
                           VoyageDriverCode = entity.DriverCode,
                       };

            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<Models.DeliveryAgentProxy, bool>>);
            }

            return linq.Where(expression);
        }

        protected override IEnumerable<DeliveryAgentProxy> OnReadShips(DeliveryAgentProxy[] results)
        {
            var carriersView = new CarriersView(this.Reponsitory);
            var driversView = new DriverView(this.Reponsitory);
            var vehiclesView = new VehicleView(this.Reponsitory);
            var decHeadsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>().Where(dh => dh.CusDecStatus != "04");

            return from result in results
                       //防止承运商、司机、车辆没有关联数据或多条数据
                   join carrier in carriersView on result.CarrierCode equals carrier.Code into carriers
                   join driver in driversView on result.DriverName equals driver.Name into drivers
                   join vehicle in vehiclesView on result.HKLicense equals vehicle.HKLicense into vehicles
                   join decHead in decHeadsView on result.ID equals decHead.VoyNo into decHeads
                   select new Models.DeliveryAgentProxy
                   {
                       ID = result.ID,
                       CarrierCode = result.CarrierCode,
                       Carrier = carriers.FirstOrDefault(),
                       DriverName = result.DriverName,
                       Driver = drivers.FirstOrDefault(),
                       HKLicense = result.HKLicense,
                       Vehicle = string.IsNullOrEmpty(result.HKLicense) ? null : vehicles.FirstOrDefault(),
                       TransportTime = result.TransportTime,
                       TotalPackNo = decHeads.Sum(dh => dh.PackNo),
                       TotalGrossWt = decHeads.Sum(dh => dh.GrossWt),
                       VoyageDriverCode = result.VoyageDriverCode,
                   };
        }
    }
}
