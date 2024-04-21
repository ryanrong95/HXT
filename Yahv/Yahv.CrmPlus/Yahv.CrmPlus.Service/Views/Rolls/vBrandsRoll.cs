using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Models.Rolls;
using Yahv.Linq;

namespace Yahv.CrmPlus.Service.Views.Rolls
{
    public class vBrandsRoll : Origins.vBrandOrigin
    {
        public vBrandsRoll()
        {
        }

        public vBrandsRoll(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<vBrand> GetIQueryable()
        {
            return base.GetIQueryable();
        }

    }
   
}
