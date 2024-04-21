using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Origins;
using Yahv.Linq;

namespace Yahv.CrmPlus.Service.Views.Rolls
{
    public class CompaniesRoll : CompaniesOrigin
    {
        public CompaniesRoll()
        {


        }
        protected override IQueryable<Models.Origins.Company> GetIQueryable()
        {
            return base.GetIQueryable(); 
        }
    }
}
