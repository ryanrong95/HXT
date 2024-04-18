using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using YaHv.PvData.Services.Models;

namespace YaHv.PvData.Services.Views.Alls
{
    /// <summary>
    /// 归类历史数据的视图
    /// </summary>
    public class ClassifiedHistoriesAll : QueryView<Models.ClassifiedHistory, PvDataReponsitory>
    {
        protected override IQueryable<ClassifiedHistory> GetIQueryable()
        {
            var linq = from ch in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvData.ClassifiedHistoriesTopView>()
                       join tariff in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvData.Tariffs>() on ch.HSCode equals tariff.HSCode
                       orderby ch.PartNumber, ch.Manufacturer
                       select new ClassifiedHistory
                       {
                           PartNumber = ch.PartNumber,
                           Manufacturer = ch.Manufacturer,
                           HSCode = ch.HSCode,
                           TariffName = ch.Name,
                           TaxCode = ch.TaxCode,
                           TaxName = ch.TaxName,
                           LegalUnit1 = tariff.LegalUnit1,
                           LegalUnit2 = tariff.LegalUnit2,
                           VATRate = tariff.VATRate / 100,
                           ImportPreferentialTaxRate = tariff.ImportPreferentialTaxRate / 100,
                           ImportControlTaxRate = tariff.ImportControlTaxRate == null ? null : (decimal?)tariff.ImportControlTaxRate.Value / 100,
                           ExciseTaxRate = tariff.ExciseTaxRate.GetValueOrDefault() / 100,
                           CIQCode = ch.CIQCode,
                           Elements = ch.Elements,

                           SupervisionRequirements = ch.SupervisionRequirements,
                           CIQC = ch.CIQC,
                           OrderDate = ch.OrderDate,

                           ID = ch.ID,
                           Ccc = ch.Ccc,
                           Embargo = ch.Embargo,
                           HkControl = ch.HkControl,
                           Coo = ch.Coo,
                           CIQ = ch.CIQ,
                           CIQprice = ch.CIQprice,
                           Summary = ch.Summary,
                       };

            return linq;
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="partNumber">型号</param>
        /// <param name="manufacturer">品牌</param>
        /// <returns></returns>
        public ClassifiedHistory this[string partNumber, string manufacturer]
        {
            get
            {
                return this.FirstOrDefault(item => item.PartNumber == partNumber && item.Manufacturer == manufacturer);
            }
        }
    }
}
