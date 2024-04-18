using System;
using System.Collections.Generic;
using Needs.Utils.Descriptions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    public enum ClientCurrency
    {
        /// <summary>
        /// 未知
        /// </summary>       
        [Description("未知")]
        Unknown = 0,

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
        /// 港元
        /// </summary>      
        [Description("港币")]
        HKD = 3,

        /// <summary>
        /// 欧元
        /// </summary>
        /// <remarks>
        /// 报关等环节需要
        /// </remarks>      
        [Description("欧元")]
        EUR = 4,

        /// <summary>
        /// 英镑
        /// </summary>
        /// <remarks>
        /// 报关等环节需要
        /// </remarks>       
        [Description("英镑")]
        GBP = 5,

        /// <summary>
        /// 日元
        /// </summary>       
        [Description("日元")]
        JPY = 6,

        ///// <summary>
        ///// 澳元
        ///// </summary>
        //[Curreny("AUD", "A$", "A$")]
        //[Description("澳元")]
        //AUD = 7,

        /// <summary>
        /// 加元
        /// </summary>
        [Description("加元")]
        CAD = 8,

        /// <summary>
        /// 新加坡元
        /// </summary>
        /// <remarks>
        /// 报关等环节需要
        /// </remarks>        
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
        /// <remarks>
        /// 报关等环节需要
        /// </remarks>        
        [Description("瑞士法郎")]
        CHF = 11,
    }
}
