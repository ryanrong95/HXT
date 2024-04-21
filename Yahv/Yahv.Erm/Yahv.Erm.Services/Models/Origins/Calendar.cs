using System;
using Yahv.Linq;

namespace Yahv.Erm.Services.Models.Origins
{
    public class Calendar
    {
        #region 属性
        /// <summary>
        /// 日期（2020-04-01）
        /// </summary>
        public DateTime ID { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        /// <summary>
        /// 周几 0开始
        /// </summary>
        public int Week { get; set; }
        #endregion

        #region 扩展属性
        /// <summary>
        /// 字符串日期
        /// </summary>
        public string Date => ID.ToString("yyyy-MM-dd");

        #endregion
    }
}