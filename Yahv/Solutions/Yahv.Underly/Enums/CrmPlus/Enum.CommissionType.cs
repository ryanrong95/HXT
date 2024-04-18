using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
   
    /// <summary>
    /// 返款类型
    /// </summary>
    public enum CommissionType {
        /// <summary>
        /// 返点
        /// </summary>
        [Description("返点")]
        Rebate = 1,
        /// <summary>
        /// 折扣
        /// </summary>
       [Description("折扣")]
        Discount = 2
    }

}
