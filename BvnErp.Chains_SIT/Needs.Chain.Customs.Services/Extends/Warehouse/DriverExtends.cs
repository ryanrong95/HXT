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
    public static class DriverExtends
    {
        public static Layer.Data.Sqls.ScCustoms.Drivers ToLinq(this Models.Driver entity)
        {
            return new Layer.Data.Sqls.ScCustoms.Drivers
            {
                ID = entity.ID,
                Name = entity.Name,
                Mobile = entity.Mobile,
                License = entity.License,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                CarrierID=entity.Carrier.ID,
                HSCode=entity.HSCode,
                DriverCardNo=entity.DriverCardNo,
                HKMobile=entity.HKMobile,
                LaoPaoCode=entity.LaoPaoCode,
                PortElecNo=entity.PortElecNo,
                IsChcd=entity.IsChcd
                
            };
        }
    }
}
