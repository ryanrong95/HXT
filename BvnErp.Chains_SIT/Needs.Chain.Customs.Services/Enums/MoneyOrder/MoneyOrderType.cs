using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    public enum MoneyOrderType
    {
        /// <summary>
        /// 银行承兑
        /// </summary>
        [Description("银行承兑")]
        Bank = 1,

        /// <summary>
        /// 商业承兑
        /// </summary>
        [Description("商业承兑")]
        Commercial = 2,
    }

    public enum MoneyOrderNature
    {
        /// <summary>
        /// 电子版
        /// </summary>
        [Description("电子版")]
        Electronic = 1,
        /// <summary>
        /// 纸质版
        /// </summary>
        [Description("纸质版")]
        Payper = 2
    }

    public enum MoneyOrderStatus
    {
        /// <summary>
        /// 未兑换
        /// </summary>
        [Description("未兑换")]
        Ticketed = 20,

        /// <summary>
        /// 已兑换
        /// </summary>
        [Description("已兑换")]
        Exchanged = 200,

        /// <summary>
        /// 已转让
        /// </summary>
        [Description("已转让")]
        Transferred = 201,
    }
}
