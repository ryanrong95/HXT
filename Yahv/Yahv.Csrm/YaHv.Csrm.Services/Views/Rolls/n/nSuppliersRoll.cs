using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    public class nSuppliersRoll : Origins.nSuppliersOrigin
    {
        string enterpriseid;
        public nSuppliersRoll(string EnterpriseID)
        {
            this.enterpriseid = EnterpriseID;
        }
        protected override IQueryable<nSupplier> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.EnterpriseID == this.enterpriseid);
        }

    }
}
