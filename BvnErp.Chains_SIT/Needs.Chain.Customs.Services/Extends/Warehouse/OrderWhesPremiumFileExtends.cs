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
    public static class OrderWhesPremiumFileExtends
    {
        public static Layer.Data.Sqls.ScCustoms.OrderWhesPremiumFiles ToLinq(this Models.OrderWhesPremiumFile entity)
        {
            return new Layer.Data.Sqls.ScCustoms.OrderWhesPremiumFiles
            {
                ID = entity.ID,
                AdminID = entity.AdminID,
                OrderWhesPremiumID = entity.OrderWhesPremiumID,
                Name = entity.Name,
                FileType = (int)Enums.FileType.WarehousFeeFile,
                FileFormat = entity.FileFormat,
                URL = entity.URL,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                Summary = entity.Summary
            };
        }
    }
}
