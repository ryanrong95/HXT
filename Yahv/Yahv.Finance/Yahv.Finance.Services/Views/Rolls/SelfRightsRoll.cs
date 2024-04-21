using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;

namespace Yahv.Finance.Services.Views.Rolls
{
    /// <summary>
    /// 资金调拨 实调
    /// </summary>
    public class SelfRightsRoll : UniqueView<SelfRight, PvFinanceReponsitory>
    {
        public SelfRightsRoll()
        {
        }

        public SelfRightsRoll(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<SelfRight> GetIQueryable()
        {
            return new SelfRightsOrigin(this.Reponsitory);
        }
    }
}