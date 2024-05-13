using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace WinApp.Services
{

    /// <summary>
    /// 顺丰支付方式
    /// </summary>
    public enum PayMethod
    {
        /// <summary>
        /// 寄付
        /// </summary>
        [Description("寄付")]
        DeliveryPay = 1,

        /// <summary>
        /// 到付
        /// </summary>
        [Description("到付")]
        CollectPay = 2,

        /// <summary>
        /// 月结
        /// </summary>
        [Description("第三方付")]
        MonthlyPay = 3
    }

    /// <summary>
    /// 跨越支付方式（通用支付方式转化）
    /// </summary>
    public enum KYPayMethod
    {
        /// <summary>
        /// 寄付
        /// </summary>
        [Description("寄付")]
        DeliveryPay = 10,

        /// <summary>
        /// 到付
        /// </summary>
        [Description("到付")]
        CollectPay = 20,

        /// <summary>
        /// 月结
        /// </summary>
        [Description("第三方付")]
        MonthlyPay = 30
    }

    /// <summary>
    /// 顺丰运输类型（筛选出来的几种类型）
    /// </summary>
    public enum SFExpressTypeId
    {
        [Description("顺丰标快")]
        顺丰标快 = 1,
        [Description("顺丰特惠")]
        顺丰特惠 = 2,
        [Description("顺丰次晨")]
        顺丰次晨 = 5,
        [Description("顺丰即日")]
        顺丰即日 = 6,
        [Description("医药安心递")]
        医药安心递 = 11,
        [Description("医药专递")]
        医药专递 = 12,
        [Description("物流普运")]
        物流普运 = 13,
        [Description("物资配送")]
        物资配送 = 35,
        [Description("顺丰空配")]
        顺丰空配 = 112,
        [Description("专线普运")]
        专线普运 = 125,
        [Description("重货包裹")]
        重货包裹 = 154,
        [Description("小票零担")]
        小票零担 = 155,
        [Description("医药安心递_陆")]
        医药安心递_陆 = 195,
        [Description("顺丰微小件")]
        顺丰微小件 = 202,
        [Description("医药快运")]
        医药快运 = 203,
        [Description("陆运微小件")]
        陆运微小件 = 204,
        [Description("特惠专配")]
        特惠专配 = 208
    }

    /// <summary>
    /// 跨越服务方式（快递方式）
    /// </summary>
    public enum KYServiceMode
    {
        [Description("当天达")]
        当天达 = 10,
        [Description("次日达")]
        次日达 = 20,
        [Description("隔日达")]
        隔日达 = 30,
        [Description("陆运件")]
        陆运件 = 40,
        [Description("同城次日")]
        同城次日 = 50,
        [Description("次晨达")]
        次晨达 = 60,
        [Description("同城即日")]
        同城即日 = 70,
        [Description("航空件")]
        航空件 = 80,
        [Description("省内次日")]
        省内次日 = 160,
        [Description("省内即日")]
        省内即日 = 170,
        [Description("空运")]
        空运 = 210,
        [Description("专运")]
        专运 = 220
    }

    public enum PrintSource
    {
        [Description("顺丰")]
        SF=10,
        [Description("跨越速运")]
        KYSY = 20
    }
}
