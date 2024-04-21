using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;

namespace Yahv.Finance.Services.Views.Rolls
{
    /// <summary>
    /// 资金调拨 应调视图
    /// </summary>
    public class SelfLeftsRoll : UniqueView<SelfLeft, PvFinanceReponsitory>
    {
        public SelfLeftsRoll()
        {
        }

        public SelfLeftsRoll(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<SelfLeft> GetIQueryable()
        {
            return new SelfLeftsOrigin();
        }
    }
}