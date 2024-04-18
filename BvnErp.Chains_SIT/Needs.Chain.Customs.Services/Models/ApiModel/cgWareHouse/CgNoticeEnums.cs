using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{

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
    /// 运费支付人
    /// </summary>
    public enum WaybillPayer
    {

        [Description("交货人")]
        Consignor = 1,

        [Description("收货人")]
        Consignee = 2,
    }


    /// <summary>
    /// 库存状态
    /// </summary>
    public enum StoragesStatus
    {
        [Description("正常")]
        Normal = 200,
        [Description("删除")]
        Deleted = 400,
        [Description("停用")]
        StopUsing = 500
    }


    /// <summary>
    /// (新的)装箱单状态
    /// </summary>
    public enum TinyOrderDeclareStatus
    {
        [Description("已装箱")]
        Boxed = 20,
        [Description("申报中")]
        Declaring = 30,
        /// <summary>
        /// 待出库
        /// </summary>
        /// <remarks>
        /// 是收到 报关出库的通知
        /// 故意与通知类型做个区别
        /// </remarks>
        [Description("待装运")]
        Shiping = 40,
        /// <summary>
        /// 故意与通知类型做个区别
        /// </summary>
        /// <remarks>故意与通知类型做个区别</remarks>
        [Description("已装运")]
        Shiped = 50,
    }

    /// <summary>
    /// 库存类型
    /// </summary>
    public enum CgStoragesType
    {
        /// <summary>
        /// 流水库
        /// </summary>
        [Description("流水库")]
        Flows = 200,
        /// <summary>
        /// 库存库
        /// </summary>
        [Description("库存库")]
        Stores = 300,
        /// <summary>
        /// 运营库
        /// </summary>
        [Description("运营库")]
        Operatings = 400,
        /// <summary>
        /// 报关库
        /// </summary>
        [Description("报关库")]
        Customs = 500,
        /// <summary>
        /// 报废库
        /// </summary>
        [Description("报废库")]
        Trashs = 600,
        /// <summary>
        /// 检测库
        /// </summary>
        [Description("检测库")]
        Testing = 700,
        /// <summary>
        /// 暂存库
        /// </summary>
        [Description("暂存库")]
        Staging = 800,
        /// <summary>
        /// 异常库
        /// </summary>
        [Description("异常库")]
        Abnormal = 810,
        /// <summary>
        /// 退货库
        /// </summary>
        [Description("退货库")]
        Returns = 900,
    }


    /// <summary>
    /// 出库进运单执行状态
    /// </summary>
    public enum CgPickingExcuteStatus
    {
        /// <summary>
        /// 全部
        /// </summary>
        [Description("全部")]
        All = 10,

        /// <summary>
        /// 正在分拣
        /// </summary>
        [Description("等待拣货")]
        Picking = 205,

        /// <remarks>
        /// 部分出库
        /// </remarks>
        [Description("部分装运")]
        PartialShiped = 210,

        /// <remarks>
        /// 完成出库
        /// </remarks>
        [Description("完成装运")]
        Completed = 215,

        /// <summary>
        /// 异常
        /// </summary>
        [Description("异常")]
        Anomalous = 220,
    }

    /// <summary>
    /// 提送货装运状态
    /// </summary>
    public enum CgLoadingExcuteStauts
    {
        /// <summary>
        /// 等待提货
        /// </summary>
        [Description("等待提货")]
        Waiting = 100,

        /// <summary>
        /// 正在提货
        /// </summary>
        [Description("正在提货")]
        Taking = 105,

        /// <summary>
        /// 提货完成
        /// </summary>
        [Description("提货完成")]
        Completed = 200
    }

    /// <summary>
    /// 重构确认收货状态
    /// </summary>
    public enum CgConfirmReceiptStatus
    {
        [Description("等待")]
        Waiting = 100,

        [Description("已确认")]
        Comfirmed = 200
    }

}
