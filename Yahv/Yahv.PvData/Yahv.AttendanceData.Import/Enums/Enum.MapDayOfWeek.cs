using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.AttendanceData.Import
{
    /// <summary>
    /// 星期枚举, 与System.DayOfWeek对应
    /// </summary>
    public enum MapDayOfWeek
    {
        /// <summary>
        /// 星期日
        /// </summary>
        [Description("星期日")]
        Sunday = 0,

        /// <summary>
        /// 星期一
        /// </summary>
        [Description("星期一")]
        Monday = 1,

        /// <summary>
        /// 星期二
        /// </summary>
        [Description("星期二")]
        Tuesday = 2,

        /// <summary>
        /// 星期三
        /// </summary>
        [Description("星期三")]
        Wednesday = 3,

        /// <summary>
        /// 星期四
        /// </summary>
        [Description("星期四")]
        Thursday = 4,

        /// <summary>
        /// 星期五
        /// </summary>
        [Description("星期五")]
        Friday = 5,

        /// <summary>
        /// 星期六
        /// </summary>
        [Description("星期六")]
        Saturday = 6
    }
}
