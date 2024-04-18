using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 订单服务类型
    /// </summary>
    public enum LsOrderType
    {
        [Description("租赁")]
        Lease = 1,

        [Description("贸易")]
        Trading = 2,

        [Description("仓储服务")]
        WarehouseServicing = 3,
    }

    /// <summary>
    /// 订单来源
    /// </summary>
    public enum LsOrderSource
    {
        [Description("代仓储")]
        WarehouseServicing = 1,

        [Description("传统贸易")]
        Trading = 2,
    }

    /// <summary>
    /// 租赁状态
    /// </summary>
    public enum LsStatus
    {
        [Description("到期")]
        Expire = 1,

        [Description("存续")]
        Subsist = 2,
    }

    /// <summary>
    /// 租赁订单状态
    /// </summary>
    public enum LsOrderStatus
    {
        [Description("待支付")]
        Unpaid = 1,

        [Description("待分配")]
        UnAllocate = 2,

        [Description("已分配")]
        Allocated = 3,

        [Description("已到期")]
        Expired = 4,

        [Description("已关闭")]
        Closed = 5,
    }
    public enum LsOrderStatusType
    {
        [Description("主状态")]
        MainStatus = 1,

        [Description("开票状态")]
        InvoiceStatus = 4,
    }
}