using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 暂存日志扩展方法
    /// </summary>
    public static class TemporaryLogExtends
    {

        public static Layer.Data.Sqls.ScCustoms.TemporaryLogs ToLinq(this Models.TemporaryLog entity)
        {
            return new Layer.Data.Sqls.ScCustoms.TemporaryLogs
            {
                ID = entity.ID,
                AdminID = entity.Admin.ID,
                TemporaryID = entity.TemporaryID,
                OperType = (int)entity.TemporaryStatus,
                CreateDate = entity.CreateDate,
                Summary = entity.Summary
            };
        }

        
        /// <summary>
        /// 写入暂存日志
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="Operator"></param>
        /// <param name="summary"></param>
        public static void Log(this Models.Temporary entity, Admin Operator, string summary)
        {
            TemporaryLog log = new TemporaryLog();
            log.TemporaryID = entity.ID;
            log.Admin = Operator;
            log.TemporaryStatus = entity.TemporaryStatus;
            log.Summary = summary;
            log.Enter();
        }

    }
}
