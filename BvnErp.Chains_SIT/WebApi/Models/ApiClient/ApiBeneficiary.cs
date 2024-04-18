using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    /// <summary>
    /// 受益人
    /// </summary>
    public class ApiBeneficiary
    {
        /// <summary>
        /// 企业
        /// </summary>
        public EnterpriseObj Enterprise { get; set; }
        /// <summary>
        /// 3海关、2增票、1普票、0无法开票（Yahv.Underly.InvoiceType）
        /// </summary>
        public int InvoiceType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// MD5(RealName)
        /// </summary>

        public string RealID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Bank { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BankAddress { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public string Account { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SwiftCode { get; set; }
        /// <summary>
        /// 汇款方式(TT,支付宝)
        /// </summary>
        public int Methord { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Currency { get; set; }
        /// <summary>
        /// 地区
        /// </summary>
        public int District { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CreateDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UpdateDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CreatorID { get; set; }
    }
}