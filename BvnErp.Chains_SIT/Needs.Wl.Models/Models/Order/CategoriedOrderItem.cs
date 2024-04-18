using Needs.Wl.Models;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 包含归类、税务信息的产品明细
    /// </summary>
    public class CategoriedOrderItem : Needs.Wl.Models.OrderItem
    {
        /// <summary>
        /// 报关品名
        /// 归类后的产品名称，用于申报
        /// </summary>
        public string CategoriedName { get; set; }

        /// <summary>
        /// 产品归类类型
        /// 普通，商检，3C，原产地证明、禁运、检疫等
        /// </summary>
        public Wl.Models.Enums.ItemCategoryType? Type { get; set; }

        /// <summary>
        /// 税务编码
        /// </summary>
        public string TaxCode { get; set; }

        /// <summary>
        /// 税务名称
        /// </summary>
        public string TaxName { get; set; }

        /// <summary>
        /// 海关编码\商品编码
        /// </summary>
        public string HSCode { get; set; }

        /// <summary>
        /// 申报要素
        /// </summary>
        public string Elements { get; set; }

        /// <summary>
        /// 法定第一单位
        /// </summary>
        public string Unit1 { get; set; }

        /// <summary>
        /// 法定第二单位（可空）
        /// </summary>
        public string Unit2 { get; set; }

        /// <summary>
        /// 法定第一数量（必填）
        /// </summary>
        public decimal? Qty1 { get; set; }

        /// <summary>
        /// 法定第二数量（非必填）
        /// </summary>
        public decimal? Qty2 { get; set; }

        /// <summary>
        /// 检验检疫编码
        /// </summary>
        public string CIQCode { get; set; }

        /// <summary>
        /// 进口关税率
        /// </summary>
        public decimal ImportTaxRate { get; set; }

        /// <summary>
        /// 进口关税
        /// </summary>
        public decimal ImportTaxValue { get; set; }

        ///<summary>
        /// 增值税率
        /// </summary>
        public decimal AddedValueRate { get; set; }

        ///<summary>
        /// 增值税
        /// </summary>
        public decimal AddedValue { get; set; }

        /// <summary>
        /// 消费税率
        /// </summary>
        public decimal ConsumeTaxRate { get; set; }

        /// <summary>
        /// 消费税
        /// </summary>
        public decimal ConsumeTaxValue { get; set; }

        /// <summary>
        ///关税增值税总金额
        ///关税+增值税+消费税
        /// </summary>
        public decimal TotalTaxAndAddedValue
        {
            get
            {
                return this.ImportTaxValue + this.AddedValue + this.ConsumeTaxValue;
            }
        }

        /// <summary>
        /// 预归类一人员
        /// </summary>
        public Admin Admin1 { get; set; }

        /// <summary>
        /// 预归类二人员
        /// </summary>
        public Admin Admin2 { get; set; }
    }
}