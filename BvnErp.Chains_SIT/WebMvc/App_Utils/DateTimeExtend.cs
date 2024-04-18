using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Needs.Wl.Web.Mvc.Utils
{
    public static class DateTimeExtend
    {
        /// <summary>
        /// 获取日期的星期
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string GetWeekName(this DateTime date)
        {
            string weekName = "";
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    weekName = "周一";
                    break;
                case DayOfWeek.Tuesday:
                    weekName = "周二";
                    break;
                case DayOfWeek.Wednesday:
                    weekName = "周三";
                    break;
                case DayOfWeek.Thursday:
                    weekName = "周四";
                    break;
                case DayOfWeek.Friday:
                    weekName = "周五";
                    break;
                case DayOfWeek.Saturday:
                    weekName = "周六";
                    break;
                case DayOfWeek.Sunday:
                    weekName = "周日";
                    break;
            }
            return weekName;
        }
    }
}