using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Finance.Services.Views.Origins
{
    /// <summary>
    /// 税率 原始视图
    /// </summary>
    public class TaxRatesOrigin : UniqueView<TaxRate, PvFinanceReponsitory>
    {
        internal TaxRatesOrigin()
        {
        }

        internal TaxRatesOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<TaxRate> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.TaxRates>()
                   select new TaxRate()
                   {
                       ID = entity.ID,
                       CreateDate = entity.CreateDate,
                       CreatorID = entity.CreatorID,
                       Name = entity.Name,
                       Code = entity.Code,
                       ModifierID = entity.ModifierID,
                       ModifyDate = entity.ModifyDate,
                       Rate = entity.Rate,
                       JsonName = entity.JsonName,
                   };
        }
    }
}