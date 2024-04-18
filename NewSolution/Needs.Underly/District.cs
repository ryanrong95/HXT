using Needs.Underly.Attributes;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Underly
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
        [District("Unknown", "Unknown", "Unknown", "Unknown")]
        Unknown = -1,

        [Description("全球")]
        [District("Global", "Globalization", "", "Globals")]
        Global = 0,

        [Description("中国")]
        [District("China", "People's Republic of China", "cn", "中国")]
        CN = 1,

        [Description("中国香港")]
        [District("HK", "Hong Kong", "hk", "中國香港")]
        HK = 2,

        [Description("印度")]
        [District("India", "Republic of India", "in", "India")]
        IN = 3,

        [Description("美国")]
        [District("United States", "United States of America", "CN", "America")]
        US = 4,
    }
}
