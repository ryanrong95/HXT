using System;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 结算方式CrmPlus
    /// </summary>
    public enum SettlementMode
    {
        //账期，现款，预付款
        /// k<summary>
        /// 账期
        /// </summary>
        [Description("账期")]
        AccountPeriod = 1,
        /// <summary>
        /// 现款
        /// </summary>
        [Description("现款")]
        Cash = 2,
        /// k<summary>
        /// 现款
        /// </summary>
        [Description("预付款")]
        AdvancePayment = 3
    }
}
