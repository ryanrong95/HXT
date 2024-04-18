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
    public static class ProductClassifyLogExtends
    {
        public static Layer.Data.Sqls.ScCustoms.ProductClassifyLogs ToLinq(this Models.ProductClassifyLog entity)
        {
            return new Layer.Data.Sqls.ScCustoms.ProductClassifyLogs
            {
                ID = entity.ID,
                ClassifyProductID = entity.ClassifyProductID,
                AdminID = entity.Declarant.ID,
                LogType = (int)entity.LogType,
                OperationLog = entity.OperationLog,
                Status = (int)entity.Stauts,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary
            };
        }
    }
}
