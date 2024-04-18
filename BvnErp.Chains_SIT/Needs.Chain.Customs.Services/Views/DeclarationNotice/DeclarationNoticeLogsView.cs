using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class DeclarationNoticeLogsView : UniqueView<Models.DeclarationNoticeLog, ScCustomsReponsitory>
    {
        protected override IQueryable<DeclarationNoticeLog> GetIQueryable()
        {
            var adminsView = new AdminsTopView(this.Reponsitory);

            return from noticeLog in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarationNoticeLogs>()
                   join admin in adminsView on noticeLog.AdminID equals admin.ID
                   select new Models.DeclarationNoticeLog
                   {
                       ID = noticeLog.ID,
                       DeclarationNoticeID = noticeLog.DeclarationNoticeID,
                       DeclarationNoticeItemID = noticeLog.DeclarationNoticeItemID,
                       Admin = admin,
                       CreateDate = noticeLog.CreateDate,
                       Summary = noticeLog.Summary
                   };
        }
    }
}
