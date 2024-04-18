using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PvWsOrder.Services.Models
{
    /// <summary>
    /// 海关税则归类信息
    /// </summary>

    public class ClassifiedPartNumber : IUnique
    {
        #region 属性

        public string ID { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string PartNumber { get; set; }

        /// <summary>
        /// 制造商
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 海关编码
        /// </summary>
        public string HSCode { get; set; }

        /// <summary>
        /// 报关品名
        /// </summary>
        public string TariffName { get; set; }

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
        /// 检验检疫类别
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
        /// 用于排序的时间字段
        /// </summary>
        public DateTime OrderDate { get; set; }

        #endregion

        /// <summary>
        /// 构造器，内部查询使用
        /// </summary>
        internal ClassifiedPartNumber()
        {
        }
    }
}
