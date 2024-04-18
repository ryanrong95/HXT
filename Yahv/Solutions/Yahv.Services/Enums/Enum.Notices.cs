using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Services.Enums
{
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
        /// 报关（外）
        /// </summary>
        [Description("代报关")]
        AgentBreakCustoms = 30,
        /// <summary>
        /// 报关(内)
        /// </summary>
        [Description("代报关(内)")]
        AgentBreakCustomsForIns = 35,
        /// <summary>
        /// 转运
        /// </summary>
        [Description("代转运")]
        Transfer = 40,
        /// <summary>
        /// 检测
        /// </summary>
        [Description("代检测")]
        AgentTesting = 50,
        /// <summary>
        /// 转报关
        /// </summary>
        [Description("转报关")]
        AgentCustomsFromStorage = 60,
    }

    /// <summary>
    /// 通知的（业务）来源 ---重构
    /// </summary>
    /// <remarks>
    /// 通知是由于什么业务产生的
    /// </remarks>
    public enum CgNoticeSource
    {
        /// <summary>
        /// 全部
        /// </summary>
        /// <remarks>
        /// 增加All 只是权宜之计，未来类似这样的前端要求只能是前端自己增加
        /// </remarks>
        [Description("全部")]
        All = 1,
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
        /// 报关（外）
        /// </summary>
        [Description("代报关")]
        AgentBreakCustoms = 30,
        /// <summary>
        /// 报关(内)
        /// </summary>
        [Description("代报关(内)")]
        //[Obsolete]
        AgentBreakCustomsForIns = 35,
        /// <summary>
        /// 转运
        /// </summary>
        [Description("代转运")]
        Transfer = 40,
        /// <summary>
        /// 检测
        /// </summary>
        [Description("代检测")]
        AgentTesting = 50,
        /// <summary>
        /// 转报关
        /// </summary>
        [Description("转报关")]
        AgentCustomsFromStorage = 60,
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
    /// 通知类型
    /// </summary>
    public enum CgNoticeType
    {
        /// <summary>
        /// 入库
        /// </summary>
        [Description("入库")]
        Enter = 100,
        /// <summary>
        /// 出库
        /// </summary>
        [Description("出库")]
        Out = 200,
        /// <summary>
        /// 装箱
        /// </summary>
        [Description("装箱")]
        Boxing = 300,
        /// <summary>
        /// 拣货
        /// </summary>
        [Description("拣货")]
        Picking = 400,
        /// <summary>
        /// 检测
        /// </summary>
        [Description("检测")]
        Testing = 500,

        //例如：盘点
    }

    /// <summary>
    /// 通知方式
    /// </summary>
    public enum NoticesTarget
    {
        [Description("默认分拣方式")]
        Default = 10,

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

    public enum BoxingSpecs
    {
        [Description("大号箱")]
        Max = 1,
        [Description("中号箱")]
        Middle = 2,
        [Description("小号箱")]
        Min = 3
    }


    /// <summary>
    /// 通知条件
    /// </summary>
    /// <remarks>
    /// 可以做双向的，入库条件、出库条件
    /// 未来可以做多向的，例如：在库的检测条件等
    /// 存储条件：冷藏等
    /// </remarks>
    public class NoticeCondition : WsCondition
    {
        /// <summary>
        /// 是否拆箱检测
        /// </summary>
        public bool DevanningCheck { get; set; }

        /// <summary>
        /// 是否称重
        /// </summary>
        public bool Weigh { get; set; }

        /// <summary>
        /// 是否点数
        /// </summary>
        public bool CheckNumber { get; set; }

        /// <summary>
        /// 是否上机检测
        /// </summary>
        public bool OnlineDetection { get; set; }

        /// <summary>
        /// 是否贴标签
        /// </summary>
        public bool AttachLabel { get; set; }

        /// <summary>
        /// 是否涂抹标签
        /// </summary>
        public bool PaintLabel { get; set; }

        /// <summary>
        /// 是否重新标签
        /// </summary>
        public bool Repacking { get; set; }

        /// <summary>
        /// 是否3C
        /// </summary>
        public bool IsCCC { get; set; }

        /// <summary>
        /// 是否商检
        /// </summary>
        public bool IsCIQ { get; set; }

        /// <summary>
        /// 是否禁运
        /// </summary>
        public bool IsEmbargo { get; set; }

        /// <summary>
        /// 是否高价值
        /// </summary>
        public bool IsHighPrice { get; set; }

        /// <summary>
        /// 是否申报
        /// </summary>
        public bool IsDeclared { get; set; }
    }

    /// <summary>
    /// 库房统一条件类
    /// </summary>
    public class WsCondition
    {
        public WsCondition()
        {
            //Models.WayCondition


        }
    }
}
