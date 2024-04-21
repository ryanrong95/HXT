using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Models.Rolls;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.CrmPlus.Service.Views.Rolls
{
    public class AgentBrandsRoll : UniqueView<nBrand, PvdCrmReponsitory>
    {
        public AgentBrandsRoll()
        {
        }

        public AgentBrandsRoll(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<nBrand> GetIQueryable()
        {
            return new Origins.nBrandOrigin(this.Reponsitory).Where(item => item.Type == nBrandType.Agent);
        }
    }
    public class ProduceBrandsRoll : UniqueView<nBrand, PvdCrmReponsitory>
    {
        public ProduceBrandsRoll()
        {
        }

        public ProduceBrandsRoll(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<nBrand> GetIQueryable()
        {
            return new Origins.nBrandOrigin(this.Reponsitory).Where(item => item.Type == nBrandType.Produce);
        }
    }

}
