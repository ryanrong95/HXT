using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;

namespace Yahv.Finance.Services.Views.Rolls
{
    public class AcceptanceRightsRoll : UniqueView<AcceptanceRight, PvFinanceReponsitory>
    {
        public AcceptanceRightsRoll()
        {
        }

        public AcceptanceRightsRoll(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<AcceptanceRight> GetIQueryable()
        {
            return new AcceptanceRightsOrigin(this.Reponsitory);
        }
    }
}