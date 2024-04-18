using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 出库人信息扩展方法
    /// </summary>
    public static class VehicleExtends
    {
        public static Layer.Data.Sqls.ScCustoms.Vehicles ToLinq(this Models.Vehicle entity)
        {
            return new Layer.Data.Sqls.ScCustoms.Vehicles
            {
                ID = entity.ID,                
                Type = (int)entity.VehicleType,
                CarrierID = entity.Carrier.ID,
                License = entity.License,
                HKLicense = entity.HKLicense,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,                
            };
        }
    }
}
