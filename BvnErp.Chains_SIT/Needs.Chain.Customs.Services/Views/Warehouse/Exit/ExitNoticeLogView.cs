using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 出库通知日志视图
    /// </summary>
    public class ExitNoticeLogView : UniqueView<Models.ExitNoticeLog, ScCustomsReponsitory>
    {
        public ExitNoticeLogView()
        {
        }

        internal ExitNoticeLogView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ExitNoticeLog> GetIQueryable()
        {
            var adminView = new AdminsTopView(this.Reponsitory);

            return from Log in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNoticeLogs>()
                   join admin in adminView on Log.AdminID equals admin.ID
                   select new Models.ExitNoticeLog
                   {
                       ID = Log.ID,
                       ExitNoticeID = Log.ExitNoticeID,
                       Admin = admin,
                       ExitOperType = (Enums.ExitOperType)Log.OperType,
                       CreateDate = Log.CreateDate,
                       Summary = Log.Summary
                   };
        }
    }
}
