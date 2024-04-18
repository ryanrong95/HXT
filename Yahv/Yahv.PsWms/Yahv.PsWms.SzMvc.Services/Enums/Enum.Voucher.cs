using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PsWms.SzMvc.Services.Enums
{
    /// <summary>
    /// 账目来源
    /// </summary>
    public enum AccountSource
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

    /// <summary>
    /// 业务类型
    /// </summary>
    public enum Conduct
    {
        /// <summary>
        /// 代仓储, 仓储服务：入库为代入库, 出库为代出库
        /// </summary>
        [Description("代仓储")]
        Warehousings = 1,

        /// <summary>
        /// 贸易, 入库为采购;出库为销售
        /// </summary>
        [Description("贸易")]
        Tradings = 2,

        /// <summary>
        /// 售后, 售后服务：入库为退货, 出库为补货, 退换货就以上两个过程
        /// </summary>
        [Description("售后")]
        PostSale = 3,
    }

    public enum VoucherType
    {
        [Description("及时账单")]
        Timely = 1,

        [Description("月结账单")]
        Monthly = 2,

        [Description("其它账单")]
        Other = 4,
    }

    public enum VoucherMode
    {
        [Description("应收账单")]
        Receivables = 1,

        [Description("应付账单")]
        Payables = 2,
    }
}
