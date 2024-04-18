using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 付款方式
    /// </summary>
    public enum PaymentMethord
    {
        /// <summary>
        /// 银行电汇
        /// </summary>
        [Description("银行电汇")]
        BankTransfer = 1,

        /// <summary>
        /// 支票
        /// </summary>
        [Description("支票")]
        Cheque = 2,

        /// <summary>
        /// 现金
        /// </summary>
        [Description("现金")]
        Cash = 3,

        /// <summary>
        /// 银行承兑
        /// </summary>
        [Description("银行承兑")]
        BankAcceptanceBill = 4,

        /// <summary>
        /// 商业承兑
        /// </summary>
        [Description("商业承兑")]
        CommercialAcceptanceBill = 5,

        /// <summary>
        /// 云信
        /// </summary>
        [Description("云信")]
        Yunxin = 6,

        /// <summary>
        /// E信通
        /// </summary>
        [Description("E信通")]
        EXinTong = 7,

        /// <summary>
        /// 支付宝
        /// </summary>
        [Description("支付宝")]
        Alipay = 8,

        /// <summary>
        /// 微信
        /// </summary>
        [Description("微信")]
        Wechat = 9,
    }
}
