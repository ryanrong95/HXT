using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace YaHv.PvData.Services.Models
{
    public class StandardPartnumbersForPlugin : IUnique
    {
        #region 属性

        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        public string PartNumber { get; set; }

        public string Manufacturer { get; set; }

        public string HSCode { get; set; }

        public string Name { get; set; }

        public string LegalUnit1 { get; set; }

        public string LegalUnit2 { get; set; }

        public decimal VATRate { get; set; }

        public decimal TariffRate { get; set; }

        public decimal ExciseTaxRate { get; set; }

        public string Elements { get; set; }

        public string SupervisionRequirements { get; set; }

        public string CIQC { get; set; }

        public string CIQCode { get; set; }

        public string TaxCode { get; set; }

        public string TaxName { get; set; }

        public bool Ccc { get; set; }

        public bool Embargo { get; set; }

        public bool HkControl { get; set; }

        public bool Coo { get; set; }

        public bool CIQ { get; set; }

        public decimal CIQprice { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? OrderDate { get; set; }

        public string Summary { get; set; }

        public string Eccn { get; set; }

        public decimal? AddedTariffRate { get; set; }

        #endregion

        #region 扩展属性

        public string SpecialTypes
        {
            get
            {
                StringBuilder specialType = new StringBuilder();

                if (this.Ccc)
                    specialType.Append("3C|");
                if (this.Embargo)
                    specialType.Append("禁运|");
                if (this.HkControl)
                    specialType.Append("香港管制|");
                if (this.Coo)
                    specialType.Append("原产地证明|");
                if (this.CIQ)
                    specialType.Append("商检|");

                if (specialType.Length == 0)
                    return "--";
                else
                    return specialType.ToString().TrimEnd('|');
            }
        }

        #endregion

        /// <summary>
        /// 构造器，内部查询使用
        /// </summary>
        internal StandardPartnumbersForPlugin()
        {
        }

    }


    public class StandardPartnumbersForPluginViewModel
    {
        public string ID { get; set; }
        public string PartNumber { get; set; }
        public string Manufacturer { get; set; }
        public string HSCode { get; set; }
        public string TariffName { get; set; }
        public string TaxCode { get; set; }
        public string TaxName { get; set; }
        public string LegalUnit1 { get; set; }
        public string LegalUnit2 { get; set; }
        public decimal ImportPreferentialTaxRate { get; set; }
        public decimal VATRate { get; set; }

        public decimal ExciseTaxRate { get; set; }

        public string Elements { get; set; }

        public string CIQCode { get; set; }
        public string SpecialTypes { get; set; }
        public bool Ccc { get; set; }

        public bool Embargo { get; set; }
        public bool HkControl { get; set; }
        public bool Coo { get; set; }

        public bool CIQ { get; set; }

        public decimal CIQprice { get; set; }
        public string Summary { get; set; }
        public string OrderDate { get; set; }
        public string StandardTariffRate { get; set; }
    }
}
