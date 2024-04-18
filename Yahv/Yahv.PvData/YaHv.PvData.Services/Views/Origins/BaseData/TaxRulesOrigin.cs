using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace YaHv.PvData.Services.Views.Origins
{
    /// <summary>
    /// 税务信息
    /// </summary>
    internal class TaxRulesOrigin : UniqueView<Models.TaxRule, PvDataReponsitory>
    {
        internal TaxRulesOrigin()
        {
        }

        internal TaxRulesOrigin(PvDataReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.TaxRule> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvData.TaxRules>()
                   select new Models.TaxRule
                   {
                       ID = entity.ID,
                       TaxCode = entity.TaxCode,
                       TaxFirstCategory = entity.TaxFirstCategory,
                       TaxSecondCategory = entity.TaxSecondCategory,
                       TaxThirdCategory = entity.TaxThirdCategory,
                       Status = (GeneralStatus)entity.Status,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate
                   };
        }
    }
}
