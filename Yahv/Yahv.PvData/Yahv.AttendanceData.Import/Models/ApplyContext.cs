using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Enums;
using Yahv.Utils.Serializers;

namespace Yahv.AttendanceData.Import.Models
{
    #region 加班申请内容

    /// <summary>
    /// 加班申请内容
    /// </summary>
    public class OverTimeContext
    {
        /// <summary>
        /// 加班日期
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 部门负责人
        /// </summary>
        public string ApproveID { get; set; }

        /// <summary>
        /// 负责人名称
        /// </summary>
        public string ApproveName { get; set; }

        /// <summary>
        /// 部门编码
        /// </summary>
        public string DepartmentCode { get; set; }

        /// <summary>
        /// 加班原因
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 加班兑换方式
        /// </summary>
        public OvertimeExchangeType OvertimeExchangeType { get; set; }

        public string ToJson(DateTime date, string approveID, string approveName, string code, string reason)
        {
            this.Date = date;
            this.ApproveID = approveID;
            this.ApproveName = approveName;
            this.DepartmentCode = code;
            this.Reason = reason;
            this.OvertimeExchangeType = OvertimeExchangeType.PayDay;

            return this.Json();
        }
    }

    #endregion

    #region 请假申请内容

    /// <summary>
    /// 请假申请内容
    /// </summary>
    public class OffTimeContext
    {
        /// <summary>
        /// 请假类型
        /// </summary>
        public SchedulePrivateType LeaveType { get; set; }

        /// <summary>
        /// 部门负责人
        /// </summary>
        public string ApproveID { get; set; }

        /// <summary>
        /// 负责人名称
        /// </summary>
        public string ApproveName { get; set; }

        /// <summary>
        /// 部门编码
        /// </summary>
        public string DepartmentCode { get; set; }

        /// <summary>
        /// 请假原因
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 公差原因
        /// </summary>
        public BusinessTripReason? BusinessReason { get; set; }

        /// <summary>
        /// 是否借款
        /// </summary>
        public LoanOrNot LoanOrNot { get; set; }

        /// <summary>
        /// 请假日期项
        /// </summary>
        public List<OffTimeDateItem> DateItems { get; set; }

        /// <summary>
        /// 请假日程项
        /// </summary>
        public List<OffTimeScheduleItem> ScheduleItems { get; set; }

        /// <summary>
        /// 请假天数
        /// </summary>
        public decimal Days { get; set; }

        /// <summary>
        /// 请假日期
        /// </summary>
        public string Dates { get; set; }

        public string ToJson(SchedulePrivateType spType, string approveID, string approveName, string code, decimal days, DateTime date, DateLengthType dlType)
        {
            this.LeaveType = spType;
            this.ApproveID = approveID;
            this.ApproveName = approveName;
            this.DepartmentCode = code;
            this.Reason = "";
            if (spType == SchedulePrivateType.OfficialBusiness || spType == SchedulePrivateType.BusinessTrip)
                this.BusinessReason = BusinessTripReason.Others;
            this.LoanOrNot = LoanOrNot.Not;
            this.DateItems = new List<OffTimeDateItem>()
            {
                new OffTimeDateItem() { Date = date, Type = dlType, StartTime = null, EndTime = null }
            };
            this.ScheduleItems = new List<OffTimeScheduleItem>();
            this.Days = days;
            this.Dates = date.ToString("yyyyMMdd");

            return this.Json();
        }
    }

    public class OffTimeDateItem
    {
        /// <summary>
        /// 请假日期
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 请假时长类型
        /// </summary>
        public DateLengthType Type { get; set; }

        /// <summary>
        /// 请假开始时间
        /// </summary>
        public TimeSpan? StartTime { get; set; }

        /// <summary>
        /// 请假结束时间
        /// </summary>
        public TimeSpan? EndTime { get; set; }
    }

    public class OffTimeScheduleItem
    {
    }

    #endregion

    #region 补签申请内容

    /// <summary>
    /// 补签申请内容
    /// </summary>
    public class ReSignContext
    {
        /// <summary>
        /// 补签日期
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 部门负责人
        /// </summary>
        public string ApproveID { get; set; }

        /// <summary>
        /// 负责人名称
        /// </summary>
        public string ApproveName { get; set; }

        /// <summary>
        /// 部门编码
        /// </summary>
        public string DepartmentCode { get; set; }

        /// <summary>
        /// 上午上班
        /// </summary>
        public bool AmOn { get; set; }

        /// <summary>
        /// 上午下班
        /// </summary>
        public bool AmOff { get; set; }

        /// <summary>
        /// 下午上班
        /// </summary>
        public bool PmOn { get; set; }

        /// <summary>
        /// 下午下班
        /// </summary>
        public bool PmOff { get; set; }

        public string ToJson(DateTime date, string approveID, string approveName, string code, bool amOn, bool amOff, bool pmOn, bool pmOff)
        {
            this.Date = date;
            this.ApproveID = approveID;
            this.ApproveName = approveName;
            this.DepartmentCode = code;
            this.AmOn = AmOn;
            this.AmOff = AmOff;
            this.PmOn = PmOn;
            this.PmOff = PmOff;
            return this.Json();
        }
    }

    #endregion
}
