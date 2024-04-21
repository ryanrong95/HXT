using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Views.Origins
{
    /// <summary>
    /// 实际调拨 原始视图
    /// </summary>
    public class SelfRightsOrigin : UniqueView<SelfRight, PvFinanceReponsitory>
    {
        internal SelfRightsOrigin()
        {
        }

        internal SelfRightsOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<SelfRight> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.SelfRights>()
                   select new SelfRight()
                   {
                       CreateDate = entity.CreateDate,
                       ID = entity.ID,
                       CreatorID = entity.CreatorID,
                       FlowID = entity.FlowID,
                       OriginPrice = entity.OriginPrice,
                       TargetCurrency = (Currency)entity.TargetCurrency,
                       OriginPrice1 = entity.OriginPrice1,
                       TargetERate1 = entity.TargetERate1,
                       OriginERate1 = entity.OriginERate1,
                       TargetPrice1 = entity.TargetPrice1,
                       OriginCurrency = (Currency)entity.OriginCurrency,
                       Rate = entity.Rate,
                       TargetCurrency1 = (Currency)entity.TargetCurrency1,
                       OriginCurrency1 = (Currency)entity.OriginCurrency1,
                       TargetPrice = entity.TargetPrice,
                       SelfLeftID = entity.SelfLeftID,
                   };

        }
    }
}