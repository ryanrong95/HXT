using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    /// <summary>
    /// 归类结果
    /// </summary>
    public class ClassifiedResult
    {
        /// <summary>
        /// OrderItemID
        /// </summary>
        public string ItemID { get; set; }

        /// <summary>
        /// OrderID或预归类产品ID
        /// </summary>
        public string MainID { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string PartNumber { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 成交单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 海关编码
        /// </summary>
        public string HSCode { get; set; }

        /// <summary>
        /// 报关品名
        /// </summary>
        public string TariffName { get; set; }

        /// <summary>
        /// 税务编码
        /// </summary>
        public string TaxCode { get; set; }

        /// <summary>
        /// 税务名称
        /// </summary>
        public string TaxName { get; set; }

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
        /// 加征税率
        /// </summary>
        public decimal OriginRate { get; set; }

        /// <summary>
        /// 消费税率
        /// </summary>
        public decimal ExciseTaxRate { get; set; }

        /// <summary>
        /// 检验检疫编码
        /// </summary>
        public string CIQCode { get; set; }

        /// <summary>
        /// 申报要素
        /// </summary>
        public string Elements { get; set; }

        /// <summary>
        /// 摘要备注
        /// </summary>
        public string Summary { get; set; }

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
        /// 是否需要原产地证明
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
        /// 是否高价值
        /// </summary>
        public bool IsHighPrice { get; set; }

        /// <summary>
        /// 是否消毒/检疫
        /// </summary>
        public bool IsDisinfected { get; set; }

        /// <summary>
        /// 是否系统判定3C
        /// </summary>
        public bool IsSysCcc { get; set; }

        /// <summary>
        /// 是否系统判定禁运
        /// </summary>
        public bool IsSysEmbargo { get; set; }

        /// <summary>
        /// 是否属于海关验估编码
        /// </summary>
        public bool IsCustomsInspection { get; set; }

        /// <summary>
        /// 归类阶段
        /// </summary>
        public string Step { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string CreatorID { get; set; }
    }

    public class PreProductCategories
    {
        public List<string> MainIDs { get; set; }
    }
}