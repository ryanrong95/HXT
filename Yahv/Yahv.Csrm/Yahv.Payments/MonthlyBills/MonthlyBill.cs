using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Payments;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Payments
{
    /// <summary>
    /// 月结账单
    /// </summary>
    //public class MonthlyBill
    //{
    //    /// <summary>
    //    /// 订单编号
    //    /// </summary>
    //    public string OrderID { get; set; }
    //    public string ReceivableID { get; set; }
    //    /// <summary>
    //    /// 付款人
    //    /// </summary>
    //    public string Payer { get; set; }
    //    /// <summary>
    //    /// 收款人
    //    /// </summary>
    //    public string Payee { get; set; }
    //    /// <summary>
    //    /// 业务
    //    /// </summary>
    //    public string Business { get; set; }
    //    /// <summary>
    //    /// 分类
    //    /// </summary>
    //    public string Catalog { get; set; }
    //    /// <summary>
    //    /// 科目
    //    /// </summary>
    //    public string Subject { get; set; }
    //    /// <summary>
    //    /// 币种
    //    /// </summary>
    //    public Currency Currency { get; set; }

    //    /// <summary>
    //    /// 创建时间
    //    /// </summary>
    //    public DateTime LeftDate { get; set; }
    //    /// <summary>
    //    /// 应收
    //    /// </summary>

    //    public decimal LeftPrice { get; set; }

    //    public decimal? Price { get; set; }
    //    /// <summary>
    //    /// 信用还款（信用类型时使用）
    //    /// </summary>

    //    public decimal? PaidPrice { get; set; }


    //    /// <summary>
    //    /// 银行支付
    //    /// </summary>
    //    public decimal? BankPay { get; set; }

    //    /// <summary>
    //    /// 还款日期
    //    /// </summary>
    //    public DateTime? PayDate { get; set; }
    //    /// <summary>
    //    ///收款账户
    //    /// </summary>
    //    public string AccountName { get; set; }

    //    /// <summary>
    //    ///收款账号
    //    /// </summary>
    //    public string BankAccount { get; set; }

    //    /// <summary>
    //    /// 流水号
    //    /// </summary>
    //    public string FormCode { get; set; }
    //    public int? AccountType { get; set; }

    //    /// <summary>
    //    /// 添加人
    //    /// </summary>
    //    public string Creator { get; set; }


    //}

    //public class BillModelData
    //{
    //    //public DateTime? PayDate { get; set; }

    //    ///// <summary>
    //    ///// 付款人
    //    ///// </summary>
    //    //public string Payer { get; set; }
    //    ///// <summary>
    //    ///// 收款人
    //    ///// </summary>
    //    //public dynamic Payee { get; set; }

    //    //public List<MonthlyBill> CNYData { get; set; }

    //    //public List<MonthlyBill> HKDData { get; set; }

    //    //public List<MonthlyBill> USDData { get; set; }

    //}

    //public class PayDateData
    //{
    //    public DateTime? PayDate { get; set; }

    //    public List<OrderData> OrderData { get; set; }
    //}

    public class OrderData
    {
        public string OrderID { get; set; }

        public IEnumerable<BillData> Items { get; set; }
        ///// <summary>
        ///// 开票类型
        ///// </summary>
        //public int InvoiceType { get; set; }
        //public List<CurrenryData> CurrenryData { get; set; }

        //public RelatedInvoice InvoiceData { get; set; }
        //public RelatedInvoice CustomInvoiceData { get; set; }
    }

    public class CurrenryData
    {
        public string CurrencyName { get; set; }
        public Currency Currency { get; set; }
        public IEnumerable<OrderData> Items { get; set; }

    }

    public class BillData
    {
        public string OrderID { get; set; }
        /// <summary>
        /// 业务
        /// </summary>
        public string Business { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public string Catalog { get; set; }
        /// <summary>
        /// 科目
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 应收
        /// </summary>

        public decimal LeftPrice { get; set; }
       
        /// <summary>
        /// 同一订单下 同一币种的  实收和
        /// </summary>
        public decimal? RightPrice { get; set; }
        /// <summary>
        /// 信用支付
        /// </summary>
        public decimal? CreditPay { get; set; }
        /// <summary>
        /// 信用还款（信用类型时使用）
        /// </summary>
        public decimal? PaidPrice { get; set; }

        /// <summary>
        /// 银行支付
        /// </summary>
        public decimal? BankPay { get; set; }
        /// <summary>
        /// 差额
        /// </summary>
        public decimal NotPayAmount { get; set; }
        /// <summary>
        /// 账户类型
        /// </summary>
        public AccountType AccountType { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? LeftDate { get; set; }
        public Currency Currency { get; set; }
      
        
    }
    /// <summary>
    /// 合计
    /// </summary>
    public class BillTotal
    {
        public string CurrencyName { get; set; }
        public Currency Currency { get; set; }
        public decimal LeftPrice { get; set; }
        public decimal RightPrice { get; set; }

    }

    public class financeBill
    {
        public DateTime? PayDate { get; set; }

        public IEnumerable<CurrenryData> Items { get; set; }
    }
    /// <summary>
    /// 服务费和海关发票
    /// </summary>
    public class ServiceAndCustomInvoice
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; }
        /// <summary>
        /// 发票号
        /// </summary>
        public string InvoiceNo { get; set; }
        /// <summary>
        /// 应开金额
        /// </summary>
        public decimal? LeftPrice { get; set; }
        /// <summary>
        /// 差额（可正可负）
        /// </summary>
        public decimal? Difference { get; set; }
        /// <summary>
        /// 实开金额
        /// </summary>
        public decimal? RightPrice { get; set; }

        /// <summary>
        /// 开票日期
        /// </summary>
        public DateTime? InvoiceDate { get; set; }

        /// <summary>
        /// 开票类型
        /// </summary>
        public int InvoiceType { get; set; }

        public string ClientName { get; set; }

        /// <summary>
        /// 发票号
        /// </summary>
        public string CusInvoiceNo { get; set; }
        /// <summary>
        /// 应开金额
        /// </summary>
        public decimal? CusLeftPrice { get; set; }
        /// <summary>
        ///  /// <summary>
        /// 实开金额
        /// </summary>
        public decimal? CusRightPrice { get; set; }
        /// <summary>
        /// 开票日期
        /// </summary>
        public DateTime? CusInvoiceDate { get; set; }
    }

   
}
