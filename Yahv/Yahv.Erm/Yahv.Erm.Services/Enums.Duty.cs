using Yahv.Underly.Attributes;

namespace Yahv.Erm.Services
{
    /// <summary>
    /// 询报价职务
    /// </summary>
    public enum Duty
    {
        /// <summary>
        /// 产品经理
        /// </summary>
        [Description("产品经理")]
        PM = 1,

        /// <summary>
        /// 产品经理助理
        /// </summary>
        [Description("产品经理助理")]
        PMa = 2,

        /// <summary>
        /// 销售
        /// </summary>
        [Description("销售")]
        Sales = 3,

        /// <summary>
        /// 采购
        /// </summary>
        [Description("采购")]
        Purchase = 4,

        /// <summary>
        /// 采纳人
        /// </summary>
        [Description("采纳人")]
        Offer = 5,
    }

    public enum PurchaceDuty
    {
        /// <summary>
        /// 产品经理
        /// </summary>
        [Description("产品经理")]
        PM = 1,

        /// <summary>
        /// 产品经理助理
        /// </summary>
        [Description("产品经理助理")]
        PMa = 2,
    }
}
