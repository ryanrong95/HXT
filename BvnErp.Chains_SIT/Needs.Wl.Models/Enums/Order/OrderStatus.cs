using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models.Enums
{
    /// <summary>
    /// 代理订单状态
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// 草稿
        /// </summary>
        [Description("草稿")]
        Draft = 0,

        /// <summary>
        /// 待归类（已下单）
        /// </summary>
        [Description("待归类")]
        Confirmed = 1,

        /// <summary>
        /// 待报价（已归类）
        /// </summary>
        [Description("待报价")]
        Classified = 2,

        /// <summary>
        /// 待客户确认（已报价）
        /// </summary>
        [Description("待客户确认")]
        Quoted = 3,

        /// <summary>
        /// 待报关（已客户确认）
        /// </summary>
        [Description("待报关")]
        QuoteConfirmed = 4,

        /// <summary>
        /// 待出库（已报关）
        /// </summary>
        [Description("待出库")]
        Declared = 5,

        /// <summary>
        /// 待收货（已出库）
        /// </summary>
        [Description("待收货")]
        WarehouseExited = 6,

        /// <summary>
        /// 完成
        /// </summary>
        [Description("完成")]
        Completed = 7,

        /// <summary>
        /// 已退回
        /// </summary>
        [Description("已退回")]
        Returned = 8,

        /// <summary>
        /// 已取消
        /// </summary>
        [Description("已取消")]
        Canceled = 9
    }
}
