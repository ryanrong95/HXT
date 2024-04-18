using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 科目类型
    /// </summary>
    public enum SubjectType
    {
        /// <summary>
        /// 应收
        /// </summary>
        [Description("应收")]
        Input,

        /// <summary>
        /// 应付
        /// </summary>
        [Description("应付")]
        Output
    }
}
