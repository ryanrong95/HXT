using NtErp.Wss.Sales.Services.Utils.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly
{
    /// <summary>
    /// 国际地区规范
    /// </summary>
    public enum District
    {
        /// <summary>
        /// 主要是告知前端强行要求用户选择已知区域
        /// </summary>
        [Naming("Unknown")]
        Unknown = -1,

        [Naming("Globalization")]
        Global = 0,

        [Naming("China")]
        CN,
        [Naming("Hongkong")]
        HK,
        [Naming("India")]
        IN,
        [Naming("United States of America")]
        US,
    }
}
