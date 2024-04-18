using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Utils.Converters
{
    public static class DateExtend
    {
        /// <summary>
        /// 时间处理
        /// </summary>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <param name="startDate"></param>
        /// <param name="EndDate"></param>
        public static void DateHandle(string starttime, string endtime, out DateTime? startDate, out DateTime? EndDate)
        {
            DateTime tempStartDate, tempEndDate;
            startDate = null;
            EndDate = null;
            if (DateTime.TryParse(starttime, out tempStartDate))
            {
                startDate = tempStartDate;
            }

            if (DateTime.TryParse(endtime, out tempEndDate))
            {
                EndDate = tempEndDate;
            }

            //两个时间都不为空时把小的时间做为开始时间
            if (startDate != null && EndDate != null)
            {
                if (DateTime.Compare((DateTime)startDate, (DateTime)EndDate) > 0)
                {
                    DateTime? tempDate = startDate;
                    startDate = EndDate;
                    EndDate = tempDate;
                }
            }

            //结束日期+1
            if (EndDate != null)
            {
                EndDate = EndDate.Value.AddDays(1);
            }
        }

        /// <summary>
        /// Linux时间戳
        /// </summary>
        /// <param name="dt">时间</param>
        /// <returns>Linux时间戳</returns>
        public static long LinuxTicks(this DateTime dt)
        {
            DateTime liunxStart = new DateTime(1970, 1, 1);
            return (dt - liunxStart).Ticks;
        }
    }
}
