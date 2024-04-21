using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;

namespace Yahv.Finance.Services.Views.Rolls
{
    public class AcceptanceLeftsRoll : UniqueView<AcceptanceLeft, PvFinanceReponsitory>
    {
        public AcceptanceLeftsRoll()
        {
        }

        public AcceptanceLeftsRoll(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<AcceptanceLeft> GetIQueryable()
        {
            return new AcceptanceLeftsOrigin(this.Reponsitory);
        }
    }
}