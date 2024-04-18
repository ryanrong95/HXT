using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class InsideOrderItem
    {
        /// <summary>
        /// 序号
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string PreProductID { get; set; }
        /// <summary>
        /// 海关编码
        /// </summary>
        public string CustomsCode { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 规格型号
        /// </summary>
        public string Elements { get; set; }
        /// <summary>
        /// 成交数量
        /// </summary>
        public decimal Quantity { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// 法定数量
        /// </summary>
        public string FirstLegalUnit { get; set; }
        /// <summary>
        /// 第二数量
        /// </summary>
        public string SecondLegalUnit { get; set; }
        /// <summary>
        /// 产地
        /// </summary>
        public string PlaceOfProduction { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal TotalDeclarePrice { get; set; }
        /// <summary>
        /// 币制
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// 临时箱号
        /// </summary>
        public string PackNo { get; set; }
        /// <summary>
        /// 净重
        /// </summary>
        public decimal NetWeight { get; set; }
        /// <summary>
        /// 毛重
        /// </summary>
        public decimal GrossWeight { get; set; }
        /// <summary>
        /// 规格型号
        /// </summary>
        public string Model { get; set; }

        public string OriginalQuantity { get; set; }
        /// <summary>
        /// 品名
        /// </summary>
        public string TaxName { get; set; }
        /// <summary>
        /// 型号信息分类值
        /// </summary>
        public string TaxCode { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }
        /// <summary>
        /// 关税
        /// </summary>
        public decimal TariffRate { get; set; }
        /// <summary>
        /// 下单公司
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 付款公司
        /// </summary>
        public string SupplierName { get; set; }
        /// <summary>
        /// 付款公司ID
        /// </summary>
        public string SupplierID { get; set; }
        /// <summary>
        /// 是否商检
        /// </summary>
        public bool IsInspection { get; set; }
        /// <summary>
        /// 商检费
        /// </summary>
        public decimal InspFee { get; set; }
        /// <summary>
        /// 是否3C
        /// </summary>
        public bool IsCCC { get; set; }
        /// <summary>
        /// 是否禁运
        /// </summary>
        public bool IsSysForbid { get; set; }
        /// <summary>
        /// 检验检疫码
        /// </summary>
        public string CIQCode { get; set; }
        /// <summary>
        /// 箱子重量
        /// </summary>
        public decimal CaseWeight { get; set; }
        /// <summary>
        /// 报关公司
        /// </summary>
        public string DeclareCompany { get; set; }

        /// <summary>
        /// 增值税率，导一期历史数据的时候用到，用完可以删掉
        /// </summary>
        public decimal VauleAddedRate { get; set; }
    }
}
