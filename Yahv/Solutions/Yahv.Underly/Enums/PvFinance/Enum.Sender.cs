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
        /// 芯达通
        /// </summary>
        [Description("芯达通")]
        [Fixed("FSender001")]
        Xindatong,
    }
}