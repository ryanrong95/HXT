using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms.Services.Extends
{
    public static class WayChcd
    {
        public static Layers.Data.Sqls.PvWms.WayChcd ToLinq(this Models.WayChcd entity)
        {
            return new Layers.Data.Sqls.PvWms.WayChcd
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
                TotalQuantity = entity.TotalQuantity,
            };
        }
    }
}
