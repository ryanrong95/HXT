using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.AttendanceData.Import.Extends
{
    /// <summary>
    /// 枚举扩展类
    /// </summary>
    public static class EnumExtend
    {
        /// <summary>
        /// DayOfWeek扩展方法
        /// </summary>
        /// <param name="dayOfWeek">星期几</param>
        /// <returns></returns>
        public static string GetDescription(this DayOfWeek dayOfWeek)
        {
            MapDayOfWeek week = (MapDayOfWeek)Enum.Parse(typeof(MapDayOfWeek), dayOfWeek.ToString());
            return week.GetDescription();
        }
    }
}
