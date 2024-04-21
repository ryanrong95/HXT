using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.CrmPlus.Service.Views.Rolls
{
    public class nFixedBrandsRoll : Origins.nFixedBrandsOrigin
    {
        string enterpriseid;
        public nFixedBrandsRoll(string EnterpriseID)
        {
            this.enterpriseid = EnterpriseID;
        }
        public nFixedBrandsRoll()
        {

        }
        protected override IQueryable<Models.Origins.nFixedBrand> GetIQueryable()
        {
            if (!string.IsNullOrWhiteSpace(this.enterpriseid))
            {
                return base.GetIQueryable().Where(item => item.EnterpriseID == this.enterpriseid);
            }
            return base.GetIQueryable();
        }
    }
}
