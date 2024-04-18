using Needs.Underly.Attributes;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Underly
{
    /// <summary>
    /// 国际币种规范
    /// </summary>
    public enum Currency
    {
        [Curreny("Unkown", "Unkown", "Unkown")]
        [Description("未知")]
        Unkown = -1,

        /// <summary>
        /// 人民币
        /// </summary>
        [Curreny("CNY", "CNY¥", "¥")]
        [Description("人民币")]
        CNY = 1,

        /// <summary>
        /// 美元
        /// </summary>
        [Curreny("USD", "US$", "$")]
        [Description("美元")]
        USD = 2,

        /// <summary>
        /// 港币
        /// </summary>
        [Curreny("HKD", "HK$", "HK$")]
        [Description("港币")]
        HKD = 3,

        /// <summary>
        /// 欧元
        /// </summary>
        [Curreny("EUR", "€", "€")]
        [Description("欧元")]
        EUR = 4,

        /// <summary>
        /// 英镑
        /// </summary>
        [Curreny("GBP", "￡", "￡")]
        [Description("英镑")]
        GBP = 5,

        /// <summary>
        /// 日元
        /// </summary>
        [Curreny("JPY", "JPY¥", "¥")]
        [Description("日元")]
        JPY = 6,

        /// <summary>
        /// 澳元
        /// </summary>
        [Curreny("AUD", "A$", "A$")]
        [Description("澳元")]
        AUD = 7,

        /// <summary>
        /// 加元
        /// </summary>
        [Curreny("CAD", "C$", "C$")]
        [Description("加元")]
        CAD = 8,

        /// <summary>
        /// 新加坡元
        /// </summary>
        [Curreny("SGD", "S$", "S$")]
        [Description("新加坡元")]
        SGD = 9,

        /// <summary>
        /// 卢比
        /// </summary>
        [Curreny("INR", "₹", "₹")]
        [Description("卢比")]
        INR = 10,
    }


    /// <summary>
    /// 国际币种规范
    /// </summary>
    public enum CRMCurrency
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Curreny("Unknown", "Unknown", "Unknown", "Unknown")]
        [Description("未知")]
        Unknown = 0,

        /// <summary>
        /// 人民币
        /// </summary>
        [Curreny("元", "CNY", "CNY¥", "¥")]
        [Description("人民币")]
        CNY = 1,

        /// <summary>
        /// 美元
        /// </summary>
        [Curreny("美元", "USD", "US$", "$")]
        [Description("美元")]
        USD = 2,

        /// <summary>
        /// 港币
        /// </summary>
        [Curreny("港币", "HKD", "HK$", "HK$")]
        [Description("港币")]
        HKD = 3,

        /// <summary>
        /// 欧元
        /// </summary>
        /// <remarks>
        /// 报关等环节需要
        /// </remarks>
        [Curreny("欧元", "EUR", "€", "€")]
        [Description("欧元")]
        EUR = 4,

        /// <summary>
        /// 英镑
        /// </summary>
        /// <remarks>
        /// 报关等环节需要
        /// </remarks>
        [Curreny("英镑", "GBP", "￡", "￡")]
        [Description("英镑")]
        GBP = 5,

        /// <summary>
        /// 日元
        /// </summary>
        [Curreny("日元", "JPY", "JPY¥", "¥")]
        [Description("日元")]
        JPY = 6,

        /// <summary>
        /// 加元
        /// </summary>
        [Curreny("加拿大元", "CAD", "C$", "C$")]
        [Description("加元")]
        CAD = 8,

        /// <summary>
        /// 新加坡元
        /// </summary>
        /// <remarks>
        /// 报关等环节需要
        /// </remarks>
        [Curreny("新加坡元", "SGD", "S$", "$")]
        [Description("新加坡元")]
        SGD = 9,
        /// <summary>
        /// 瑞士法郎
        /// </summary>
        /// <remarks>
        /// 报关等环节需要
        /// </remarks>
        [Curreny("瑞士法郎", "CHF", "CH₣", "₣")]
        [Description("瑞士法郎")]
        CHF = 11,
    }
}
