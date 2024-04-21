using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.CrmPlus.Service.Views.Rolls
{
    public class RequiremensRoll:Origins.RequirementsOrigin
    {

        public RequiremensRoll()
        {


        }
        protected override IQueryable<Models.Origins.Requirement> GetIQueryable()
        {
            return base.GetIQueryable();
        }

    }
}
