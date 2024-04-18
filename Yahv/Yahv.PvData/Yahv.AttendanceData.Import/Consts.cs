using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.AttendanceData.Import
{
    /// <summary>
    /// 考勤记录
    /// </summary>
    public class AttendanceRecord
    {
        public const string Normal = "正常";
        public const string BeLate = "迟到";
        public const string EarlyLeave = "早退";
        /// <summary>
        /// be late and early leave
        /// </summary>
        public const string BL_EL = "迟到早退";
        public const string Overtime = "加班";

        public const string CasualLeave = "事假";
        public const string CasualLeave_AM = "事假(上午)";
        public const string CasualLeave_PM = "事假(下午)";

        public const string SickLeave = "病假";
        public const string SickLeave_AM = "病假(上午)";
        public const string SickLeave_PM = "病假(下午)";

        public const string PaidLeave = "带薪休假";
        public const string PaidLeave_AM = "带薪休假(上午)";
        public const string PaidLeave_PM = "带薪休假(下午)";

        /// <summary>
        /// official business
        /// </summary>
        public const string OB = "公务";
        public const string OB_AM = "公务(上午)";
        public const string OB_PM = "公务(下午)";

        public const string BusinessTrip = "公差";
        public const string BusinessTrip_AM = "公差(上午)";
        public const string BusinessTrip_PM = "公差(下午)";

        public const string MaternityLeave = "产假";

        /// <summary>
        /// supplementary signature
        /// </summary>
        public const string SS = "补签";
        public const string SS_AM = "补签(上午)";
        public const string SS_PM = "补签(下午)";
        /// <summary>
        /// legal holidays
        /// </summary>
        public const string LegalHolidays = "法定节假日";
        public const string PublicHoliday = "公休日";
        public const string Absenteeism = "旷工";
        /// <summary>
        /// System Authorizing
        /// </summary>
        public const string SA = "系统授权";
    }

    /// <summary>
    /// 上下午
    /// </summary>
    public class AmOrPm
    {
        public const string Am = "Am";
        public const string Pm = "Pm";
    }
}
