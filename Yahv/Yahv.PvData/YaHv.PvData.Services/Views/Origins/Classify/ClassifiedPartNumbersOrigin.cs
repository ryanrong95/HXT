using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace YaHv.PvData.Services.Views.Origins
{
    /// <summary>
    /// 海关税则归类信息
    /// </summary>
    internal class ClassifiedPartNumbersOrigin : UniqueView<Models.ClassifiedPartNumber, PvDataReponsitory>
    {
        internal ClassifiedPartNumbersOrigin()
        {
        }

        internal ClassifiedPartNumbersOrigin(PvDataReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ClassifiedPartNumber> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvData.ClassifiedPartNumbers>()
                   select new Models.ClassifiedPartNumber
                   {
                       ID = entity.ID,
                       PartNumber = entity.PartNumber,
                       Manufacturer = entity.Manufacturer,
                       HSCode = entity.HSCode,
                       TariffName = entity.Name,
                       LegalUnit1 = entity.LegalUnit1,
                       LegalUnit2 = entity.LegalUnit2,
                       VATRate = entity.VATRate,
                       ImportPreferentialTaxRate = entity.ImportPreferentialTaxRate,
                       ExciseTaxRate = entity.ExciseTaxRate,
                       Elements = entity.Elements,
                       SupervisionRequirements = entity.SupervisionRequirements,
                       CIQC = entity.CIQC,
                       CIQCode = entity.CIQCode,
                       TaxCode = entity.TaxCode,
                       TaxName = entity.TaxName,
                       CreateDate = entity.CreateDate,
                       OrderDate = entity.OrderDate
                   };
        }
    }
}
