using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PsWms.SzMvc.Services.Enums
{
    public enum LogAction
    {
        /// <summary>
        /// 新增入库订单发送通知
        /// </summary>
        [Description("新增入库订单发送通知")]
        NewStorageInSendNotice = 1,

        /// <summary>
        /// 新增出库订单发送通知
        /// </summary>
        [Description("新增出库订单发送通知")]
        NewStorageOutSendNotice = 2,

        /// <summary>
        /// 新增入库订单接收通知
        /// </summary>
        [Description("新增入库订单接收通知")]
        NewStorageInRevNotice = 3,

        /// <summary>
        /// 新增出库订单接收通知
        /// </summary>
        [Description("新增出库订单接收通知")]
        NewStorageOutRevNotice = 4,

        /// <summary>
        /// 修改订单状态接收
        /// </summary>
        [Description("修改订单状态接收")]
        ChangeOrderStatusRequest = 5,

        /// <summary>
        /// 到货完成接收
        /// </summary>
        [Description("到货完成接收")]
        DeliveryCompleteRequest = 6,

        /// <summary>
        /// 到货完成处理
        /// </summary>
        [Description("到货完成处理")]
        DeliveryCompleteHandle = 7,

        /// <summary>
        /// 到货完成后发送通知
        /// </summary>
        [Description("到货完成后发送通知")]
        SendNoticeAfterDeliveryComplete = 8,

        /// <summary>
        /// 发送文件信息请求
        /// </summary>
        [Description("发送文件信息请求")]
        SendFileInfoInInNoticeReq = 9,

        /// <summary>
        /// 发送文件信息接收
        /// </summary>
        [Description("发送文件信息接收")]
        SendFileInfoInInNoticeRes = 10,

        /// <summary>
        /// Real修改订单状态接收
        /// </summary>
        [Description("Real修改订单状态接收")]
        RealChangeOrderStatusRequest = 11,

        /// <summary>
        /// 同步货运信息查询
        /// </summary>
        [Description("同步货运信息查询")]
        SyncOrderTransportQuery = 12,

        /// <summary>
        /// 同步库房费用信息
        /// </summary>
        [Description("同步库房费用信息")]
        SyncPayerLeft = 13,
    }
}
