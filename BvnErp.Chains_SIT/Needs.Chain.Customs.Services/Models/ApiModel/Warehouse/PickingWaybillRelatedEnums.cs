using Needs.Utils.Descriptions;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 通用状态
    /// </summary>
    public enum GeneralStatus
    {
        /// <summary>
        /// 待处理
        /// </summary>
        [Description("待处理")]
        Waiting = 100,
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal = 200,
        /// <summary>
        /// 关闭
        /// </summary>
        [Description("关闭")]
        Closed = 400,
        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        Deleted = 500
    }


    /// <summary>
    /// 汇款方式
    /// </summary>
    public enum Methord
    {
        /// <summary>
        /// TT
        /// </summary>
        [Description("TT")]
        TT = 1,
        /// <summary>
        /// 支付宝
        /// </summary>
        [Description("支付宝")]
        Alipay = 2,
        /// <summary>
        /// 转账
        /// </summary>

        [Description("转账")]
        Transfer = 3,
        /// <summary>
        /// 电汇
        /// </summary>

        [Description("电汇")]
        Exchange = 4,
        /// <summary>
        /// 支票
        /// </summary>
        [Description("支票")]
        Check = 5,
        /// <summary>
        /// 现金
        /// </summary>

        [Description("现金")]
        Cash = 6,
    }

    /// <summary>
    /// 通知的（业务）来源
    /// </summary>
    /// <remarks>
    /// 通知是由于什么业务产生的
    /// </remarks>
    public enum NoticeSource
    {
        /// <summary>
        /// 入库
        /// </summary>
		[Description("代收货")]
        AgentEnter = 10,

        /// <summary>
        /// 出库
        /// </summary>
        [Description("代发货")]
        AgentSend = 20,

        /// <summary>
        /// 报关
        /// </summary>
        [Description("代报关")]
        AgentBreakCustoms = 30,
        /// <summary>
        /// 转运
        /// </summary>
        [Description("代转运")]
        Transfer = 40,
        /// <summary>
        /// 检测
        /// </summary>
        [Description("代检测")]
        AgentTesting = 50
    }
    public enum NoticesStatus
    {
        /// <summary>
        /// 等待
        /// </summary>
        [Description("待处理")]
        Waiting = 100,

        /// <summary>
        /// 部分到货
        /// </summary>
        [Description("部分到货")]
        PartialArriva = 110,

        /// <summary>
        /// 暂存
        /// </summary>
        [Description("暂存")]
        TempStorage = 120,

        /// <summary>
        /// 完成
        /// </summary>
        [Description("完成")]
        Completed = 200,

        /// <summary>
        /// 异常到货
        /// </summary>
        [Description("异常到货")]
        Abnormal = 300,

        /// <summary>
        /// 关闭
        /// </summary>
        [Description("关闭")]
        Closed = 400,
    }

    public enum NoticesTarget
    {

        /// <summary>
        /// 按地址分拣
        /// </summary>
        [Description("按地址分拣")]

        Address = 100,

        /// <summary>
        /// 按报关分拣
        /// </summary>
        [Description("按报关分拣")]

        Customs = 200,

        /// <summary>
        /// 按所有人分拣
        /// </summary>
        [Description("按所有人分拣")]

        Owner = 300,

        /// <summary>
        /// 按订单分拣
        /// </summary>
        [Description("按订单分拣")]

        Order = 400,

        /// <summary>
        /// 按订单通知分拣
        /// </summary>
        [Description("按订单通知分拣")]

        OrderNotice = 500,

        /// <summary>
        /// 按采购员分拣
        /// </summary>
        [Description("按采购员分拣")]

        Purchaser = 600,

        /// <summary>
        /// 按销售分拣
        /// </summary>
        [Description("按销售分拣")]

        Sales = 700,

        /// <summary>
        /// 按客户分拣
        /// </summary>
        [Description("按客户分拣")]

        Customer = 800,
    }

    /// <summary>
    /// 出库进运单执行状态
    /// </summary>
    public enum PickingExcuteStatus
    {
        /// <summary>
        /// 待处理
        /// </summary>
        [Description("待处理")]
        Waiting = 200,

        /// <summary>
        /// 正在分拣
        /// </summary>
        [Description("正在分拣")]
        Picking = 205,

        /// <summary>
        /// 等待发货
        /// </summary>
        [Description("等待发货")]
        WaitDelivery = 210,

        /// <summary>
        /// 已出库
        /// </summary>
        [Description("已出库")]
        OutStock = 215,

        /// <summary>
        /// 异常
        /// </summary>
        [Description("异常")]
        Anomalous = 220,

        ///// <summary>
        ///// 删除
        ///// </summary>
        //[Description("删除")]
        //Deleted = 225,
    }

    /// <summary>
    /// 通知类型
    /// </summary>
    /// <remarks>
    /// 在库的人员要做的事情
    /// </remarks>
    public enum NoticeType
    {
        /// <summary>
        /// 采购入库
        /// </summary>
        [Description("采购入库", "采购")]

        PurchasingEnter = 100,

        /// <summary>
        /// 馈赠入库
        /// </summary>
        [Description("馈赠入库", "采购")]

        GiftEnter = 105,

        /// <summary>
        /// 退货入库
        /// </summary>
        [Description("退货入库", "采购")]

        ReturnEnter = 110,

        /// <summary>
        /// 补货入库
        /// </summary>
        [Description("补货入库", "采购")]
        ReplenishmentEnter = 115,

        /// <summary>
        /// 销售出库
        /// </summary>
        [Description("销售出库", "销售")]

        SaleOut = 200,

        /// <summary>
        /// 馈赠出库
        /// </summary>
        [Description("馈赠出库", "采购")]

        GiftOut = 205,

        /// <summary>
        /// 补货出库
        /// </summary>
        [Description("补货出库", "采购")]

        ReplenishmentOut = 210,


        /// <summary>
        /// 代收货入库
        /// </summary>
        [Description("代收货入库", "代仓储")]

        ReceiptEnter = 300,

        /// <summary>
        /// 代发货出库
        /// </summary>
        [Description("代发货出库", "代仓储")]
        SendOut = 305,

        /// <summary>
        /// 报关入库
        /// </summary>
        [Description("报关入库", "代仓储（代报关）")]
        CustomsEnter = 310,

        /// <summary>
        /// 报关出库
        /// </summary>
        [Description("报关出库", "代仓储（代报关）")]
        CustomsOut = 315,


        /// <summary>
        /// 备货入库
        /// </summary>
        [Description("备货入库", "备货")]

        StockUpEnter = 400,


        /// <summary>
        /// 转运入库
        /// </summary>
        [Description("转运入库", "转运")]
        TransferEnter = 500,

        /// <summary>
        /// 转运出库
        /// </summary>
        [Description("转运出库", "转运")]
        TransferOut = 505,

    }

    /// <summary>
    /// 运单类型
    /// </summary>
    public enum WaybillType
    {

        [Description("自提")]
        PickUp = 1,

        [Description("送货上门")]
        DeliveryToWarehouse = 2,

        [Description("本地快递")]
        LocalExpress = 3,

        [Description("国际快递")]
        InternationalExpress = 4,
    }

    /// <summary>
    /// 货物条款类型
    /// </summary>
    public enum WayChargeType
    {
        [Description("代付货款")]
        PayCharge = 1,

        [Description("代收货款")]
        ReciveCharge = 2,
    }
}
