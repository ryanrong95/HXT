using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 受益人
    /// </summary>
    public class ApiBeneficiary
    {
        /// <summary>
        /// 代仓储供应商
        /// </summary>
        public EnterpriseObj Enterprise { get; set; }
        /// <summary>
        /// 代仓储客户
        /// </summary>
        public EnterpriseObj WsClient { get; set; }
        /// <summary>
        /// 3海关、2增票、1普票、0无法开票（Yahv.Underly.InvoiceType）
        /// </summary>
        public int InvoiceType { get; set; }
        /// <summary>
        /// 供应商的名称
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        ///操作人
        /// </summary>

        public string Creator { get; set; }
        /// <summary>
        /// 银行名称
        /// </summary>
        public string Bank { get; set; }
        /// <summary>
        /// 银行地址
        /// </summary>
        public string BankAddress { get; set; }
        /// <summary>
        /// 账号
        /// </summary>

        public string Account { get; set; }
        /// <summary>
        /// 银行代码
        /// </summary>
        public string SwiftCode { get; set; }
        /// <summary>
        /// 汇款方式(TT,支付宝)
        /// </summary>
        public int Methord { get; set; }
        /// <summary>
        /// 币种
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
        public int Status { get; set; }
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
        public bool IsDefault { get; set; }

        public string Place { get; set; }
    }

   
}