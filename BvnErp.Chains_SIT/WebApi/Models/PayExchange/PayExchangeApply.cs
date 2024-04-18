using Needs.Ccs.Services.Enums;
using System;
using System.Collections.Generic;

namespace WebApi.Models
{


    /// <summary>
    /// 付汇申请
    /// </summary>
    public class PayExchangeRequest
    {

        #region 属性

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 会员
        /// 客户提交的付汇申请
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// 供应商英文名称
        /// </summary>
        public string SupplierEnglishName { get; set; }

        /// <summary>
        /// 供应商地址
        /// </summary>
        public string SupplierAddress { get; set; }

        /// <summary>
        /// 银行账户
        /// </summary>
        public string BankAccount { get; set; }

        /// <summary>
        /// 银行地址
        /// </summary>
        public string BankAddress { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 银行国际代码
        /// </summary>
        public string SwiftCode { get; set; }

        /// <summary>
        /// 使用的汇率类型
        /// </summary>
        public ExchangeRateType ExchangeRateType { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 汇率
        /// </summary>
        public decimal ExchangeRate { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        public PaymentType PaymentType { get; set; }

        /// <summary>
        /// 期望付款日期
        /// </summary>
        public DateTime? ExpectPayDate { get; set; }

        /// <summary>
        /// 结算截止日期
        /// </summary>
        public DateTime SettlemenDate { get; set; }

        /// <summary>
        /// 其它资料
        /// </summary>
        public string OtherInfo { get; set; }

      /// <summary>
      /// 订单项
      /// </summary>
        public List<UnPayExchangeOrder> UnPayExchangeOrders { get; set; }

        ///// <summary>
        ///// PI文件列表
        ///// </summary>
        //public List<PayExchangeApplyFile> PayExchangeApplyFiles { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }


        /// <summary>
        /// 美国付汇账号
        /// </summary>
        public string ABA { get; set; }
        /// <summary>
        /// 欧美付汇账号
        /// </summary>
        public string IBAN { get; set; }

        /// <summary>
        /// 是否垫块(0是垫款，1是不垫款)
        /// </summary>
        public int IsAdvanceMoney { get; set; }


        /// <summary>
        /// 代付款手续费类型
        /// </summary>
        public string HandlingFeePayerType { get; set; }

        /// <summary>
        /// 手续费（美元）
        /// </summary>
        public decimal? HandlingFee { get; set; }

        /// <summary>
        /// 美元实时汇率
        /// </summary>
        public decimal? USDRate { get; set; }

        #endregion




    }
    /// <summary>
    /// 合同附件
    /// </summary>
    public class PayExchangeApplyFile
    {

        public string FileName { get; set; }

        public string FileFormat { get; set; }

        public string Url { get; set; }

        public FileType FileType { get; set; }

    }
    /// <summary>
    /// 
    /// </summary>
    public class UnPayExchangeOrder
    {
        ///// <summary>
        ///// 汇率类型
        ///// </summary>
        //public ExchangeRateType ExchangeRateType { get; set; }
        ///// <summary>
        /////汇率
        ///// </summary>

        //public decimal ExchangeRate { get; set; }
        ///// <summary>
        ///// 币种
        ///// </summary>
        //public string Currency { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //public string ClientID { get; set; }

        public string OrderID { get; set; }
        

        /// <summary>
        /// 报关总价
        /// </summary>
        public decimal DeclarePrice { get; set; }
        /// <summary>
        /// 本地付汇金额
        /// </summary>
        public decimal CurrentPaidAmount { get; set; }

        /// <summary>
        /// 已申请付汇总价
        /// </summary>
        public decimal PaidExchangeAmount { get; set; }


        /// <summary>
        /// 可申请付汇金额
        /// </summary>
        //  public decimal PaidAmount { get; set; }

        /// <summary>
        /// 申请状态
        /// </summary>
        //public ApplyItemStatus ApplyStatus { get; set; }


    }
}