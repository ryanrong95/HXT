using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PsWms.PdaApi.Services.Enums
{
    /// <summary>
    /// 通知的状态
    /// </summary>
    public enum NoticeStatus
    {
        /// <summary>
        /// 等待库房处理, 入库：等待安排提货, 出库：等待安排送货
        /// </summary>
        [Description("等待库房安排")]
        Arranging = 101,

        /// <summary>
        /// 用于出库自提, 送至自提区
        /// </summary>
        [Description("等待客户提货")]
        ClientTaking = 102,

        /// <summary>
        /// 等待库房处理：入库：分拣, 出库：拣货
        /// </summary>
        [Description("等待处理")]
        Processing = 200,

        /// <summary>
        /// 等待盘点
        /// </summary>
        [Description("等待盘点")]
        Stocking = 201,

        /// <summary>
        /// 等待复核, 出入库都有复核
        /// </summary>
        [Description("等待复核")]
        Reviewing = 202,

        /// <summary>
        /// 等待包装, 出库复核后使用
        /// </summary>
        [Description("等待包装")]
        Packing = 251,

        /// <summary>
        /// 等待收货", 出库复核后使用,同为已经出库
        /// </summary>
        [Description("等待收货")]
        Arrivaling = 300,

        /// <summary>
        /// 拒收，无法出库,库房有拒的权利
        /// </summary>
        [Description("拒收")]
        Rejected = 401,

        /// <summary>
        /// 关闭
        /// </summary>
        [Description("关闭")]
        Closed = 500,

        /// <summary>
        /// 完成， 出入库均可使用
        /// </summary>
        [Description("完成")]
        Completed = 600,

    }

    /// <summary>
    /// 库房通知类型
    /// </summary>
    public enum NoticeType
    {
        /// <summary>
        /// 入库
        /// </summary>
        [Description("入库")]
        Inbound = 1,

        /// <summary>
        /// 出库
        /// </summary>
        [Description("出库")]
        Outbound = 2,

        /// <summary>
        /// 即入即出
        /// </summary>
        [Description("即入即出")]
        InAndOut = 3,
    }

    /// <summary>
    /// 通知来源
    /// </summary>
    public enum NoticeSource
    {
        /// <summary>
        /// 库房, 库管
        /// </summary>
        [Description("库房")]
        Keeper = 1,

        /// <summary>
        /// 跟单, 客服
        /// </summary>
        [Description("跟单")]
        Tracker = 2
    }
}
