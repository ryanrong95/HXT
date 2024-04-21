using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Common;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Erm.WebApp.Erm_KQ.Attends
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboBoxData();
                LoadData();
            }
        }

        protected void LoadComboBoxData()
        {
            var staffs = Erp.Current.Erm.XdtStaffs
                .Where(item => item.Status == StaffStatus.Normal || item.Status == StaffStatus.Period);
            //员工
            this.Model.StaffData = staffs.Select(item => new
            {
                Value = item.Admin.ID,
                Text = item.Name,
            });
        }

        protected void LoadData()
        {
            #region 查询条件
            Expression<Func<PastsAttend, bool>> predicate = item => true;
            var startDate = AttendHelper.GetAttendStartDate(DateTime.Now.Year, DateTime.Now.Month);
            var endDate = AttendHelper.GetAttendEndDate(DateTime.Now.Year, DateTime.Now.Month);
            predicate = predicate.And(item => item.Date >= startDate);
            predicate = predicate.And(item => item.Date < endDate.AddDays(1));
            #endregion

            //获取考勤数据
            var query = Erp.Current.Erm.PastsAttendRoll.GetPastsAttend(predicate);
            this.Model.AttendData = new
            {
                Data = DateTime.Now.Year + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0'),
                //工作日天数
                WorkDays = query.Where(item => item.InFact != AttendInFactType.LegalHolidays && item.InFact != AttendInFactType.PublicHoliday &&
                    item.InFact != AttendInFactType.Overtime).Count() * 0.5,
                //正常天数
                NormalDays = query.Where(item => item.InFact == AttendInFactType.Normal || item.InFact == AttendInFactType.SystemAuthorizing).Count() * 0.5,
                //加班天数
                OvertimeDays = query.Where(item => item.InFact == AttendInFactType.Overtime).Count() * 0.5,
                //迟到早退次数
                LaterOrEarlyTimes = query.Where(item => item.IsEarly == true).Count() + query.Where(item => item.IsLater == true).Count(),
                //补签次数
                ResignTimes = query.Where(item => item.OnWorkRemedy == true).Count() + query.Where(item => item.OffWorkRemedy == true).Count(),
                //事假天数
                CasualLeavedays = query.Where(item => item.InFact == AttendInFactType.CasualLeave).Count() * 0.5,
                //病假天数
                SickLeavedays = query.Where(item => item.InFact == AttendInFactType.SickLeave).Count() * 0.5,
                //旷工天数
                Absenteeismdays = query.Where(item => item.InFact == AttendInFactType.Absenteeism).Count() * 0.5,
                //公务天数
                OfficialBusinessdays = query.Where(item => item.InFact == AttendInFactType.OfficialBusiness).Count() * 0.5,
                //公差天数
                BusinessTripdays = query.Where(item => item.InFact == AttendInFactType.BusinessTrip).Count() * 0.5,
                //带薪假期天数
                PaidLeavedays = query.Where(item => item.InFact == AttendInFactType.PaidLeave).Count() * 0.5,
            };
        }

        /// <summary>
        /// 统计数据
        /// </summary>
        /// <returns></returns>
        protected void DateChange()
        {
            try
            {
                #region 查询条件
                var Date = Request.Form["Date"];
                var AdminID = Request.Form["AdminID"];
                Expression<Func<PastsAttend, bool>> predicate = item => true;
                if (string.IsNullOrEmpty(Date))
                {
                    var startDate = AttendHelper.GetAttendStartDate(DateTime.Now.Year, DateTime.Now.Month);
                    var endDate = AttendHelper.GetAttendEndDate(DateTime.Now.Year, DateTime.Now.Month);
                    predicate = predicate.And(item => item.Date >= startDate);
                    predicate = predicate.And(item => item.Date < endDate.AddDays(1));
                }
                else
                {
                    DateTime date = Convert.ToDateTime(Date.Trim());
                    var startDate = AttendHelper.GetAttendStartDate(date.Year, date.Month);
                    var endDate = AttendHelper.GetAttendEndDate(date.Year, date.Month);
                    predicate = predicate.And(item => item.Date >= startDate);
                    predicate = predicate.And(item => item.Date < endDate.AddDays(1));
                }
                var staff = Alls.Current.Staffs.Where(item => item.Admin.ID == AdminID).FirstOrDefault();
                if (staff != null)
                {
                    predicate = predicate.And(item => item.StaffID == staff.ID);
                    if (staff.Labour?.EntryDate != null)
                    {
                        predicate = predicate.And(item => item.Date >= staff.Labour.EntryDate);
                    }
                }
                #endregion

                //获取考勤数据
                var query = Erp.Current.Erm.PastsAttendRoll.GetPastsAttend(predicate);
                var data = new
                {
                    Data = Date,
                    //工作日天数
                    WorkDays = query.Where(item => item.InFact != AttendInFactType.LegalHolidays && item.InFact != AttendInFactType.PublicHoliday &&
                        item.InFact != AttendInFactType.Overtime).Count() * 0.5,
                    //正常天数
                    NormalDays = query.Where(item => item.InFact == AttendInFactType.Normal || item.InFact == AttendInFactType.SystemAuthorizing).Count() * 0.5,
                    //加班天数
                    OvertimeDays = query.Where(item => item.InFact == AttendInFactType.Overtime).Count() * 0.5,
                    //迟到早退次数
                    LaterOrEarlyTimes = query.Where(item => item.IsEarly == true).Count() + query.Where(item => item.IsLater == true).Count(),
                    //补签次数
                    ResignTimes = query.Where(item => item.OnWorkRemedy == true).Count() + query.Where(item => item.OffWorkRemedy == true).Count(),
                    //事假天数
                    CasualLeavedays = query.Where(item => item.InFact == AttendInFactType.CasualLeave).Count() * 0.5,
                    //病假天数
                    SickLeavedays = query.Where(item => item.InFact == AttendInFactType.SickLeave).Count() * 0.5,
                    //旷工天数
                    Absenteeismdays = query.Where(item => item.InFact == AttendInFactType.Absenteeism).Count() * 0.5,
                    //公务天数
                    OfficialBusinessdays = query.Where(item => item.InFact == AttendInFactType.OfficialBusiness).Count() * 0.5,
                    //公差天数
                    BusinessTripdays = query.Where(item => item.InFact == AttendInFactType.BusinessTrip).Count() * 0.5,
                    //带薪假期天数
                    PaidLeavedays = query.Where(item => item.InFact == AttendInFactType.PaidLeave).Count() * 0.5,
                };

                Response.Write((new { success = true, message = "成功", data = data }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 系统授权考勤正常
        /// </summary>
        protected void SysNormal()
        {
            try
            {
                var AdminID = Request.Form["AdminID"];
                var Dates = Request.Form["Dates"];

                List<DateTime> datetimes = new List<DateTime>();
                foreach (var date in Dates.Split(','))
                {
                    datetimes.Add(Convert.ToDateTime(date));
                }
                var staff = Alls.Current.Staffs.Single(item => item.Admin.ID == AdminID);
                Services.Common.AttendHelper.UpdatePastsAttendSysNormal(staff.ID, datetimes.ToArray());

                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "员工考勤",
                    $"系统授权", staff.ID + ":" + Dates);
                Response.Write((new { success = true, message = "成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// Excel导出
        /// </summary>
        protected void btnExport_Click(object sender, EventArgs e)
        {
            #region 查询条件
            var Date = Request.Form["s_date"];
            var AdminID = Request.Form["Staff"];
            Expression<Func<PastsAttend, bool>> predicate = item => true;
            if (string.IsNullOrEmpty(Date))
            {
                var startDate = AttendHelper.GetAttendStartDate(DateTime.Now.Year, DateTime.Now.Month);
                var endDate = AttendHelper.GetAttendEndDate(DateTime.Now.Year, DateTime.Now.Month);
                predicate = predicate.And(item => item.Date >= startDate);
                predicate = predicate.And(item => item.Date < endDate.AddDays(1));
            }
            else
            {
                DateTime date = Convert.ToDateTime(Date.Trim());
                var startDate = AttendHelper.GetAttendStartDate(date.Year, date.Month);
                var endDate = AttendHelper.GetAttendEndDate(date.Year, date.Month);
                predicate = predicate.And(item => item.Date >= startDate);
                predicate = predicate.And(item => item.Date < endDate.AddDays(1));
            }
            var staff = Alls.Current.Staffs.Where(item => item.Admin.ID == AdminID).FirstOrDefault();
            if (staff != null)
            {
                predicate = predicate.And(item => item.StaffID == staff.ID);
            }
            #endregion

            //获取考勤数据
            var attends = Erp.Current.Erm.PastsAttendRoll.GetPastsAttend(predicate).ToArray();
            var fileName = ExportWages.Current.MakeAttendsExcel(DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls", attends);
            //下载文件
            DownLoadFile(fileName);
        }

        #region 获取数据

        protected object data()
        {
            int page = int.Parse(Request.QueryString["page"]);
            int rows = int.Parse(Request.QueryString["rows"]);

            Expression<Func<PastsAttend, bool>> expression = Predicate();
            var query = Erp.Current.Erm.PastsAttendRoll.GetPastsAttendFinalResult(expression).OrderByDescending(item=>item.Date);
            var linq = from entity in query
                       select new
                       {
                           AdminID = Erp.Current.IsSuper ? "" : Erp.Current.ID,
                           Date = entity.Date.ToString("yyyy-MM-dd dddd"),
                           Name = entity.Staff.Name,
                           Code = entity.Staff.Code,
                           Scheduling = entity.Scheduling.Name,
                           DepartmentCode = string.IsNullOrEmpty(entity.Staff.DepartmentCode) ? "" : ((DepartmentType)Enum.Parse(typeof(DepartmentType), entity.Staff.DepartmentCode)).GetDescription(),
                           AttendTime = entity.AttendTime,
                           WordHours = entity.WordHours,
                           AttendResult = entity.AttendResult,

                           StaffID = entity.Staff.ID,
                           DateStr = entity.Date.ToString("yyyy-MM-dd"),
                       };
            return new
            {
                rows = linq.Skip((page - 1) * rows).Take(rows),
                total = query.Count(),
            };
        }

        protected object LoadDate()
        {
            return new
            {
                rows = new List<object>(),
                total = 0,
            };
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 查询条件
        /// </summary>
        /// <returns></returns>
        private Expression<Func<PastsAttend, bool>> Predicate()
        {
            Expression<Func<PastsAttend, bool>> predicate = item => true;

            var Date = Request.QueryString["Date"];
            var AdminID = Request.QueryString["AdminID"];
            var IsWorkDate = Request.QueryString["IsWorkDate"];
            if (string.IsNullOrEmpty(Date))
            {
                var startDate = AttendHelper.GetAttendStartDate(DateTime.Now.Year, DateTime.Now.Month);
                var endDate = AttendHelper.GetAttendEndDate(DateTime.Now.Year, DateTime.Now.Month);
                predicate = predicate.And(item => item.Date >= startDate);
                predicate = predicate.And(item => item.Date < endDate.AddDays(1));
            }
            else
            {
                DateTime date = Convert.ToDateTime(Date.Trim());
                var startDate = AttendHelper.GetAttendStartDate(date.Year, date.Month);
                var endDate = AttendHelper.GetAttendEndDate(date.Year, date.Month);
                predicate = predicate.And(item => item.Date >= startDate);
                predicate = predicate.And(item => item.Date < endDate.AddDays(1));
            }
            if (IsWorkDate != "true")
            {
                predicate = predicate.And(item => item.InFact != AttendInFactType.LegalHolidays && item.InFact != AttendInFactType.PublicHoliday);
            }
            var staff = Alls.Current.Staffs.Where(item => item.Admin.ID == AdminID).FirstOrDefault();
            if (staff != null)
            {
                predicate = predicate.And(item => item.StaffID == staff.ID);
                if (staff.Labour?.EntryDate != null)
                {
                    predicate = predicate.And(item => item.Date >= staff.Labour.EntryDate);
                }
            }
            return predicate;
        }

        #endregion
    }
}