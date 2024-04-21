using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Usually;
using System;
using Yahv.Utils.Converters.Contents;
using Layers.Data;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using System.Collections.Generic;
using Yahv.Erm.Services.Views;
using Yahv.Services.Models;
using Yahv.Erm.Services.Interfaces;
using System.Configuration;
using NPOI.OpenXmlFormats.Dml.Diagram;
using Yahv.Erm.Services.Models.Rolls;
using Yahv.Erm.Services.Common;

namespace Yahv.Erm.Services.Models.Origins
{
    #region 加班申请内容
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
        /// 部门名称
        /// </summary>
        public string DepartmentName
        {
            get
            {
                if (string.IsNullOrEmpty(this.DepartmentCode))
                {
                    return "";
                }
                else
                {
                    return ((DepartmentType)Enum.Parse(typeof(DepartmentType), this.DepartmentCode)).GetDescription();
                }
            }
        }

        /// <summary>
        /// 加班原因
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 加班兑换方式
        /// </summary>
        public OvertimeExchangeType OvertimeExchangeType { get; set; }
    }
    #endregion

    #region 请假申请内容
    public class OffTimeContext
    {
        /// <summary>
        /// 请假类型
        /// </summary>
        public Underly.Enums.LeaveType LeaveType { get; set; }

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
        /// 部门名称
        /// </summary>
        public string DepartmentName
        {
            get
            {
                if (string.IsNullOrEmpty(this.DepartmentCode))
                {
                    return "";
                }
                else
                {
                    return ((DepartmentType)Enum.Parse(typeof(DepartmentType), this.DepartmentCode)).GetDescription();
                }
            }
        }

        /// <summary>
        /// 请假原因
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 工作交接人
        /// </summary>
        public string SwapStaff { get; set; }

        /// <summary>
        /// 工作内容描述
        /// </summary>
        public string WorkContext { get; set; }

        /// <summary>
        /// 公差原因
        /// </summary>
        public BusinessTripReason? BusinessReason { get; set; }

        /// <summary>
        /// 出差随行人员
        /// </summary>
        public string Entourage { get; set; }

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

        #region 扩展属性

        /// <summary>
        /// 请假天数
        /// </summary>
        public decimal Days
        {
            get
            {
                decimal day1 = this.DateItems.Where(item => item.Type == DateLengthType.AllDay).Count();
                decimal day2 = this.DateItems.Where(item => item.Type != DateLengthType.AllDay).Count();
                return day1 + day2 / 2;
            }
        }

        /// <summary>
        /// 请假日期
        /// </summary>
        public string Dates
        {
            get
            {
                var date = this.DateItems.OrderBy(item => item.Date).Select(item => item.Date.ToString("yyyy-MM-dd")).ToArray();
                if (date.Count() >= 10)
                {
                    return date.First() + "  至  " + date.Last();//注：只是展示可能不准确。
                }
                else
                {
                    return string.Join(";", date);
                }
            }
        }

        /// <summary>
        /// 请假原因
        /// </summary>
        public string ApplyReason
        {
            get
            {
                if (this.LeaveType == Underly.Enums.LeaveType.BusinessTrip || this.LeaveType == Underly.Enums.LeaveType.OfficialBusiness)
                {
                    if (this.BusinessReason != null)
                    {
                        return this.BusinessReason.GetDescription();
                    }
                    else
                    {
                        return BusinessTripReason.VisitClient.GetDescription();
                    }
                }
                else
                {
                    return this.Reason;
                }
            }
        }
        #endregion
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
        /// <summary>
        /// 出发日期
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 到达日期
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 出发地点
        /// </summary>
        public string StartPlace { get; set; }
        /// <summary>
        /// 到达地点
        /// </summary>
        public string EndPlace { get; set; }
        /// <summary>
        /// 交通工具
        /// </summary>
        public string Vehicle { get; set; }
        /// <summary>
        /// 交通花费
        /// </summary>
        public decimal VehicleCost { get; set; }
        /// <summary>
        /// 住宿天数
        /// </summary>
        public int StayDay { get; set; }
        /// <summary>
        /// 公司名称（客户、供应商）
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 拜访人员
        /// </summary>
        public string Person { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string Department { get; set; }
        /// <summary>
        /// 岗位名称
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 出差事由
        /// </summary>
        public string BusinessReason { get; set; }
    }
    #endregion

    #region 补签申请内容
    public class ReSignContext
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
        /// 部门名称
        /// </summary>
        public string DepartmentName
        {
            get
            {
                if (string.IsNullOrEmpty(this.DepartmentCode))
                {
                    return "";
                }
                else
                {
                    return ((DepartmentType)Enum.Parse(typeof(DepartmentType), this.DepartmentCode)).GetDescription();
                }
            }
        }

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

        #region 扩展属性

        public List<TimeSpan> Attends { get; set; }

        public string AttendStr
        {
            get
            {
                if (this.Attends == null || this.Attends.Count == 0)
                {
                    return "--";
                }
                else
                {
                    var min = Attends.Min();
                    var max = Attends.Max();
                    return min.ToString(@"hh\:mm\:ss") + " - " + max.ToString(@"hh\:mm\:ss");
                }
            }
        }

        /// <summary>
        /// 本月已申请补签次数
        /// </summary>
        public int ReSignTimes { get; set; }

        #endregion
    }
    #endregion

    #region 招聘申请内容
    public class RecruitContext
    {
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
        /// 部门名称
        /// </summary>
        public string DepartmentName
        {
            get
            {
                if (string.IsNullOrEmpty(this.DepartmentCode))
                {
                    return "";
                }
                else
                {
                    return ((DepartmentType)Enum.Parse(typeof(DepartmentType), this.DepartmentCode)).GetDescription();
                }
            }
        }

        /// <summary>
        /// 招聘岗位名称
        /// </summary>
        public string PostionName { get; set; }

        /// <summary>
        /// 工作地点
        /// </summary>
        public string WorkAddress { get; set; }

        /// <summary>
        /// 需求人数
        /// </summary>
        public string NumberOfNeeds { get; set; }

        /// <summary>
        /// 现有岗位数量
        /// </summary>
        public string NumberOfPositions { get; set; }

        /// <summary>
        /// 现有在岗人数
        /// </summary>
        public string NumberOfNow { get; set; }

        /// <summary>
        /// 拟招聘人数
        /// </summary>
        public string NumberOfRecruiters { get; set; }

        /// <summary>
        /// 试用期薪资
        /// </summary>
        public string PeriodSalary { get; set; }

        /// <summary>
        /// 转正后薪资
        /// </summary>
        public string NormalSalary { get; set; }

        /// <summary>
        /// 期望到岗时间
        /// </summary>
        public DateTime ExpectedArrivalTime { get; set; }

        /// <summary>
        /// 招聘途经
        /// </summary>
        public RecruitmentRoute RecruitmentRoute { get; set; }

        /// <summary>
        /// 招聘原因
        /// </summary>
        public RecruitmentReason RecruitmentReason { get; set; }

        /// <summary>
        /// 出差需求
        /// </summary>
        public string BussnessTripRequirement { get; set; }

        /// <summary>
        /// 需求等级
        /// </summary>
        public string EmergentRequirement { get; set; }

        /// <summary>
        /// 性别要求
        /// </summary>
        public string GenderRequirement { get; set; }

        /// <summary>
        /// 学历要求
        /// </summary>
        public string EducationRequirement { get; set; }

        /// <summary>
        /// 年龄要求
        /// </summary>
        public string AgeRequirement { get; set; }

        /// <summary>
        /// 专业要求
        /// </summary>
        public string MajorRequirement { get; set; }

        /// <summary>
        /// 经验要求
        /// </summary>
        public string ExperienceRequirement { get; set; }

        /// <summary>
        /// 其它要求
        /// </summary>
        public string OtherRequirement { get; set; }

        /// <summary>
        /// 岗位职责
        /// </summary>
        public string PositionDescription { get; set; }
    }

    public class RecruitmentRoute
    {
        /// <summary>
        /// 社会招聘
        /// </summary>
        public bool SocialRecruitment { get; set; }
        /// <summary>
        /// 校园招聘
        /// </summary>
        public bool CampusRecruitment { get; set; }
        /// <summary>
        /// 内部协调
        /// </summary>
        public bool InternalTransfer { get; set; }
        /// <summary>
        /// 其它途经
        /// </summary>
        public bool OtherWay { get; set; }
    }

    public class RecruitmentReason
    {
        /// <summary>
        /// 离职补充
        /// </summary>
        public bool LeaveSupplement { get; set; }
        /// <summary>
        /// 调协补充
        /// </summary>
        public bool CoordinateSupplement { get; set; }
        /// <summary>
        /// 岗位新增
        /// </summary>
        public bool PostAddition { get; set; }
        /// <summary>
        /// 岗位扩员
        /// </summary>
        public bool PostExpansion { get; set; }
    }
    #endregion

    #region 印章借用申请内容
    public class SealBorrowContext
    {
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
        /// 部门名称
        /// </summary>
        public string DepartmentName
        {
            get
            {
                if (string.IsNullOrEmpty(this.DepartmentCode))
                {
                    return "";
                }
                else
                {
                    return ((DepartmentType)Enum.Parse(typeof(DepartmentType), this.DepartmentCode)).GetDescription();
                }
            }
        }

        /// <summary>
        /// 印章证照名称
        /// </summary>
        public string SealType { get; set; }

        /// <summary>
        /// 印章证照使用性质
        /// </summary>
        public string SealBorrowType { get; set; }

        /// <summary>
        /// 借用日期
        /// </summary>
        public DateTime BorrowDate { get; set; }

        /// <summary>
        /// 归还日期
        /// </summary>
        public DateTime ReturnDate { get; set; }

        /// <summary>
        /// 借用事由
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 盖章人
        /// </summary>
        public string Sealer { get; set; }
    }
    #endregion

    #region 印章刻制申请内容
    public class SealEngraveContext
    {
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
        /// 部门名称
        /// </summary>
        public string DepartmentName
        {
            get
            {
                if (string.IsNullOrEmpty(this.DepartmentCode))
                {
                    return "";
                }
                else
                {
                    return ((DepartmentType)Enum.Parse(typeof(DepartmentType), this.DepartmentCode)).GetDescription();
                }
            }
        }

        /// <summary>
        /// 印章名称
        /// </summary>
        public string SealName { get; set; }

        /// <summary>
        /// 印章形状
        /// </summary>
        public string SealShape { get; set; }

        /// <summary>
        /// 形状描述
        /// </summary>
        public string SealShapeDec { get; set; }

        /// <summary>
        /// 印章规格
        /// </summary>
        public string SealSize { get; set; }

        /// <summary>
        /// 刻制事由
        /// </summary>
        public string Reason { get; set; }
    }
    #endregion

    #region 工牌补办申请内容
    public class WorkCardContext
    {
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
        /// 部门名称
        /// </summary>
        public string DepartmentName
        {
            get
            {
                if (string.IsNullOrEmpty(this.DepartmentCode))
                {
                    return "";
                }
                else
                {
                    return ((DepartmentType)Enum.Parse(typeof(DepartmentType), this.DepartmentCode)).GetDescription();
                }
            }
        }

        /// <summary>
        /// 受理日期
        /// </summary>
        public DateTime AcceptDate { get; set; }

        /// <summary>
        /// 补办理由
        /// </summary>
        public string Reason { get; set; }
    }
    #endregion

    #region 培训申请内容
    public class InternalTrainingContext
    {
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
        /// 部门名称
        /// </summary>
        public string DepartmentName
        {
            get
            {
                if (string.IsNullOrEmpty(this.DepartmentCode))
                {
                    return "";
                }
                else
                {
                    return ((DepartmentType)Enum.Parse(typeof(DepartmentType), this.DepartmentCode)).GetDescription();
                }
            }
        }

        /// <summary>
        /// 培训方式
        /// </summary>
        public string[] TrainingMethods { get; set; }

        /// <summary>
        /// 培训时间
        /// </summary>
        public DateTime TrainingTime { get; set; }

        /// <summary>
        /// 培训地址
        /// </summary>
        public string TrainingLocation { get; set; }

        /// <summary>
        /// 培训原因
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 培训对象
        /// </summary>
        public string[] Trainees { get; set; }

        /// <summary>
        /// 培训内容
        /// </summary>
        public string TrainingContent { get; set; }
    }
    #endregion

    #region 外训申请内容
    public class ExternalTrainingContext
    {
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
        /// 部门名称
        /// </summary>
        public string DepartmentName
        {
            get
            {
                if (string.IsNullOrEmpty(this.DepartmentCode))
                {
                    return "";
                }
                else
                {
                    return ((DepartmentType)Enum.Parse(typeof(DepartmentType), this.DepartmentCode)).GetDescription();
                }
            }
        }

        /// <summary>
        /// 培训时间
        /// </summary>
        public DateTime TrainingTime { get; set; }

        /// <summary>
        /// 培训地址
        /// </summary>
        public string TrainingLocation { get; set; }

        /// <summary>
        /// 课程名称
        /// </summary>
        public string CourseName { get; set; }

        /// <summary>
        /// 费用
        /// </summary>
        public string Tuition { get; set; }

        /// <summary>
        /// 差旅费
        /// </summary>
        public string TravelExpenses { get; set; }

        /// <summary>
        /// 外训机构名称
        /// </summary>
        public string InstitutionName { get; set; }

        /// <summary>
        /// 课程价值
        /// </summary>
        public string CourseValue { get; set; }

        /// <summary>
        /// 培训对象
        /// </summary>
        public string[] Trainees { get; set; }
    }
    #endregion

    #region 单证档案借阅申请
    public class ArchiveBorrowContext
    {
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
        /// 部门名称
        /// </summary>
        public string DepartmentName
        {
            get
            {
                if (string.IsNullOrEmpty(this.DepartmentCode))
                {
                    return "";
                }
                else
                {
                    return ((DepartmentType)Enum.Parse(typeof(DepartmentType), this.DepartmentCode)).GetDescription();
                }
            }
        }

        /// <summary>
        /// 单证档案名称
        /// </summary>
        public string ArchiveName { get; set; }

        /// <summary>
        /// 借阅日期
        /// </summary>
        public DateTime BorrowDate { get; set; }

        /// <summary>
        /// 预计归还日期
        /// </summary>
        public DateTime ReturnDate { get; set; }

        /// <summary>
        /// 借阅原因
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 单证档案保管人
        /// </summary>
        public string Keeper { get; set; }

        /// <summary>
        /// 财务部负责人
        /// </summary>
        public string Financer { get; set; }
    }
    #endregion

    #region 单证档案外借申请
    public class ArchiveLendingContext
    {
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
        /// 部门名称
        /// </summary>
        public string DepartmentName
        {
            get
            {
                if (string.IsNullOrEmpty(this.DepartmentCode))
                {
                    return "";
                }
                else
                {
                    return ((DepartmentType)Enum.Parse(typeof(DepartmentType), this.DepartmentCode)).GetDescription();
                }
            }
        }

        /// <summary>
        /// 单证档案名称
        /// </summary>
        public string ArchiveName { get; set; }

        /// <summary>
        /// 借阅日期
        /// </summary>
        public DateTime BorrowDate { get; set; }

        /// <summary>
        /// 预计归还日期
        /// </summary>
        public DateTime ReturnDate { get; set; }

        /// <summary>
        /// 借阅原因
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 借阅数量
        /// </summary>
        public string Count { get; set; }

        /// <summary>
        /// 单证档案保管人
        /// </summary>
        public string Keeper { get; set; }
    }
    #endregion

    #region 单证档案销毁申请
    public class ArchiveDestroyContext
    {
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
        /// 部门名称
        /// </summary>
        public string DepartmentName
        {
            get
            {
                if (string.IsNullOrEmpty(this.DepartmentCode))
                {
                    return "";
                }
                else
                {
                    return ((DepartmentType)Enum.Parse(typeof(DepartmentType), this.DepartmentCode)).GetDescription();
                }
            }
        }

        /// <summary>
        /// 审计部负责人
        /// </summary>
        public string AuditManager { get; set; }

        /// <summary>
        /// 档案监销人
        /// </summary>
        public string Supervisor { get; set; }

        public string SupervisorName { get; set; }

        /// <summary>
        /// 销毁档案内容
        /// </summary>
        public List<ArchiveContext> Archives { get; set; }
    }
    public class ArchiveContext
    {
        /// <summary>
        /// 档案年份
        /// </summary>
        public string Year { get; set; }
        /// <summary>
        /// 档案类别
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 档案编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 档案内容
        /// </summary>
        public string Context { get; set; }
        /// <summary>
        /// 档案数量
        /// </summary>
        public string Count { get; set; }
    }
    #endregion

    #region 员工奖惩申请

    public class RewardAndPunishContext
    {
        /// <summary>
        /// 奖惩人
        /// </summary>
        public string StaffID { get; set; }

        public string StaffName { get; set; }

        public string Department { get; set; }

        /// <summary>
        /// 岗位
        /// </summary>
        public string PositionID { get; set; }

        public string PositionName { get; set; }

        /// <summary>
        /// 奖惩类别
        /// </summary>
        public string RewardOrPunish { get; set; }

        /// <summary>
        /// 奖励方式
        /// </summary>
        public string RewardType { get; set; }

        /// <summary>
        /// 惩罚方式
        /// </summary>
        public string PunishType { get; set; }

        /// <summary>
        /// 奖励描述
        /// </summary>
        public string RewardDec { get; set; }

        /// <summary>
        /// 惩罚描述
        /// </summary>
        public string PunishDec { get; set; }

        /// <summary>
        /// 奖惩事由
        /// </summary>
        public string Reason { get; set; }

    }
    #endregion
}