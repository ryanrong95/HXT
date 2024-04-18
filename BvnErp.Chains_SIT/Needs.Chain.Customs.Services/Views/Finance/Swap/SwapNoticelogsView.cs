using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views//.Finance.Swap
{
    public class SwapNoticelogsView : UniqueView<Models.SwapNoticeLog, ScCustomsReponsitory>
    {
        public SwapNoticelogsView()
        {
        }

        internal SwapNoticelogsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<SwapNoticeLog> GetIQueryable()
        {
            var result = from swapNoticesLog in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticesLogs>()
                         select new Models.SwapNoticeLog
                         {
                             SwapNoticeID = swapNoticesLog.SwapNoticeID,
                             CreateDate = swapNoticesLog.CreateDate,
                             Summary = swapNoticesLog.Summary,
                         };
            return result;
        }
    }
}
