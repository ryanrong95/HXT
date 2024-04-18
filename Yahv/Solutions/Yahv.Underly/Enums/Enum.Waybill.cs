using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 运单类型
    /// </summary>
    public enum WaybillType
    {
        /// <summary>
        /// 全部
        /// </summary>
        /// <remarks>
        /// 类似这样的要求不要在后台处理，临时可以这里处理只是权宜之计。
        /// 例如：在显示的时候，要求增加全部。这个要在显示的阶段处理，不能在程序、接口、数据等地方处理
        /// </remarks>
        [Description("全部")]
        All = 0,

        /// <summary>
        /// 自提
        /// </summary>
        [Description("自提")]
        PickUp = 1,

        /// <summary>
        /// 送货上门
        /// </summary>
        [Description("送货上门")]
        DeliveryToWarehouse = 2,

        [Description("快递")]
        LocalExpress = 3,

        [Description("国际快递")]
        InternationalExpress = 4,
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
    /// 入库时运单执行
    /// </summary>
    public enum SortingExcuteStatus
    {
        /// <summary>
        /// 等待提货
        /// </summary>
        [Description("等待提货")]
        WaitTake = 100,

        /// <summary>
        /// 正在提货
        /// </summary>
        [Description("正在提货")]
        Taking = 105,

        /// <summary>
        /// 等待入库
        /// </summary>
        [Description("等待入库")]
        PendingStorage = 108,

        /// <summary>
        /// 正在分拣
        /// </summary>
        [Description("正在分拣")]
        Sorting = 110,

        /// <summary>
        /// 部分入库
        /// </summary>
        [Description("部分入库")]
        PartStocked = 115,

        /// <summary>
        /// 完成入库
        /// </summary>
        [Description("完成入库")]
        Stocked = 120,

        /// <summary>
        /// 在检测
        /// </summary>
        [Description("在检测")]
        Testing = 125,

        /// <summary>
        /// 异常
        /// </summary>
        [Description("异常")]
        Anomalous = 130

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
    /// 暂存时运单执行状态
    /// </summary>
    public enum TempStockExcuteStatus
    {
        /// <summary>
        /// 暂存
        /// </summary>
        [Description("暂存")]
        TempStock = 300,


    }
}
