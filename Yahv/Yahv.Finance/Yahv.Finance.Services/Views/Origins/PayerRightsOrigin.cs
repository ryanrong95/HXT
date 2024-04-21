using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Views.Origins
{
    /// <summary>
    /// 货款实付 原始视图
    /// </summary>
    public class PayerRightsOrigin : UniqueView<PayerRight, PvFinanceReponsitory>
    {
        public PayerRightsOrigin()
        {
        }

        public PayerRightsOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<PayerRight> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.PayerRights>()
                   select new PayerRight()
                   {
                       CreateDate = entity.CreateDate,
                       ID = entity.ID,
                       Currency = (Currency)entity.Currency,
                       CreatorID = entity.CreatorID,
                       Price = entity.Price,
                       Currency1 = (Currency)entity.Currency1,
                       Price1 = entity.Price1,
                       ERate1 = entity.ERate1,
                       FlowID = entity.FlowID,
                       PayerLeftID = entity.PayerLeftID,
                   };
        }
    }
}