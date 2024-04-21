using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Views.Origins;

namespace Yahv.CrmPlus.Service.Views.Rolls
{
    public class OwnerRoll : OwnerOrigin
    {
        string enterpriseid;
           
        public OwnerRoll()
        {


        }

        public OwnerRoll(string enterpriseid)
        {
            this.enterpriseid = enterpriseid;

        }
        protected override IQueryable<Models.Origins.Relation> GetIQueryable()
        {
            if (!string.IsNullOrWhiteSpace(this.enterpriseid))
            {
                return base.GetIQueryable().Where(item => item.ClientID == this.enterpriseid);
            }
            return base.GetIQueryable();

        }

    }
}
