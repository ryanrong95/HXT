using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Views.Origins
{
    /// <summary>
    /// 收款核销 原始视图
    /// </summary>
    public class PayeeRightsOrigin : UniqueView<PayeeRight, PvFinanceReponsitory>
    {
        internal PayeeRightsOrigin()
        {
        }

        internal PayeeRightsOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<PayeeRight> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.PayeeRights>()
                   select new PayeeRight()
                   {
                       Currency = (Currency)entity.Currency,
                       ID = entity.ID,
                       CreatorID = entity.CreatorID,
                       CreateDate = entity.CreateDate,
                       Price = entity.Price,
                       Price1 = entity.Price1,
                       Currency1 = (Currency)entity.Currency1,
                       ERate1 = entity.ERate1,
                       SenderID = entity.SenderID,
                       Department = entity.Department,
                       PayeeLeftID = entity.PayeeLeftID,
                       AccountCatalogID = entity.AccountCatalogID,
                   };
        }
    }
}