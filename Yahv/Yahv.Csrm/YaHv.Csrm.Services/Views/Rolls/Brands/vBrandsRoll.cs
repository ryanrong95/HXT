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
    public class vBrandsRoll : Origins.vBrandsOrigin
    {
        string brandid;
        public vBrandsRoll()
        {

        }
        public vBrandsRoll(string BrandID)
        {
            this.brandid = BrandID;
        }

        protected override IQueryable<vBrand> GetIQueryable()
        {
            if (!string.IsNullOrWhiteSpace(this.brandid))
            {
                return base.GetIQueryable().Where(item => item.BrandID == this.brandid);
            }
            return base.GetIQueryable();
        }

    }
}
