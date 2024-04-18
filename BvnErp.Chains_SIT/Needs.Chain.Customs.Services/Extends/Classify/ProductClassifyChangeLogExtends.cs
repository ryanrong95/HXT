using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 产品归类变更日志的扩展方法
    /// </summary>
    public static class ProductClassifyChangeLogExtends
    {
        public static Layer.Data.Sqls.ScCustoms.ProductClassifyChangeLogs ToLinq(this Models.ProductClassifyChangeLog entity)
        {
            return new Layer.Data.Sqls.ScCustoms.ProductClassifyChangeLogs
            {
                ID = entity.ID,
                Model = entity.Model,
                Manufacturer = entity.Manufacturer,
                AdminID = entity.Declarant.ID,
                CreateDate = entity.CreateDate,
                Summary = entity.Summary
            };
        }
    }
}
