using System.Collections.Generic;
using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Models.Rolls;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;
using Yahv.Utils.Serializers;

namespace Yahv.Finance.Services.Views.Rolls
{
    /// <summary>
    /// 税率视图 
    /// </summary>
    public class TaxRatesRoll : UniqueView<TaxRate, PvFinanceReponsitory>
    {
        public TaxRatesRoll()
        {
        }

        public TaxRatesRoll(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<TaxRate> GetIQueryable()
        {
            var taxRatesView = new TaxRatesOrigin(this.Reponsitory);
            var adminsView = new AdminsTopView(this.Reponsitory);

            return from tax in taxRatesView
                   join admin in adminsView on tax.CreatorID equals admin.ID into _admin
                   from admin in _admin.DefaultIfEmpty()
                   select new TaxRate()
                   {
                       ID = tax.ID,
                       CreateDate = tax.CreateDate,
                       CreatorID = tax.CreatorID,
                       Name = tax.Name,
                       Code = tax.Code,
                       ModifierID = tax.ModifierID,
                       ModifyDate = tax.ModifyDate,
                       Rate = tax.Rate,
                       CreatorName = admin.RealName,
                       JsonName = tax.JsonName,
                   };
        }
    }
}