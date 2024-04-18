using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 付款方式
    /// </summary>
    public enum PaymentType
    {
        /// <summary>
        /// 支票
        /// </summary>
        [Description("支票")]
        Check = 1,

        /// <summary>
        /// 现金
        /// </summary>
        [Description("现金")]
        Cash = 2,

        /// <summary>
        /// 转账
        /// </summary>
        [Description("转账")]
        TransferAccount = 3,

        /// <summary>
        /// 银行承兑
        /// </summary>
        [Description("银行承兑")]
        AcceptanceBill = 4,

        /// <summary>
        /// 商业承兑
        /// </summary>
        [Description("商业承兑")]
        BusinessAcceptanceBill = 5,

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

    public enum CenterPaymentType
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

    public enum WhsePaymentType
    {
        /// <summary>
        /// 现金
        /// </summary>
        [Description("现金")]
        Cash = 1,

        /// <summary>
        /// 非现金
        /// </summary>
        [Description("非现金")]
        UnCash = 2,
    }

    /// <summary>
    /// 付款操作人类型
    /// </summary>
    public enum PaymentOperatorType
    {
        /// <summary>
        /// 可用的
        /// </summary>
        Avaiable = 0,

        /// <summary>
        /// 付款操作人
        /// </summary>
        PaymentOperator = 1,
    }

    /// <summary>
    /// 账户性质
    /// </summary>
    public enum AccountProperty
    {
        /// <summary>
        /// 公司账户
        /// </summary>
        [Description("公司账户")]
        Public = 1,

        /// <summary>
        /// 个人账户
        /// </summary>
        [Description("个人账户")]
        Private = 2,

        /// <summary>
        /// 未知
        /// </summary>
        [Description("")]
        UnKnown = 10000,
    }

}
