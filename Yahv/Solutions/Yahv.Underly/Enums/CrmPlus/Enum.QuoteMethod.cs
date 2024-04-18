using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    
    /// <summary>
    /// 询价议价方式CrmPlus
    /// </summary>
    public enum QuoteMethod
    {
        /// <summary>
        /// Email
        /// </summary>
        [Description("Email")]
        Email = 1,
        /// <summary>
        /// 电话
        /// </summary>
        [Description("电话")]
        Phone = 2,
        /// <summary>
        /// 线上
        /// </summary>
        [Description("线上")]
        Online = 3
    }
 
}
