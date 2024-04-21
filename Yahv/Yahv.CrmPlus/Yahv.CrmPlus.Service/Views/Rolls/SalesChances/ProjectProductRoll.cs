using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;

namespace Yahv.CrmPlus.Service.Views.Rolls.SalesChances
{
    public class ProjectProductRoll : Origins.ProjectProductOrigin
    {
        public ProjectProductRoll()
        {

        }

        protected override IQueryable<ProjectProduct> GetIQueryable()
        {
            return base.GetIQueryable();
        }
    }


    public class ProjectProductExtendRoll : Origins.ProjectProductExtendOrigin
    {
        public ProjectProductExtendRoll()
        {

        }

        protected override IQueryable<ProjectProduct> GetIQueryable()
        {
            return base.GetIQueryable();
        }
    }
}
