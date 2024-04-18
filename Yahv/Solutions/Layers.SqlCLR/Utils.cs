using System;
using System.Collections.Generic;
using System.Text;

namespace Layers.SqlCLR
{
    class Utils
    {
        /// <summary>
        /// 将.net 日期格式转换成Json日期格式
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string DateTimeToJson(DateTime dateTime)
        {
            DateTime d1 = new DateTime(1970, 1, 1);
            DateTime d2 = dateTime.ToUniversalTime();
            TimeSpan ts = new TimeSpan(d2.Ticks - d1.Ticks);
            return string.Format("\"Date({0})\"", ts.TotalMilliseconds.ToString("#"));
        }

        /// <summary>
        /// 返回消息值
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>消息值</returns>
        public static string GetValue(object value)
        {
            if (value == DBNull.Value)
            {
                return "null";
            }

            Type type = value.GetType();
            if (type == typeof(DateTime))
            {
                //return DateTimeToJson((DateTime)value);
                return string.Format("\"{0}\"", ((DateTime)value).GetDateTimeFormats('s')[0]);
            }

            if (type == typeof(string))
            {
                return string.Format("\"{0}\"", ((string)value).
                    Replace(@"\", @"\\").
                    Replace("\"", "\\\""));
            }

            if (type == typeof(Guid))
            {
                return string.Format("\"{0}\"", value.ToString());
            }

            if (type == typeof(bool))
            {
                return value.ToString().ToLower();
            }


            if (type.IsValueType)
            {
                return value.ToString();
            }

            return null;
        }
    }
}
