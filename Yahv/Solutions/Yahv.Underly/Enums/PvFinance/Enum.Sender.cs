using Yahv.Underly.Attributes;

namespace Yahv.Underly.Enums.PvFinance
{
    /// <summary>
    /// 接口发起人
    /// </summary>
    public enum SystemSender
    {
        /// <summary>
        /// 中心财务
        /// </summary>
        [Description("中心财务")]
        [Fixed("FSender000")]
        Center,

        /// <summary>
        /// 华芯通
        /// </summary>
        [Description("华芯通")]
        [Fixed("FSender001")]
        Xindatong,
    }
}