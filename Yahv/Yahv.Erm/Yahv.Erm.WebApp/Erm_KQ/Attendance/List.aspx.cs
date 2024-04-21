using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Common;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Erm.WebApp.Erm_KQ.Attendance
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboBoxData();
            }
        }

        #region 获取数据
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

        protected object data()
        {
            Expression<Func<PastsAttend, bool>> expression = Predicate();
            var query = Erp.Current.Erm.PastsAttendRoll.GetPastsAttendFinalResult(expression);
            var linq = from entity in query
                       select new
                       {
                           AdminID = Erp.Current.IsSuper ? "" : Erp.Current.ID,
                           Date = entity.Date.ToString("yyyy-MM-dd"),
                           Week = GetWeekName((int)entity.Date.DayOfWeek),
                           Name = entity.Staff.Name,
                           Code = entity.Staff.Code,
                           Scheduling = entity.Scheduling.Name,
                           DepartmentCode = string.IsNullOrEmpty(entity.Staff.DepartmentCode) ? "" : ((DepartmentType)Enum.Parse(typeof(DepartmentType), entity.Staff.DepartmentCode)).GetDescription(),
                           AttendTime = entity.AttendTime,
                           AttendResult = entity.AttendResult,
                           entity.ID,
                           entity.StaffID,
                       };
            return new
            {
                rows = linq.ToArray(),
                total = query.Count(),
            };
        }

        #endregion

        #region 功能函数

        protected object Calc()
        {
            JMessage json = new JMessage() { success = true, data = "操作成功!" };

            try
            {
                var date = DateTime.Parse(Request.Form["date"]);
                var staffId = Request.Form["staffId"];

                if (string.IsNullOrWhiteSpace(staffId))
                {
                    json.success = false;
                    json.data = "员工ID不能为空!";
                    return json;
                }

                Yahv.Erm.Services.AttendanceCalc.Current.Calculate(date, AttendCalcStep.ModifyPastsStatus | AttendCalcStep.ModifyPastsStatusBySched, staffId: staffId);
            }
            catch (Exception ex)
            {
                json.success = false;
                json.data = $"计算异常：{ex.Message}";
            }

            return json;
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