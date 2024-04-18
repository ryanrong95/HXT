using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 特殊类型信息 - 禁运、Ccc
    /// </summary>
    public class XbjInfo : IUnique
    {
        #region 属性

        /// <summary>
        /// 唯一主键
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string PartNumber { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 海关编码
        /// </summary>
        public string HSCode { get; set; }

        /// <summary>
        /// 报关品名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 法一单位
        /// </summary>
        public string LegalUnit1 { get; set; }

        /// <summary>
        /// 法二单位
        /// </summary>
        public string LegalUnit2 { get; set; }

        /// <summary>
        /// 增值税率
        /// </summary>
        public decimal VATRate { get; set; }

        /// <summary>
        /// 进口优惠税率
        /// </summary>
        public decimal ImportPreferentialTaxRate { get; set; }

        /// <summary>
        /// 消费税率
        /// </summary>
        public decimal ExciseTaxRate { get; set; }

        /// <summary>
        /// 申报要素
        /// </summary>
        public string Elements { get; set; }

        /// <summary>
        /// 监管条件
        /// </summary>
        public string SupervisionRequirements { get; set; }

        /// <summary>
        /// 检验检疫条件
        /// </summary>
        public string CIQC { get; set; }

        /// <summary>
        /// 检验检疫编码
        /// </summary>
        public string CIQCode { get; set; }

        /// <summary>
        /// 税务编码
        /// </summary>
        public string TaxCode { get; set; }

        /// <summary>
        /// 税务名称
        /// </summary>
        public string TaxName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// 是否3C
        /// </summary>
        public bool Ccc { get; set; }

        /// <summary>
        /// 是否禁运
        /// </summary>
        public bool Embargo { get; set; }

        /// <summary>
        /// 是否香港管制
        /// </summary>
        public bool HkControl { get; set; }

        /// <summary>
        /// 是否原产地证明
        /// </summary>
        public bool Coo { get; set; }

        /// <summary>
        /// 是否商检
        /// </summary>
        public bool CIQ { get; set; }

        /// <summary>
        /// 商检费
        /// </summary>
        public decimal CIQprice { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        public string Summary { get; set; }

        #endregion

        #region 扩展属性

        public List<Eccn> Eccns { get; set; }

        #endregion
    }
}
