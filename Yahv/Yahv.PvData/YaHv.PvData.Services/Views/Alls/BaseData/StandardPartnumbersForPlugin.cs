using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace YaHv.PvData.Services.Views.Alls
{
    public class StandardPartnumbersForPlugin : UniqueView<Models.StandardPartnumbersForPlugin, PvDataReponsitory>
    {
        public StandardPartnumbersForPlugin()
        {
        }

        internal StandardPartnumbersForPlugin(PvDataReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.StandardPartnumbersForPlugin> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvData.StandardPartnumbersForPlugin>()
                   select new Models.StandardPartnumbersForPlugin
                   {
                       ID = entity.ID,
                       PartNumber = entity.PartNumber,
                       Manufacturer = entity.Manufacturer,
                       HSCode = entity.HSCode,
                       Name = entity.Name,
                       LegalUnit1 = entity.LegalUnit1,
                       LegalUnit2 = entity.LegalUnit2,
                       VATRate = entity.VATRate ,
                       TariffRate = entity.TariffRate,
                       ExciseTaxRate = entity.ExciseTaxRate,
                       Elements = entity.Elements,
                       SupervisionRequirements = entity.SupervisionRequirements,
                       CIQC = entity.CIQC,
                       CIQCode = entity.CIQCode,
                       TaxCode = entity.TaxCode,
                       TaxName = entity.TaxName,
                       Ccc = entity.Ccc,
                       Embargo = entity.Embargo,
                       HkControl = entity.HkControl,
                       Coo = entity.Coo,
                       CIQ = entity.CIQ,
                       CIQprice = entity.CIQprice,
                       CreateDate = entity.CreateDate,
                       OrderDate = entity.OrderDate,
                       Summary = entity.Summary,
                       Eccn = entity.Eccn,
                       AddedTariffRate = entity.AddedTariffRate
                   };
        }
    }
}
