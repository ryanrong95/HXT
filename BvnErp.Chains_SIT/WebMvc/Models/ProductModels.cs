using Needs.Ccs.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMvc.Models
{

    /// <summary>
    /// 自定义产品税号
    /// </summary>
    public class ProductTaxViewModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 品名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string Models { get; set; }

        /// <summary>
        /// 税务编号
        /// </summary>
        public string TaxCode { get; set; }

        /// <summary>
        /// 税务名称
        /// </summary>
        public string TaxName { get; set; }

        /// <summary>
        /// 税务状态
        /// </summary>
        public string TaxStatus { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public string CreateDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }
    }

    /// <summary>
    /// 我的产品
    /// </summary>
    public class MyProductsViewModel
    {
        public string ID { get; set; }
        /// <summary>
        /// 批次
        /// </summary>
        public string Batch { get; set; }

        /// <summary>
        /// 品名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string Models { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
      //  public decimal Price { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }
    }

    /// <summary>
    /// 产品预归类
    /// </summary>
    public class PreProductInfoViewModel
    {
        public string ID { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string Models { get; set; }

        /// <summary>
        /// 货币数据源
        /// </summary>
        public string CurrencyOptions { get; set; }

        /// <summary>
        /// 货币
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string Supplier { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal ? Price { get; set; }

        /// <summary>
        /// 物流号
        /// </summary>
        public string ProductUnionCode { get; set; }
    }

    /// <summary>
    /// 产品预归类明细
    /// </summary>
    public class PreProductViewModel
    {
        public string ID { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 品名
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 海关编码
        /// </summary>
        public string HSCode { get; set; }

        /// <summary>
        /// 关税率
        /// </summary>
        public decimal? TariffRate { get; set; }

        /// <summary>
        /// 增值税率
        /// </summary>
        public decimal? AddedValueRate { get; set; }

        /// <summary>
        /// 税务编码
        /// </summary>
        public string TaxCode { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        public string ClassifyType { get; set; }

        /// <summary>
        /// 商检费
        /// </summary>
        public decimal? InspectionFee { get; set; }

        /// <summary>
        /// 第一单位
        /// </summary>
        public string Unit1 { get; set; }
        
        /// <summary>
        /// 第二单位
        /// </summary>
        public string Unit2 { get; set; }

        /// <summary>
        /// 检验检疫编码
        /// </summary>
        public string CIQCode { get; set; }

        /// <summary>
        /// 申报要素
        /// </summary>
        public string Elements { get; set; }

        /// <summary>
        /// 归类状态
        /// </summary>
        public ClassifyStatus ClassifyStatus { get; set; }

        public Status Status { get; set; }
        public DateTime Createtime { get; set; }
        public DateTime Updatetime { get; set; }
        public string ClassifyFirstOperator { get; set; }
        public string ClassifySecondOperator { get; set; }
        public string ClientID { get; set; }
    }

    /// <summary>
    /// 预归类产品导入数据模型
    /// </summary>
    public class PreProcuctsData
    {
        //产品名称
        public string Name { get; set; }

        //批次号
        public string Batch { get; set; }

        //型号
        public string Model { get; set; }

        //单价
        public string UnitPrice { get; set; }

        //品牌
        public string Manufacturer { get; set; }

        //币种
        public string Currency { get; set; }

        //物流号
        public string UniqueCode { get; set; }
        /// <summary>
        /// 预计到货日期
        /// </summary>
        public string DueDate { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public string Qty { get; set; }
    }
}