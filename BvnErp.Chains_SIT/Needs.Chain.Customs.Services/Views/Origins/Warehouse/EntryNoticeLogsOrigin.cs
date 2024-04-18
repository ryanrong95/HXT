using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Origins
{
    internal class EntryNoticeLogsOrigin : UniqueView<Models.EntryNoticeLog, ScCustomsReponsitory>
    {
        internal EntryNoticeLogsOrigin()
        {
        }

        internal EntryNoticeLogsOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.EntryNoticeLog> GetIQueryable()
        {
            return from log in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNoticeLogs>()
                   select new Models.EntryNoticeLog
                   {
                       ID = log.ID,
                       EntryNoticeID = log.EntryNoticeID,
                       AdminID = log.AdminID,
                       EntryNoticeStatus = log.EntryNoticeStatus,
                       CreateDate = log.CreateDate,
                       Summary = log.Summary
                   };
        }
    }
}
