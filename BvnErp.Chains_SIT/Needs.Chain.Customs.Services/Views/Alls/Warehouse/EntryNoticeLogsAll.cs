using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Alls
{
    public class EntryNoticeLogsAll : UniqueView<Models.EntryNoticeLog, ScCustomsReponsitory>
    {
        public EntryNoticeLogsAll()
        {
        }

        internal EntryNoticeLogsAll(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.EntryNoticeLog> GetIQueryable()
        {
            var adminsView = new AdminsTopView(this.Reponsitory);
            var logsView = new Origins.EntryNoticeLogsOrigin(this.Reponsitory);

            return from log in logsView
                   join admin in adminsView on log.AdminID equals admin.ID
                   select new Models.EntryNoticeLog
                   {
                       ID = log.ID,
                       EntryNoticeID = log.EntryNoticeID,
                       AdminID = log.AdminID,
                       Admin = admin,
                       EntryNoticeStatus = log.EntryNoticeStatus,
                       CreateDate = log.CreateDate,
                       Summary = log.Summary
                   };
        }
    }
}
