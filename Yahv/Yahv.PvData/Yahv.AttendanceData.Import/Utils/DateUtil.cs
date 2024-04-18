using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.AttendanceData.Import.Utils
{
    /// <summary>
    /// 日期工具类
    /// </summary>
    public static class DateUtil
    {
        /// <summary>
        /// 上午的正常上班打卡时间
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public static DateTime GetAmStartTime(int year, int month, int day)
        {
            Random r = new Random();
            int hour = r.Next(8, 10);
            int minute = 0;
            if (hour == 8)
            {
                minute = r.Next(30, 60);
            }
            else
            {
                minute = r.Next(0, 10);
            }
            int second = r.Next(0, 60);
            int milliSecond = r.Next(0, 1000);

            return new DateTime(year, month, day, hour, minute, second, milliSecond);
        }

        /// <summary>
        /// 上午的正常下班打卡时间
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public static DateTime GetAmEndTime(int year, int month, int day)
        {
            Random r = new Random();
            int hour = r.Next(11, 13);
            int minute = 0;
            if (hour == 11)
            {
                minute = r.Next(50, 60);
            }
            else
            {
                minute = r.Next(0, 30);
            }
            int second = r.Next(0, 60);
            int milliSecond = r.Next(0, 1000);

            return new DateTime(year, month, day, hour, minute, second, milliSecond);
        }

        /// <summary>
        /// 下午的正常上班打卡时间
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public static DateTime GetPmStartTime(int year, int month, int day)
        {
            Random r = new Random();
            int hour = r.Next(12, 14);
            int minute = 0;
            if (hour == 12)
            {
                minute = r.Next(30, 60);
            }
            else
            {
                minute = r.Next(0, 10);
            }
            int second = r.Next(0, 60);
            int milliSecond = r.Next(0, 1000);

            return new DateTime(year, month, day, hour, minute, second, milliSecond);
        }

        /// <summary>
        /// 下午的正常下班打卡时间
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public static DateTime GetPmEndTime(int year, int month, int day)
        {
            Random r = new Random();
            int hour = r.Next(17, 19);
            int minute = 0;
            if (hour == 17)
            {
                minute = r.Next(50, 60);
            }
            else
            {
                minute = r.Next(0, 30);
            }
            int second = r.Next(0, 60);
            int milliSecond = r.Next(0, 1000);

            return new DateTime(year, month, day, hour, minute, second, milliSecond);
        }
    }
}
