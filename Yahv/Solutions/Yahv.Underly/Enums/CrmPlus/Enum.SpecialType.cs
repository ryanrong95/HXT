using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    //特殊类型
    /// </summary>
    public enum SpecialType
    {
        /// <summary>
        /// 终端
        /// </summary>
        [Description("标签")]
        Lable = 1,
        /// <summary>
        /// 贸易
        /// </summary>
        [Description("包装")]
        Wrap = 2,
        /// <summary>
        /// OEM
        /// </summary>
        [Description("质量")]
        Quality = 3,
        [Description("温度")]
        Temperature = 4,

        [Description("交货期")]
        Ddate = 5,

        [Description("批次")]
        Batch = 9,

        [Description("开票要求")]
        invoice=10
    }
}
