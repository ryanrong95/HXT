using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 所在地[大陆香港]
    /// </summary>
    public enum Range
    {
        /// <summary>
        /// 大陆
        /// </summary>
        [Description("大陆")]
        Local,
        /// <summary>
        /// 香港
        /// </summary>
        [Description("香港")]
        Overseas
    }

    /// <summary>
    /// 地域[国内国际]
    /// </summary>
    public enum AreaType
    {
        /// <summary>
        /// 国际
        /// </summary>
        [Description("国际")]
        International = 1,
        /// <summary>
        /// 国内
        /// </summary>
        [Description("国内")]
        domestic = 2
    }
}
