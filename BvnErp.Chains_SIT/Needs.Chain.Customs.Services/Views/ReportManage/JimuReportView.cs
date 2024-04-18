using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class JimuReportView : QueryView<Models.jumuReport, ScCustomsReponsitory>
    {
        public JimuReportView()
        {
        }

        protected JimuReportView(ScCustomsReponsitory reponsitory, IQueryable<Models.jumuReport> iQueryable) : base(reponsitory, iQueryable)
        {
        }
        protected override IQueryable<Models.jumuReport> GetIQueryable()
        {
            var iQuery = from jimuReport in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.JimuReport>()
                         join adminRole in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminRoles>() on jimuReport.RoleID equals adminRole.RoleID into adminRoles
                         from role in adminRoles.DefaultIfEmpty()
                         where jimuReport.Status == (int)Enums.Status.Normal
                         select new Models.jumuReport
                         {
                             ID = jimuReport.ID,
                             ReportName = jimuReport.ReportName,
                             ReportUrl = jimuReport.ReportUrl,
                             RoleID = jimuReport.RoleID,
                             AdminID = role.AdminID,
                             Status = (Enums.Status)jimuReport.Status,
                             CreateDate = jimuReport.CreateDate,
                             UpdateDate = jimuReport.UpdateDate,
                             Summary = jimuReport.Summary,
                         };
            return iQuery;
        }
    }
}
