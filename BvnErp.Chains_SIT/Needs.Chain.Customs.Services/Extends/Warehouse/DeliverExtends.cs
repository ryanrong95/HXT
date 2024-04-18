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
    public static class DeliverExtends
    {
        public static Layer.Data.Sqls.ScCustoms.Delivers ToLinq(this Models.Deliver entity)
        {
            return new Layer.Data.Sqls.ScCustoms.Delivers
            {
                ID = entity.ID,
                Contact = entity.Contact,
                Mobile = entity.Mobile,
                Address = entity.Address,
                DriverID = entity.Driver.ID,
                VehicleID = entity.Vehicle.ID,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate
            };
        }
    }
}
