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
using Yahv.Underly.Erps;

namespace Yahv.CrmPlus.Service.Views.Rolls
{
    public class RelationsRoll : RelationsOrigin
    {
        string enterpriseid;
        public RelationsRoll()
        {


        }

        public RelationsRoll(string enterpriseid)
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


    public class MyRelationRoll : RelationsOrigin
    {
        IErpAdmin Admin;
        string EnterpriseID;
        public MyRelationRoll(IErpAdmin admin)
        {
            this.Admin = admin;


        }

       

        public MyRelationRoll(IErpAdmin admin, string enterpriseid)
        {
            this.Admin = admin;
            this.EnterpriseID = enterpriseid;

        }

        protected override IQueryable<Models.Origins.Relation> GetIQueryable()
        {
            if (!string.IsNullOrWhiteSpace(this.EnterpriseID))
            {
                return base.GetIQueryable().Where(item => item.ClientID == this.EnterpriseID && item.OwnerID == this.Admin.ID);
            }
            return base.GetIQueryable().Where(x => x.OwnerID == this.Admin.ID);

        }


    }
}
