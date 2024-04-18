using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.AttendanceData.Import.Models
{
    /// <summary>
    /// 芯达通员工
    /// </summary>
    public class XdtStaff
    {
        /// <summary>
        /// 员工ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 大赢家编码
        /// </summary>
        public string DyjCode { get; set; }

        /// <summary>
        /// 大赢家UserID
        /// </summary>
        public string DyjID { get; set; }

        /// <summary>
        /// 员工姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 部门类型
        /// </summary>
        public DepartmentType DepartmentType { get; set; }

        /// <summary>
        /// 职务类型
        /// </summary>
        public PostType PostType { get; set; }

        /// <summary>
        /// 员工的AdminID
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 入职日期
        /// </summary>
        public DateTime HireDate { get; set; }

        /// <summary>
        /// 离职日期
        /// </summary>
        public DateTime? LeaveDate { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        public string RegionID { get; set; }

        /// <summary>
        /// 班别
        /// </summary>
        public string SchedulingID { get; set; }
    }
}
