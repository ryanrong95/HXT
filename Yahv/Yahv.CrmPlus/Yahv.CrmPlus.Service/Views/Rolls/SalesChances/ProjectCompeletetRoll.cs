using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;

namespace Yahv.CrmPlus.Service.Views.Rolls.SalesChances
{
    public class ProjectCompeletetRoll : Origins.ProjectCompeleteOrigin
    {
        public ProjectCompeletetRoll()
        {

        }

        protected override IQueryable<ProjectCompelete> GetIQueryable()
        {
            return base.GetIQueryable();
        }
    }
}
