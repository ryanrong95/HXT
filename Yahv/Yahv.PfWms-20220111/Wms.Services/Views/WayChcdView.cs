using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Models;
using Yahv.Linq;

namespace Wms.Services.Views
{
    public class WayChcdView : UniqueView<Models.WayChcd, PvWmsRepository>
    {
        protected override IQueryable<WayChcd> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable <Layers.Data.Sqls.PvWms.WayChcd>()
                   select new WayChcd
                   {
                       ID = entity.ID,
                       LotNumber = entity.LotNumber,
                       CarNumber1 = entity.CarNumber1,
                       CarNumber2 = entity.CarNumber2,
                       Carload = entity.Carload,
                       IsOnevehicle = entity.IsOnevehicle,
                       Driver = entity.Driver,
                       PlanDate = entity.PlanDate,
                       DepartDate = entity.DepartDate,
                       TotalQuantity = entity.TotalQuantity
                   };
        }
    }
}
