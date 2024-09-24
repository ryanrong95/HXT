using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 货物特殊处理
    /// </summary>
    public enum SpecialRequire
    {
        /// <summary>
        /// 标签处理
        /// </summary>
        [Description("标签处理")]
        Label = 1,

        /// <summary>
        /// 换箱单
        /// </summary>
        [Description("换箱单")]
        ChangePackingFile = 2,

        /// <summary>
        /// 换纸箱
        /// </summary>
        [Description("换纸箱")]
        ChangeBox = 3,

        /// <summary>
        /// 分箱
        /// </summary>
        [Description("分箱")]
        DivideBox = 4,

        /// <summary>
        /// 换卡板
        /// </summary>
        [Description("换卡板")]
        ChangePallet = 5,
    }

    /// <summary>
    /// 标签处理类型
    /// </summary>
    public enum LabelType
    {
        /// <summary>
        /// 贴标签
        /// </summary>
        [Description("贴标签")]
        Labeling = 1,

        /// <summary>
        /// 撕标签
        /// </summary>
        [Description("撕标签")]
        TearLabel = 2,

        /// <summary>
        /// 先撕后贴
        /// </summary>
        [Description("先撕后贴")]
        TearAndStick = 3,
    }

    /// <summary>
    /// 订单服务类型
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// 仓储收货
        /// </summary>
        [Description("仓储收货")]
        Recieved = 1,
        /// <summary>
        /// 即收即发
        /// </summary>
        [Description("即收即发")]
        Transport = 2,
        /// <summary>
        /// 代发货
        /// </summary>
        [Description("代发货")]
        Delivery = 3,
        /// <summary>
        /// 转报关
        /// </summary>
        [Description("转报关")]
        TransferDeclare = 4,
        /// <summary>
        /// 代报关
        /// </summary>
        [Description("代报关")]
        Declare = 5,
    }

    /// <summary>
    /// 订单状态类型
    /// </summary>
    public enum OrderStatusType
    {
        [Description("主状态")]
        MainStatus = 1,

        [Description("支付状态")]
        PaymentStatus = 2,

        [Description("开票状态")]
        InvoiceStatus = 3,

        [Description("付汇状态")]
        RemittanceStatus = 4,
    }

    /// <summary>
    /// 订单支付状态
    /// </summary>
    public enum OrderPaymentStatus
    {
        /// <summary>
        /// 待出账
        /// </summary>
        [Description("待出账")]
        Waiting = 1,

        /// <summary>
        /// 待确认
        /// </summary>
        [Description("待确认")]
        Confirm = 2,

        /// <summary>
        /// 待支付
        /// </summary>
        [Description("待支付")]
        ToBePaid = 3,

        /// <summary>
        /// 部分支付
        /// </summary>
        [Description("部分支付")]
        PartPaid = 4,

        /// <summary>
        /// 已支付
        /// </summary>
        [Description("已支付")]
        Paid = 5,
    }

    /// <summary>
    /// 订单开票状态
    /// </summary>
    public enum OrderInvoiceStatus
    {
        [Description("未开票")]
        UnInvoiced = 1,

        [Description("已申请")]
        Applied = 2,

        [Description("已开票")]
        Invoiced = 3,
    }

    /// <summary>
    /// 订单的付汇状态
    /// </summary>
    public enum OrderRemittanceStatus
    {
        [Description("未付汇")]
        UnRemittance = 1,

        [Description("部分付汇")]
        PartRemittance = 2,

        [Description("已付汇")]
        Remittanced = 3
    }

    /// <summary>
    /// 重构客户端订单状态
    /// </summary>
    public enum CgOrderStatus
    {
        [Description("退回")]
        退回 = 5, //跟单退回

        [Description("草稿")]
        暂存 = 10,//客户保存

        [Obsolete("报关业务不使用")]
        [Description("挂起")]
        挂起 = 20,//客户挂起

        [Obsolete("报关业务不使用")]
        [Description("已提交")]
        已提交 = 100,//已提交

        [Description("待审核")]
        待审核 = 110,//报关业务提交到华芯通之后

        [Description("待确认")]
        待确认 = 150,//报价完成后等待客户确认

        [Description("待交货")]
        待交货 = 200, //向库房发入库通知

        [Obsolete("建议废除")]
        [Description("已交货")]
        已交货 = 300, //仓储业务库房入库完成

        [Description("已装箱")]
        已装箱 = 305, //报关业务

        [Description("待报关")]
        待报关 = 310, //报关业务入库完成

        [Obsolete("建议废除")]
        [Description("已申报")]
        已申报 = 400,//报关业务封箱完成

        [Description("待收货")]
        待收货 = 500,//发出库通知,

        [Obsolete("建议废除")]
        [Description("待收货")]
        客户待收货 = 600, //仓库出库完成

        [Description("已收货")]
        客户已收货 = 700, //客户确认收货

        [Description("取消")]
        取消 = 999,
    }

    ///// <summary>
    ///// 申请状态
    ///// </summary>
    //public enum ApplicationStatus
    //{

    //    [Description("待审核")]
    //    Examining = 1,

    //    [Description("已审核")]
    //    Examined = 2,

    //    [Description("待审批")]
    //    Approving = 3,

    //    [Description("已审批")]
    //    Approved = 4,

    //    [Description("已完成")]
    //    Completed = 5,

    //    [Description("已驳回")]
    //    Reject = 6,
    //}

    ///// <summary>
    ///// 申请类型
    ///// </summary>
    //public enum ApplicationType
    //{
    //    [Description("付款申请")]
    //    Payment = 1,

    //    [Description("收款申请")]
    //    Receival = 2,
    //}

    /// <summary>
    /// 代付款手续费类型
    /// </summary>
    public enum HandlingFeePayerType
    {
        [Description("收款方")]
        收款方 = 1,

        [Description("付款方")]
        付款方 = 2,

        [Description("双方承担")]
        双方承担 = 3,
    }
}
