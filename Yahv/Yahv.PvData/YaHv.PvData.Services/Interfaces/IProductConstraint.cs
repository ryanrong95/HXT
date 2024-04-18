using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaHv.PvData.Services.Interfaces
{
    /// <summary>
    /// 约束ID生成规则
    /// </summary>
    public interface IProductConstraint
    {
        #region 属性

        /// <summary>
        /// 产品ID
        /// </summary>
        string ID { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        string PartNumber { get; set; }

        /// <summary>
        /// 品牌/制造商
        /// </summary>
        string Manufacturer { get; set; }

        /// <summary>
        /// 海关编码
        /// </summary>
        string HSCode { get; set; }

        /// <summary>
        /// 报关品名
        /// </summary>
        string TariffName { get; set; }

        /// <summary>
        /// 税务编码
        /// </summary>
        string TaxCode { get; set; }

        /// <summary>
        /// 税务名称
        /// </summary>
        string TaxName { get; set; }

        /// <summary>
        /// 法定第一单位
        /// </summary>
        string LegalUnit1 { get; set; }

        /// <summary>
        /// 法定第二单位
        /// </summary>
        string LegalUnit2 { get; set; }

        /// <summary>
        /// 增值税率
        /// </summary>
        decimal VATRate { get; set; }

        /// <summary>
        /// 进口优惠税率
        /// </summary>
        decimal ImportPreferentialTaxRate { get; set; }

        /// <summary>
        /// 消费税率
        /// </summary>
        decimal ExciseTaxRate { get; set; }

        /// <summary>
        /// 检验检疫编码
        /// </summary>
        string CIQCode { get; set; }

        /// <summary>
        /// 申报要素
        /// </summary>
        string Elements { get; set; }

        #endregion

        /// <summary>
        /// 监管条件
        /// </summary>
        string SupervisionRequirements { get; set; }

        /// <summary>
        /// 建议编码 
        /// </summary>
        /// <remarks>
        /// 999,105等
        /// </remarks>
        string CIQC { get; set; }
    }
}
