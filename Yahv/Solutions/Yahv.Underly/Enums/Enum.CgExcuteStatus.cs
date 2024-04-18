using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 入库时运单执行状态---重构
    /// </summary>
    public enum CgSortingExcuteStatus
    {
        /// <summary>
        /// 全部
        /// </summary>
        [Description("全部")]
        All = 10,

        /// <summary>
        /// 等待分拣
        /// </summary>
        [Description("等待分拣")]
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
        Completed = 120,

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
    /// 暂存时运单执行状态
    /// </summary>
    public enum CgTempStockExcuteStatus
    {
        /// <summary>
        /// 暂存
        /// </summary>
        [Description("暂存")]
        TempStock = 300,
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
    /// 截单状态
    /// </summary>
    public enum CgCuttingOrderStatus
    {
        /// <summary>
        /// 待处理
        /// </summary>
        [Description("等待")]
        Waiting = 0,

        /// <summary>
        /// 截单
        /// </summary>
        [Description("已截单")]
        Cutting = 1,

        /// <summary>
        /// 截单
        /// </summary>
        [Description("已完成")]
        Completed = 2,
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

    /// <summary>
    /// 是否上传送货单
    /// </summary>
    public enum IsUpload
    {
        /// <summary>
        /// 全部
        /// </summary>
        [Description("全部")]
        All = 2,

        /// <summary>
        /// 未上传
        /// </summary>
        [Description("未上传")]
        Uploading = 0,

        /// <remarks>
        /// 已上传
        /// </remarks>
        [Description("已上传")]
        Uploaded = 1,

    
    }
}
