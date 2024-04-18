using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PsWms.SzMvc.Models
{
    public class GetMyInvoiceReturnModel
    {
        /// <summary>
        /// InvoiceID
        /// </summary>
        public string InvoiceID { get; set; }

        /// <summary>
        /// 开票名称
        /// </summary>
        public string InvoiceName { get; set; }

        /// <summary>
        /// 纳税人识别号
        /// </summary>
        public string TaxNumber { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string RegAddress { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string CompanyTel { get; set; }

        /// <summary>
        /// 开户行名称
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 银行账号
        /// </summary>
        public string BankAccount { get; set; }

        /// <summary>
        /// 发票交付方式 Int
        /// </summary>
        public string DeliveryTypeInt { get; set; }

        /// <summary>
        /// 发票交付方式名称
        /// </summary>
        public string DeliveryTypeName { get; set; }

        /// <summary>
        /// 收票地址完整
        /// </summary>
        public string RevAddress { get; set; }

        /// <summary>
        /// 收票地址地区部分
        /// </summary>
        public string[] RevAddressArray { get; set; }

        /// <summary>
        /// 收票地址详细地址
        /// </summary>
        public string RevAddressDetail { get; set; }

        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// 联系人手机
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 联系人邮箱
        /// </summary>
        public string Email { get; set; }
    }

    public class InvoiceInfoSubmitModel
    {
        /// <summary>
        /// InvoiceID
        /// </summary>
        public string InvoiceID { get; set; }

        /// <summary>
        /// 开票名称
        /// </summary>
        public string InvoiceName { get; set; }

        /// <summary>
        /// 纳税人识别号
        /// </summary>
        public string TaxNumber { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string RegAddress { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string CompanyTel { get; set; }

        /// <summary>
        /// 开户行名称
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 银行账号
        /// </summary>
        public string BankAccount { get; set; }

        /// <summary>
        /// 发票交付方式 Int
        /// </summary>
        public int DeliveryTypeInt { get; set; }

        /// <summary>
        /// 收票地址地区部分
        /// </summary>
        public string[] RevAddressArray { get; set; }

        /// <summary>
        /// 收票地址详细地址
        /// </summary>
        public string RevAddressDetail { get; set; }

        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// 联系人手机
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 联系人邮箱
        /// </summary>
        public string Email { get; set; }
    }
}