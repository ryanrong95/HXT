using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Finance.Services.Enums
{
    /// <summary>
    /// 文件描述类型
    /// </summary>
    public enum FileDescType
    {
        /// <summary>
        /// 银行收款凭证
        /// </summary>
        [Description("银行收款凭证")]
        ReceiptVoucher = 10,

        /// <summary>
        /// 货款申请
        /// </summary>
        [Description("货款申请")]
        ProductsFeeApply = 20,

        /// <summary>
        /// 货款支付凭证
        /// </summary>
        [Description("货款支付凭证")]
        ProductsFeeVoucher = 21,

        /// <summary>
        /// 代付货款申请
        /// </summary>
        [Description("代付货款申请")]
        ReplaceProductsFeeApply = 30,

        /// <summary>
        /// 代付货款支付凭证
        /// </summary>
        [Description("代付货款凭证")]
        ReplaceProductsFeeVoucher = 31,

        /// <summary>
        /// 费用申请
        /// </summary>
        [Description("费用申请")]
        ChargeApply = 40,

        /// <summary>
        /// 费用支付凭证
        /// </summary>
        [Description("费用支付凭证")]
        ChargeApplyVoucher = 41,

        /// <summary>
        /// 资金申请
        /// </summary>
        [Description("资金申请")]
        FundApply = 50,

        /// <summary>
        /// 资金支付凭证
        /// </summary>
        [Description("资金支付凭证")]
        FundApplyVoucher = 51,

        /// <summary>
        /// 资金调拨申请
        /// </summary>
        [Description("资金调拨申请")]
        SelfApply = 60,

        /// <summary>
        /// 资金调拨申请
        /// </summary>
        [Description("资金调拨凭证")]
        SelfApplyVoucher = 61,

        /// <summary>
        /// 承兑汇票
        /// </summary>
        [Description("承兑汇票")]
        MoneyOrder = 70,

        /// <summary>
        /// 背书转让
        /// </summary>
        [Description("背书转让")]
        Endorsement = 71,

        /// <summary>
        /// 预收退款申请
        /// </summary>
        [Description("预收退款申请")]
        RefundApply = 80,

        /// <summary>
        /// 预收退款申请凭证
        /// </summary>
        [Description("预收退款申请凭证")]
        RefundApplyVoucher = 81,

        /// <summary>
        /// 其他
        /// </summary>
        [Description("其他")]
        Other = 9999,
    }
}
