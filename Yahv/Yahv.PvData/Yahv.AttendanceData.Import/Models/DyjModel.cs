using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.AttendanceData.Import.Models
{
    public class DyjResponse<T>
    {
        public int errCod { get; set; }

        public string errMsg { get; set; }

        public List<T> data { get; set; }
    }

    /// <summary>
    /// 大赢家考勤信息
    /// </summary>
    public class DyjModel
    {
        public string UserID { get; set; }

        public DateTime WorkBeginDate { get; set; }

        public DateTime WorkEndDate { get; set; }

        public string BeginIP { get; set; }

        public string EndIP { get; set; }
    }

    public class QingJia
    {
        public string EmployeeID { get; set; }

        public string EmployeeName { get; set; }

        public string Stype { get; set; }

        public string STATE { get; set; }

        public DateTime sdate { get; set; }

        /// <summary>
        /// 0:整天 1:上午 2:下午
        /// </summary>
        public int bantian { get; set; }
    }
}
