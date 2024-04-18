using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 入库通知日志扩展方法
    /// </summary>
    public static partial class EntryNoticeLogExtends
    {
        public static Layer.Data.Sqls.ScCustoms.EntryNoticeLogs ToLinq(this Models.EntryNoticeLog entity)
        {
            return new Layer.Data.Sqls.ScCustoms.EntryNoticeLogs
            {
                ID = entity.ID,
                EntryNoticeID = entity.EntryNoticeID,
                AdminID = entity.AdminID,
                EntryNoticeStatus = entity.EntryNoticeStatus,
                CreateDate = entity.CreateDate,
                Summary = entity.Summary
            };
        }
    }
}


