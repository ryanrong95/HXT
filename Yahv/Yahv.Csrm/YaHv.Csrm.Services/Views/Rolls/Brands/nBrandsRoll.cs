using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    public class nBrandsRoll : Origins.nBrandsOrigin
    {
        public nBrandsRoll()
        {

        }
        string SupplierID;
        public nBrandsRoll(string supplierid)
        {
            this.SupplierID = supplierid;
        }
        protected override IQueryable<nBrand> GetIQueryable()
        {
            if (!string.IsNullOrWhiteSpace(this.SupplierID))
            {
                return base.GetIQueryable().Where(item => item.EnterpriseID == this.SupplierID);
            }
            return base.GetIQueryable();
        }

    }
}
