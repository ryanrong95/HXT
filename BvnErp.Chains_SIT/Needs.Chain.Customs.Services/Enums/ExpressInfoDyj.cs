using System;
using Needs.Utils.Descriptions;

namespace Needs.Ccs.Services.Enums
{
    public enum DyjExpressCompany
    {
        /// <summary>
        /// 顺丰
        /// </summary>
        [Description("顺丰")]
        KDN_SF=0,

        /// <summary>
        /// EMS
        /// </summary>
        [Description("EMS")]
        EMS=1
    }

    public enum SFType
    {
        /// <summary>
        /// 顺丰次日
        /// </summary>
        [Description("顺丰次日")]
        NextDay = 1,

        /// <summary>
        /// 顺丰隔日
        /// </summary>
        [Description("顺丰隔日")]
        GapDay = 2,

        /// <summary>
        /// 顺丰次日
        /// </summary>
        [Description("顺丰次晨")]
        NextMorning = 5,

        /// <summary>
        /// 顺丰即日
        /// </summary>
        [Description("顺丰即日")]
        CurrentDay = 6,
    }

    public enum EMSType
    {
        /// <summary>
        /// 标准快递
        /// </summary>
        [Description("标准快递")]
        Normal = 1,

        /// <summary>
        /// 代收到付
        /// </summary>
        [Description("代收到付")]
        PayAfter = 8,

        /// <summary>
        /// 标准快递
        /// </summary>
        [Description("快递包裹")]
        Package = 9,
    }
}
