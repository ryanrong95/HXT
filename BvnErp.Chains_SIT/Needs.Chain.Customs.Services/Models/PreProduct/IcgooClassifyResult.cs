using Needs.Ccs.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class IcgooClassifyResult
    {
        /// <summary>
        /// 型号
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// 品名
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { get; set; }
        /// <summary>
        /// HSCode
        /// </summary>
        public string HSCode { get; set; }
        /// <summary>
        /// 检疫编码
        /// </summary>
        public string CIQCode { get; set; }
        /// <summary>
        /// 申报要素
        /// </summary>
        public string Elements { get; set; }
        /// <summary>
        /// 产地
        /// </summary>
        public string Origin { get; set; }
        /// <summary>
        /// 关税率
        /// </summary>
        public decimal TariffRate { get; set; }
        /// <summary>
        /// 实缴关税率
        /// </summary>
        public decimal ReceiptRate { get; set; }
        /// <summary>
        /// TaxName
        /// </summary>
        public string TaxName { get; set; }
        /// <summary>
        /// TaxCode
        /// </summary>
        public string TaxCode { get; set; }
        /// <summary>
        /// 归类日期
        /// </summary>
        public DateTime UpdateDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ItemCategoryType Type { get; set; }
        /// <summary>
        /// 法一单位
        /// </summary>
        public string Unit1 { get; set; }
        /// <summary>
        /// 法二单位
        /// </summary>
        public string Unit2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Eccn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProductUniqueCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; }

        public bool Inspection
        {
            get
            {
                return ((this.Type & ItemCategoryType.Inspection) > 0);
            }
        }
        public bool CCC
        {
            get
            {
                return ((this.Type & ItemCategoryType.CCC) > 0);
            }
        }
        public bool OriginProof
        {
            get
            {
                return ((this.Type & ItemCategoryType.OriginProof) > 0);
            }
        }
        public bool HighValue
        {
            get
            {
                return ((this.Type & ItemCategoryType.HighValue) > 0);
            }
        }
        public bool Forbidden
        {
            get
            {
                return ((this.Type & ItemCategoryType.Forbid) > 0);
            }
        }
        public bool HKForbidden
        {
            get
            {
                return ((this.Type & ItemCategoryType.HKForbid) > 0);
            }
        }
    }
}
