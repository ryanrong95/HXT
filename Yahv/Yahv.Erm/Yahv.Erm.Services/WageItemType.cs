using Yahv.Underly.Attributes;

namespace Yahv.Erm.Services
{
    public enum WageItemType
    {
        /// <summary>
        /// 普通列
        /// </summary>
        [Description("普通列")]
        Normal = 1,

        /// <summary>
        /// 计算列
        /// </summary>
        [Description("计算列")]
        Calc = 2,

        /// <summary>
        /// 累计收入
        /// </summary>
        [Description("累计收入")]
        AccumIncome = 10,

        /// <summary>
        /// 累计免税收入
        /// </summary>
        [Description("累计免税收入")]
        AccumFreeIncome = 11,

        /// <summary>
        /// 累计专项扣除（五险一金）
        /// </summary>
        [Description("累计专项扣除")]
        AccumSpecDeduction = 12,

        /// <summary>
        /// 累计专项附加扣除
        /// </summary>
        [Description("累计专项附加扣除")]
        AccumSpecAddDeduction = 13,

        /// <summary>
        /// 累计专项附加调整列
        /// </summary>
        [Description("累计专项附加调整列")]
        AccumSpecAddAdjustments = 14,

        /// <summary>
        /// 累计个税起征点调整
        /// </summary>
        [Description("累计个税起征点调整")]
        AccumPitAdjustments = 15,

        /// <summary>
        /// 累计预扣预缴应纳税所得额
        /// </summary>
        [Description("累计预扣预缴应纳税所得额")]
        AccumPayTaxableIncome = 16,

        /// <summary>
        /// 累计已预扣预缴税额(累计个税)
        /// </summary>
        [Description("累计已预扣预缴税额")]
        AccumPersonalIncomeTax = 17,

        /// <summary>
        /// 本月个税
        /// </summary>
        [Description("本月个税")]
        PersonalIncomeTax = 18,
    }
}
