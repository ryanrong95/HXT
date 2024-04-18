using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 出库通知扩展方法
    /// </summary>
    public static class ExitNoticeLogExtends
    {
        public static Layer.Data.Sqls.ScCustoms.ExitNoticeLogs ToLinq(this Models.ExitNoticeLog entity)
        {
            return new Layer.Data.Sqls.ScCustoms.ExitNoticeLogs
            {
                ID = entity.ID,
                ExitNoticeID = entity.ExitNoticeID,
                AdminID = entity.Admin.ID,
                OperType = (int)entity.ExitOperType,
                CreateDate = entity.CreateDate,
                Summary = entity.Summary
            };
        }
    }
}
