using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// Currency枚举类大部分币种注释掉了
    /// 临时创建该类用于中国银行外汇牌价实时汇率抓取和查询
    /// </summary>
    [Obsolete("Currency枚举类相关币种已取消注释，该类废弃")]
    public enum FerobocCurrency
    {
        /// <summary>
        /// 人民币
        /// </summary>
        [Description("人民币")]
        CNY = 1,

        /// <summary>
        /// 美元
        /// </summary>
        [Description("美元")]
        USD = 2,

        /// <summary>
        /// 港币
        /// </summary>
        [Description("港币")]
        HKD = 3,

        /// <summary>
        /// 欧元
        /// </summary>
        [Description("欧元")]
        EUR = 4,

        /// <summary>
        /// 英镑
        /// </summary>
        [Description("英镑")]
        GBP = 5,

        /// <summary>
        /// 日元
        /// </summary>
        [Description("日元")]
        JPY = 6,

        /// <summary>
        /// 澳元
        /// </summary>
        [Description("澳元")]
        AUD = 7,

        /// <summary>
        /// 加元
        /// </summary>
        [Description("加元")]
        CAD = 8,

        /// <summary>
        /// 新加坡元
        /// </summary>
        [Description("新加坡元")]
        SGD = 9,

        ///// <summary>
        ///// 卢比
        ///// </summary>
        //[Curreny("INR", "₹", "₹")]
        //[Description("卢比")]
        //INR = 10,

        /// <summary>
        /// 瑞士法郎
        /// </summary>
        [Description("瑞士法郎")]
        CHF = 11,
    }
}
