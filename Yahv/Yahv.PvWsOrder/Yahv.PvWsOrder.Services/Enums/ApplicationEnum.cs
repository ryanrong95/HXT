using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PvWsOrder.Services.Enums
{
    /// <summary>
    /// 申请类型
    /// </summary>
    public enum ApplicationType
    {
        /// <summary>
        /// 付款申请
        /// </summary>
        [Description("代付货款")]
        Payment = 1,
        
        /// <summary>
        /// 收款申请
        /// </summary>
        [Description("代收货款")]
        Receival = 2,
    }

    /// <summary>
    /// 申请审批状态
    /// </summary>
    public enum ApplicationStatus
    {

        [Description("待审核")]
        Examining = 1,

        [Description("已审核")]
        Examined = 2,

        [Description("待审批")]
        Approving = 3,

        [Description("经理同意")]
        Approved = 4,

        [Description("已完成")]
        Completed = 5,

        [Description("已驳回")]
        Reject = 6,
    }

    /// <summary>
    /// 收款状态
    /// </summary>
    public enum ApplicationReceiveStatus
    {
        [Description("待收款")]
        UnReceive = 1,

        [Description("已收款")]
        Received = 2,
    }

    /// <summary>
    /// 付款状态
    /// </summary>
    public enum ApplicationPaymentStatus
    {
        [Description("待付款")]
        UnPay = 1,

        [Description("已付款")]
        Paid = 2,
    }

    /// <summary>
    /// 发货时机
    /// </summary>
    public enum DelivaryOpportunity
    {
        [Description("现货现款")]
        CashOn = 1,

        [Description("先款后货")]
        PaymentBeforeDelivery = 2,

        [Description("先货后款")]
        PaymentAfterDelivery = 3,
    }

    /// <summary>
    /// 支票投递方式
    /// </summary>
    public enum CheckDeliveryType
    {
        [Description("自取")]
        Self= 1,

        //[Description("倒存")]
        //Import = 2,

        [Description("快递")]
        Express = 3,

        [Description("送票")]
        Delivery = 4,
    }

    /// <summary>
    /// 审批的状态
    /// </summary>
    public enum ApprovalStatus
    {
        [Description("等待")]
        Waiting = 100,

        [Description("同意")]
        Agree = 200,

        [Description("驳回")]
        Reject = 400,
    }
}
