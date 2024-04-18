using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.AttendanceData.Import.Models
{
    /// <summary>
    /// 员工哺乳假配置
    /// </summary>
    public class BreastfeedingLeave
    {
        /// <summary>
        /// 员工姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 员工AdminID
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 哺乳假开始日期
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 哺乳假结束日期
        /// </summary>
        public DateTime EndDate { get; set; }
    }
}
