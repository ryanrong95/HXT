using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;

namespace Yahv.Finance.Services.Views.Rolls
{
    /// <summary>
    /// 收款认领 视图
    /// </summary>
    public class AccountWorksRoll : UniqueView<AccountWork, PvFinanceReponsitory>
    {
        public AccountWorksRoll()
        {
        }

        public AccountWorksRoll(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<AccountWork> GetIQueryable()
        {
            return new AccountWorksOrigin(this.Reponsitory);
        }
    }
}