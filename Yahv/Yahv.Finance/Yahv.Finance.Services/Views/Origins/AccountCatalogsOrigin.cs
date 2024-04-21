using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Underly.Enums;

namespace Yahv.Finance.Services.Views.Origins
{
    /// <summary>
    /// 收付款类型 原始视图
    /// </summary>
    public class AccountCatalogsOrigin : UniqueView<AccountCatalog, PvFinanceReponsitory>
    {
        internal AccountCatalogsOrigin() { }

        internal AccountCatalogsOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<AccountCatalog> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.AccountCatalogs>()
                   select new AccountCatalog()
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       ModifierID = entity.ModifierID,
                       ModifyDate = entity.ModifyDate,
                       Status = (GeneralStatus)entity.Status,
                       CreateDate = entity.CreateDate,
                       CreatorID = entity.CreatorID,
                       FatherID = entity.FatherID,
                   };
        }
    }
}