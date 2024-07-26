using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 代仓储主订单订单状态（应与客户端使用的枚举保持一致）
    /// </summary>
    public enum CgOrderStatus
    {
        [Description("暂存")]
        暂存 = 10,//客户保存

        [Description("挂起")]
        挂起 = 20,//客户挂起

        [Description("已提交")]
        已提交 = 100,//已提交

        [Description("待审核")]
        待审核 = 110,//报关业务提交到华芯通之后

        [Description("待确认")]
        待确认 = 150,//报价完成后等待客户确认

        [Description("待收货")]
        待收货 = 200, //向库房发入库通知

        [Description("已收货")]
        已收货 = 300, //仓储业务库房入库完成

        [Description("待报关")]
        待报关 = 310, //报关业务入库完成

        [Description("已申报")]
        已申报 = 400,//报关业务封箱完成

        [Description("待发货")]
        待发货 = 500,//发出库通知,

        [Description("已发货")]
        已发货 = 600, //仓库出库完成

        [Description("客户已收货")]
        客户已收货 = 700, //客户确认收货

        [Description("取消")]
        取消 = 999,
    }

    /// <summary>
    /// 国际币种规范(应与会员端使用币种保持一致)
    /// </summary>
    public enum WsOrderCurrency
    {
        /// <summary>
        /// 未知
        /// </summary>
        //[Curreny("Unknown", "Unknown", "Unknown", "Unknown")]
        [Description("未知")]
        Unknown = 0,

        /// <summary>
        /// 人民币
        /// </summary>
        //[Curreny("元", "CNY", "CNY¥", "¥")]
        [Description("人民币")]
        CNY = 1,

        /// <summary>
        /// 美元
        /// </summary>
        //[Curreny("美元", "USD", "US$", "$")]
        [Description("美元")]
        USD = 2,

        /// <summary>
        /// 港币
        /// </summary>
        //[Curreny("港币", "HKD", "HK$", "HK$")]
        [Description("港币")]
        HKD = 3,

        #region 暂时停用注释

        ///// <summary>
        ///// 欧元
        ///// </summary>
        //[Curreny("EUR", "€", "€")]
        //[Description("欧元")]
        //EUR = 4,

        ///// <summary>
        ///// 英镑
        ///// </summary>
        //[Curreny("GBP", "￡", "￡")]
        //[Description("英镑")]
        //GBP = 5,

        ///// <summary>
        ///// 日元
        ///// </summary>
        //[Curreny("JPY", "JPY¥", "¥")]
        //[Description("日元")]
        //JPY = 6,

        ///// <summary>
        ///// 澳元
        ///// </summary>
        //[Curreny("AUD", "A$", "A$")]
        //[Description("澳元")]
        //AUD = 7,

        ///// <summary>
        ///// 加元
        ///// </summary>
        //[Curreny("CAD", "C$", "C$")]
        //[Description("加元")]
        //CAD = 8,

        ///// <summary>
        ///// 新加坡元
        ///// </summary>
        //[Curreny("SGD", "S$", "S$")]
        //[Description("新加坡元")]
        //SGD = 9,

        ///// <summary>
        ///// 卢比
        ///// </summary>
        //[Curreny("INR", "₹", "₹")]
        //[Description("卢比")]
        //INR = 10,

        #endregion 
    }

}
