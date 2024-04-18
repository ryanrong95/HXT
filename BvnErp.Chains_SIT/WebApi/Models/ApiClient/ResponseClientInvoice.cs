using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    /// <summary>
    /// 客户发票信息
    /// </summary>
    public class ResponseClientInvoice
    {
        /// <summary>
        /// 客户ID
        /// </summary>

        public string ClientId { get; set; }
        /// <summary>
        ///  统一社会信用编码
        /// </summary>
        public string CompanyCode { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// <summary>
        /// 客户发票ID
        /// </summary>
        public string InvoiceId { get; set; }

        /// <summary>
        /// 纳税人识别号
        /// </summary>
        public string TaxCode { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Tel { get; set; }

      
        /// <summary>
        /// 开户行
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string BankAccount { get; set; }
        /// <summary>
        /// 交付方式
        /// </summary>
        public int DeliveryType { get; set; }
        /// <summary>
        /// 开票信息备注
        /// </summary>
        public string Summary { get; set; }
        
        /// <summary>
        /// 收件地址
        /// </summary>
          public InvoiceConsignee InvoiceConsignee { get; set; }
    }

    ///// <summary>
    ///// 客户信息
    ///// </summary>
    //public class ClientInfo
    //{

    //    /// <summary>
    //    /// 客户ID
    //    /// </summary>

    //    public string clientId { get; set; }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string CompanyCode { get; set; }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string CompanyName { get; set; }
    //    /// <summary>
    //    /// 发票地址
    //    /// </summary>
    //    public string Address { get; set; }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string ContactName { get; set; }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string Email { get; set; }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string Tel { get; set; }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string Mobile { get; set; }


    //}

    ///// <summary>
    /////发票信息
    ///// </summary>
    //public class InvoiceInfo
    //{
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string InvoiceId { get; set; }


    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string TaxCode { get; set; }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string Address { get; set; }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string Tel { get; set; }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string Model { get; set; }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string BankName { get; set; }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string BankAccount { get; set; }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public int DeliveryType { get; set; }
    //}

    /// <summary>
    /// 发票收件地址
    /// </summary>
    public class InvoiceConsignee
    {
        /// <summary>
        /// 收件人
        /// </summary>
        public string ConsigneeName { get; set; }
        /// <summary>
        /// 收件地址
        /// </summary>
        public string ConsigneeAddress { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string ConsigneeMobile { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string ConsigneeTel { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string ConsigneeEmail { get; set; }

    }
}