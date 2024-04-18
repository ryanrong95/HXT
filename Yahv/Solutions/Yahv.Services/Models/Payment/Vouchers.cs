using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 财务实际收付款
    /// </summary>
    public class Voucher
    {
        /// <summary>
        /// 财务确认单
        /// </summary>
        public string ID { get; set; }
        public string OrderID { get; set; }

        /// <summary>
        /// 期号
        /// </summary>
        public int? DateIndex { get; set; }
        /// <summary>
        /// 收款确认，付款确认
        /// </summary>
        public VoucherType Type { get; set; }
        /// <summary>
        /// 付款人
        /// </summary>
        public string Payer { get; set; }
        /// <summary>
        /// 收款人
        /// </summary>
        public string Payee { get; set; }

        public Currency Currency { get; set; }
        /// <summary>
        /// 操作人 
        /// </summary>
        public string CreatorID { get; set; }
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 状态：待确认，已确认，关闭
        /// </summary>
        public VoucherStatus Status { get; set; }

        /// <summary>
        /// 申请ID
        /// </summary>
        public string ApplicationID { get; set; }

        /// <summary>
        /// 是否结算其他费用
        /// </summary>
        public bool IsSettlement { get; set; }
    }
}
