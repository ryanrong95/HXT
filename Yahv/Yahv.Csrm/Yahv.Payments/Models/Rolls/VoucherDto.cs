using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Payments.Models.Rolls
{
    /// <summary>
    /// 财务通知单录入
    /// </summary>
    public class VoucherDto
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 财务通知类型
        /// </summary>
        public VoucherType Type { get; set; }

        /// <summary>
        /// 付款公司
        /// </summary>
        public string Payer { get; set; }

        /// <summary>
        /// 收款公司
        /// </summary>
        public string Payee { get; set; }

        /// <summary>
        /// 财务通知ID
        /// </summary>
        public string VoucherID { get; set; }

        /// <summary>
        /// 收款银行（应与收益人相同）
        /// </summary>
        public string Bank { get; set; }

        /// <summary>
        /// 收款银行账户
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 银行、现金收付款手续的流水号
        /// </summary>
        public string FormCode { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        
        public string ApplicationID { get; set; }
    }
}
