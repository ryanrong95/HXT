using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.AttendanceData.Import.Models
{
    /// <summary>
    /// 考勤记录
    /// </summary>
    public class Attendance
    {
        /// <summary>
        /// 员工ID
        /// </summary>
        public string StaffID { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 大赢家编码
        /// </summary>
        public string DyjCode { get; set; }

        /// <summary>
        /// 区域ID
        /// </summary>
        public string RegionID { get; set; }

        /// <summary>
        /// 班别ID
        /// </summary>
        public string SchedulingID { get; set; }

        /// <summary>
        /// 考勤日期
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 实际考勤结果
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// 计算出的上午考勤结果
        /// </summary>
        public string Result_Am { get; set; }

        /// <summary>
        /// 计算出的下午考勤结果
        /// </summary>
        public string Result_Pm { get; set; }

        /// <summary>
        /// 实际打卡时间
        /// </summary>
        public DateTime? Date_Start { get; set; }
        public DateTime? Date_End { get; set; }

        /// <summary>
        /// 打卡IP
        /// </summary>
        public string BeginIP { get; set; }
        public string EndIP { get; set; }

        /// <summary>
        /// 计算出的上午打卡时间
        /// </summary>
        public DateTime? Date_Am_Start { get; set; }
        public DateTime? Date_Am_End { get; set; }

        /// <summary>
        /// 计算出的下午打卡时间
        /// </summary>
        public DateTime? Date_Pm_Start { get; set; }
        public DateTime? Date_Pm_End { get; set; }
    }
}
