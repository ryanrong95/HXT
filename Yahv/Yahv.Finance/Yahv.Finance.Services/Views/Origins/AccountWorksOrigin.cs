using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Finance.Services.Views.Origins
{
    /// <summary>
    /// 认领原始视图
    /// </summary>
    public class AccountWorksOrigin : UniqueView<AccountWork, PvFinanceReponsitory>
    {
        public AccountWorksOrigin()
        {
        }

        public AccountWorksOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<AccountWork> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.AccountWorks>()
                   select new AccountWork()
                   {
                       ClaimantID = entity.ClaimantID,
                       Company = entity.Company,
                       Conduct = entity.Conduct,
                       CreateDate = entity.CreateDate,
                       ID = entity.ID,
                       ModifyDate = entity.ModifyDate,
                       PayeeLeftID = entity.PayeeLeftID,
                   };
        }
    }
}