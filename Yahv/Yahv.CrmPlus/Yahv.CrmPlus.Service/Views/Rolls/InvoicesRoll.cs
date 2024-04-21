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
    public class InvoicesRoll : InvoicesOrigin
    {
        string  enterpriseid;
        RelationType relationType;
        public InvoicesRoll()
        {


        }

        public InvoicesRoll(string enterpriseid, RelationType type)
        {
            this.enterpriseid = enterpriseid;
            this.relationType = type;

        }
        protected override IQueryable<Models.Origins.Invoice> GetIQueryable()
        {
            if (!string.IsNullOrWhiteSpace(enterpriseid))
            {
                return base.GetIQueryable().Where(item => item.EnterpriseID == this.enterpriseid && item.RelationType == relationType);
            }
            return base.GetIQueryable();
        }
       
        public IQueryable<Invoice> this[string EnterpriseID, RelationType relationType]
        {
            get
            {
                return
                    this.Where(
                        item => item.EnterpriseID == EnterpriseID && item.RelationType == relationType);
            }
        }
    }
}
