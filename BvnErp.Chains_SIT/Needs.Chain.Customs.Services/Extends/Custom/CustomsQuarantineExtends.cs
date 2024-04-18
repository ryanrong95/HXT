using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 海关检疫扩展方法
    /// </summary>
    public static class CustomsQuarantineExtends
    {
        public static Layer.Data.Sqls.ScCustoms.CustomsQuarantines ToLinq(this Models.CustomsQuarantine entity)
        {
            return new Layer.Data.Sqls.ScCustoms.CustomsQuarantines
            {
                ID = entity.ID,
                Origin = entity.Origin,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                Summary = entity.Summary
            };
        }
    }
}
