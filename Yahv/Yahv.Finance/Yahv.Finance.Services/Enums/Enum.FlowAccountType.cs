using Yahv.Underly.Attributes;

namespace Yahv.Finance.Services.Enums
{
    /// <summary>
    /// 流水表账户类型
    /// </summary>
    public enum FlowAccountType
    {
        /// <summary>
        /// 银行流水账户
        /// </summary>
        [Description("银行流水账户")]
        BankStatement = 1,

        /// <summary>
        /// 汇票账户
        /// </summary>
        [Description("汇票账户")]
        MoneyOrder = 2,
    }
}