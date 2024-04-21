using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Yahv.Erm.Services.Common;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Erm.WebApp.Erm_KQ.Attends
{
    public partial class ListOfMy : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
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
                var staff = Alls.Current.Staffs.Where(item => item.Admin.ID == Erp.Current.ID).FirstOrDefault();
                if (staff != null)
                {
                    predicate = predicate.And(item => item.StaffID == staff.ID);
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

        #region 获取数据

        protected object data()
        {
            Expression<Func<PastsAttend, bool>> expression = Predicate();
            var query = Erp.Current.Erm.PastsAttendRoll.GetPastsAttendFinalResult(expression).OrderByDescending(item => item.Date); ;
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
                           DateStr = entity.Date.ToString("yyyy-MM-dd"),
                       };
            return new
            {
                rows = linq.ToArray(),
                total = query.Count(),
            };
        }

        protected void LoadData()
        {
            #region 查询条件
            Expression<Func<PastsAttend, bool>> predicate = item => true;
            var startDate = AttendHelper.GetAttendStartDate(DateTime.Now.Year, DateTime.Now.Month);
            var endDate = AttendHelper.GetAttendEndDate(DateTime.Now.Year, DateTime.Now.Month);
            predicate = predicate.And(item => item.Date >= startDate);
            predicate = predicate.And(item => item.Date < endDate.AddDays(1));

            var staff = Alls.Current.Staffs.Where(item => item.Admin.ID == Erp.Current.ID).FirstOrDefault();
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
            var staff = Alls.Current.Staffs.Where(item => item.Admin.ID == Erp.Current.ID).FirstOrDefault();
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

        /// <summary>
        /// 获取周几
        /// </summary>
        /// <returns></returns>
        private string GetWeekName(int week)
        {
            string result = string.Empty;

            switch (week)
            {
                case 0:
                    result = "星期日";
                    break;
                case 1:
                    result = "星期一";
                    break;
                case 2:
                    result = "星期二";
                    break;
                case 3:
                    result = "星期三";
                    break;
                case 4:
                    result = "星期四";
                    break;
                case 5:
                    result = "星期五";
                    break;
                case 6:
                    result = "星期六";
                    break;
                default:
                    break;
            }

            return result;
        }

        #endregion
    }
}