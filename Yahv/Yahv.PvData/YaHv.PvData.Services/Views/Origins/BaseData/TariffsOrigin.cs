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
    /// 海关税则
    /// </summary>
    internal class TariffsOrigin : UniqueView<Models.Tariff, PvDataReponsitory>
    {
        internal TariffsOrigin()
        {
        }

        internal TariffsOrigin(PvDataReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Tariff> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvData.Tariffs>()
                   select new Models.Tariff
                   {
                       ID = entity.ID,
                       HSCode = entity.HSCode,
                       Name = entity.Name,
                       LegalUnit1 = entity.LegalUnit1,
                       LegalUnit2 = entity.LegalUnit2,
                       VATRate = entity.VATRate,
                       ImportPreferentialTaxRate = entity.ImportPreferentialTaxRate,
                       ImportControlTaxRate = entity.ImportControlTaxRate,
                       ImportGeneralTaxRate = entity.ImportGeneralTaxRate,
                       ExciseTaxRate = entity.ExciseTaxRate,
                       DeclareElements = entity.DeclareElements,
                       SupervisionRequirements = entity.SupervisionRequirements,
                       CIQC = entity.CIQC,
                       CIQCode = entity.CIQCode,
                       Status = (GeneralStatus)entity.Status,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate
                   };
        }
    }
}
