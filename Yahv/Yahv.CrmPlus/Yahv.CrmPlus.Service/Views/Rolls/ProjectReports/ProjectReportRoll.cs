using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly.Erps;

namespace Yahv.CrmPlus.Service.Views.Rolls.ProjectReports
{
    public class ProjectReportRoll : Origins.ProjectReportOrigin
    {
        public ProjectReportRoll()
        {

        }

        protected override IQueryable<ProjectReport> GetIQueryable()
        {
            return base.GetIQueryable();
        }
    }


    public class MyProjectReportRoll : Origins.ProjectReportOrigin
    {

        IErpAdmin Admin;
        public MyProjectReportRoll() { }
        public MyProjectReportRoll(IErpAdmin admin)
        {
           
            this.Admin = admin;
        }
        protected override IQueryable<ProjectReport> GetIQueryable()
        {
            if (Admin.IsSuper)
            {
                return base.GetIQueryable();
            }
            else {
            return base.GetIQueryable().Where(item => item.PM.ID==Admin.ID);
            }
        }

    }
}
