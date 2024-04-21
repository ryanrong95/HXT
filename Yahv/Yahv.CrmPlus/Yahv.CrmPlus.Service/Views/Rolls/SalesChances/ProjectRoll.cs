using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Origins;
using Yahv.Underly.Erps;

namespace Yahv.CrmPlus.Service.Views.Rolls
{

    public class ProjectRoll : Origins.ProjectOrigin
    {
        public ProjectRoll()
        {

        }

        protected override IQueryable<Project> GetIQueryable()
        {
            return base.GetIQueryable();
        }
    }

    public class MyProjectRoll : ProjectOrigin
    {

        IErpAdmin Admin;
        public MyProjectRoll() { }
        public MyProjectRoll(IErpAdmin admin)
        {

            this.Admin = admin;
        }
        protected override IQueryable<Project> GetIQueryable()
        {
            if (Admin.IsSuper)
            {
                return base.GetIQueryable();
            }
            else
            {
                return base.GetIQueryable().Where(item => item.OwnerID == Admin.ID);
            }
        }

    }
}
