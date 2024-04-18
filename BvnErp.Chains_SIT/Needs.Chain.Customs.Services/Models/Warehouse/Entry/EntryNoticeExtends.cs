using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public static partial class EntryNoticeExtends
    {
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="summary"></param>
        public static void Log(this Models.EntryNotice entity, Admin Operator, string summary)
        {
            EntryNoticeLog log = new EntryNoticeLog();
            log.EntryNoticeID = entity.ID;
            log.AdminID = Operator.ID;
            log.EntryNoticeStatus =(int)entity.EntryNoticeStatus;
            log.Summary = summary;
            log.Enter();
        }
    }
}
