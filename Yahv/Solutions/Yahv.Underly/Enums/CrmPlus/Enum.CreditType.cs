using Yahv.Underly.Attributes;

namespace Yahv.Underly
{

    /// <summary>
    /// 询价议价方式CrmPlus
    /// </summary>
    public enum CreditType
    {
        /// <summary>
        /// 授予方
        /// </summary>
        [Description("授予方")]
        GrantingParty = 1,
        /// <summary>
        /// 信用接收方
        /// </summary>
        [Description("信用接收方")]
        CreditReceiver = 2,

    }

}
