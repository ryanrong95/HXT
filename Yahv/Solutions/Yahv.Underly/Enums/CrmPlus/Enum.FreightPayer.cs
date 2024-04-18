using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    
    public enum FreightPayer {
        /// <summary>
        /// 我方
        /// </summary>
        [Description("我方")]
        Own = 0,
        /// <summary>
        /// 对方
        /// </summary>
        [Description("对方")]
        Other = 1,
    }
}
