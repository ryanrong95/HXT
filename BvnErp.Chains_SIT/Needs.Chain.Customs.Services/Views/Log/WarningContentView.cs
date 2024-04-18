using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class WarningContentView : UniqueView<Models.WarningContext, ScCustomsReponsitory>
    {
        public WarningContentView()
        {
        }

        internal WarningContentView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.WarningContext> GetIQueryable()
        {           
            var result = from notice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Logs_Notices>()
                         join adminContext in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PvbErmAdminContactInfo>()
                         on notice.AdminID equals adminContext.ID into g
                         from adminContext in g.DefaultIfEmpty()
                         where notice.Readed == false && notice.Type != null
                         select new Models.WarningContext
                         {
                             ID = notice.ID.ToString(),
                             AdminID = notice.AdminID,
                             Title = notice.Title,
                             Context = notice.Context,
                             NoticeType = (Enums.SendNoticeType)notice.Type,
                             MainID = notice.MainID,
                             Email = adminContext.Email,
                             Moblie = adminContext.Mobile
                         };

            return result;
        }
    }
}
