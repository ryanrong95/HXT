using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Kdn.Library
{
    /// <summary>
    /// 计费方式
    /// </summary>
    public enum PayType
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
        [Description("月结")]
        MonthlyPay = 3,

        /// <summary>
        /// 第三方月结
        /// </summary>
        [Description("第三方月结")]
        ThirdParty = 4,
    }

    /// <summary>
    /// EMS付款方式
    /// </summary>
    public enum PayTypeForEMS
    {
        [Description("寄件人")]
        DeliveryPay = 1,

        [Description("收件人")]
        CollectPay = 2,     
    }


   

    /// <summary>
    /// 包装类型
    /// </summary>
    public enum PackingType
    {
        /// <summary>
        /// 纸
        /// </summary>
        Paper = 0,
        /// <summary>
        /// 纤
        /// </summary>
        Fiber = 1,

        /// <summary>
        /// 木
        /// </summary>
        Wood = 2,
        /// <summary>
        /// 托膜
        /// </summary>
        Membrane = 3,
        /// <summary>
        /// 木托
        /// </summary>
        WoodPallet = 4,
        /// <summary>
        /// 托膜
        /// </summary>
        Others = 99,
    }

    public enum DeliveryMethod
    {
        /// <summary>
        /// 自提
        /// </summary>
        Self = 0,
        /// <summary>
        /// 送货上门
        /// </summary>
        Door = 1,
        /// <summary>
        /// 送货上楼
        /// </summary>
        Floor = 2
    }
}
