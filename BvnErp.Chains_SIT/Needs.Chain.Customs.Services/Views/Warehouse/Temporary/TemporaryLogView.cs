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
    /// 暂存日志View
    /// </summary>
    public class TemporaryLogView : UniqueView<Models.TemporaryLog, ScCustomsReponsitory>
    {
        public TemporaryLogView()
        {
        }

        internal TemporaryLogView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.TemporaryLog> GetIQueryable()
        {
            var adminView = new Views.AdminsTopView(this.Reponsitory);

            return from temporaryLog in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.TemporaryLogs>()
                   join admin in adminView on temporaryLog.AdminID equals admin.ID
                   select new Models.TemporaryLog
                   {
                       ID = temporaryLog.ID,
                       TemporaryID = temporaryLog.TemporaryID,
                       Admin = admin,
                       TemporaryStatus = (Enums.TemporaryStatus)temporaryLog.OperType,
                       CreateDate = temporaryLog.CreateDate,
                       Summary = temporaryLog.Summary,
                   };
        }
    }
}
