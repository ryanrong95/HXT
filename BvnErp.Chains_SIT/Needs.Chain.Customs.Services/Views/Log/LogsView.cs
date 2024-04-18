using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class LogsView : UniqueView<Models.Logs, ScCustomsReponsitory>
    {
        public LogsView()
        {
        }

        internal LogsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Logs> GetIQueryable()
        {           

            var result = from notice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Logs>()
                        join admin in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView2>() on notice.AdminID equals admin.OriginID into adminview
                        from admin in adminview.DefaultIfEmpty()
                         select new Models.Logs
                         {
                             ID = notice.ID.ToString(),
                             Name = notice.Name,
                             MainID = notice.MainID,
                             AdminID = notice.AdminID,
                             AdminName = admin == null ? "" : admin.RealName,
                             Summary = notice.Summary,
                             Json = notice.Json,
                             CreateDate = notice.CreateDate
                         };

            return result;
        }
    }
}
