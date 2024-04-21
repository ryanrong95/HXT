using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.CrmPlus.Service.Views.Rolls
{
    public class ConductsRoll : ConductsOrigin
    {
        string enterpriseid;
        public ConductsRoll()
        {


        }

        public ConductsRoll( string  enterpriseid)
        {
            this.enterpriseid = enterpriseid;
        }
        protected override IQueryable<Models.Origins.Conduct> GetIQueryable()
        {
            return from item in base.GetIQueryable()
                   where item.EnterpriseID == this.enterpriseid
                   select item;
        }
    }
}
