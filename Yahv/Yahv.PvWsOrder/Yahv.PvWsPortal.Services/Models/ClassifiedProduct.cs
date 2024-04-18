using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PvWsPortal.Services.Models
{
    public class ClassifiedProduct : IUnique
    {
        #region 属性
        /// <summary>
        /// 主键ID
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
        /// 商品编码
        /// </summary>
        public string HSCode { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 法定第一单位
        /// </summary>
        public string LegalUnit1 { get; set; }

        /// <summary>
        /// 法定第二单位
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
        /// 进口普通税率
        /// </summary>
        public decimal ImportGeneralTaxRate { get; set; }

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
        /// 中国检验检验类别
        /// </summary>
        public string CIQC { get; set; }

        /// <summary>
        /// 中国检验检疫代码
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
        /// 是否3c
        /// </summary>
        public bool Ccc { get; set; }

        /// <summary>
        /// 商检
        /// </summary>
        public bool CIQ { get; set; }

        /// <summary>
        /// 禁运
        /// </summary>
        public bool Embargo { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 专用于排序的时间字段
        /// </summary>
        public DateTime OrderDate { get; set; }
        #endregion

        #region 拓展属性
        /// <summary>
        /// 申报元素
        /// </summary>
        public Dictionary<string, string> ElementsExtend { get; set; }
        #endregion
    }
}
