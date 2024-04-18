using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 国际地区规范
    /// </summary>
    public enum District
    {

        /// <summary>
        /// 主要是告知前端强行要求用户选择已知区域
        /// </summary>
        [Description("未知")]
        [District("Unknown", "Unknown", "Unknown", "Unknown", "未知")]
        Unknown = 0,



        /// <summary>
        /// 中国
        /// </summary>
        [Description("中国")]
        [District("China", "People's Republic of China", "cn", "中国", "中国")]
        CN = 1,

        /// <summary>
        /// 中国香港
        /// </summary>
        [Description("中国香港")]
        [District("HK", "Hong Kong", "hk", "中國香港", "中国香港")]
        HK = 2,

        /// <summary>
        /// 印度
        /// </summary>
        [Description("印度")]
        [District("India", "Republic of India", "in", "India", "印度")]
        IN = 3,

        /// <summary>
        /// 美国
        /// </summary>
        [Description("美国")]
        [District("United States", "United States of America", "CN", "America", "美国")]
        US = 4,

        /// <summary>
        /// 全球
        /// </summary>
        [Description("全球")]
        [District("Global", "Globalization", "", "Globals", "全球")]
        Global = int.MaxValue,
    }
}
