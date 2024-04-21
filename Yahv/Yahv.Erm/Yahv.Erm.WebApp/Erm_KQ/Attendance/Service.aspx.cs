using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Erm.Services;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.Erm.WebApp.Erm_KQ.Attendance
{
    public partial class Service : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 根据Pasts_Attend里的打卡时间，更新考勤状态
        /// </summary>
        /// <returns></returns>
        protected object ModifyPastsStatus()
        {
            var json = new JMessage() { success = true, data = "操作成功!" };

            try
            {
                var date = Request.Form["date"];
                if (string.IsNullOrWhiteSpace(date))
                {
                    json.success = false;
                    json.data = "请您选择考勤日期!";
                    return json;
                }

                Yahv.Erm.Services.AttendanceCalc.Current
                    .Calculate(DateTime.Parse(date), AttendCalcStep.ModifyPastsStatus);
            }
            catch (Exception ex)
            {
                json.success = false;
                json.data = $"更新异常!{ex.Message}";
            }

            return json;
        }

        /// <summary>
        /// 初始化考勤记录
        /// </summary>
        /// <returns></returns>
        protected object InitPasts()
        {
            var json = new JMessage() { success = true, data = "操作成功!" };

            try
            {
                var date = Request.Form["date"];
                if (string.IsNullOrWhiteSpace(date))
                {
                    json.success = false;
                    json.data = "请您选择考勤日期!";
                    return json;
                }

                Yahv.Erm.Services.AttendanceCalc.Current
                    .Calculate(DateTime.Parse(date), AttendCalcStep.InitPasts);
            }
            catch (Exception ex)
            {
                json.success = false;
                json.data = $"更新异常!{ex.Message}";
            }

            return json;
        }

        /// <summary>
        /// 初始化考勤记录
        /// </summary>
        /// <returns></returns>
        protected object SyncData()
        {
            var json = new JMessage() { success = true, data = "操作成功!" };

            try
            {
                var date = Request.Form["date"];
                if (string.IsNullOrWhiteSpace(date))
                {
                    json.success = false;
                    json.data = "请您选择考勤日期!";
                    return json;
                }

                Yahv.Erm.Services.AttendanceCalc.Current
                    .Calculate(DateTime.Parse(date), AttendCalcStep.SyncLogs | AttendCalcStep.ModifyPastsTime);
            }
            catch (Exception ex)
            {
                json.success = false;
                json.data = $"更新异常!{ex.Message}";
            }

            return json;
        }

        /// <summary>
        /// 根据日程安排更新考勤
        /// </summary>
        /// <returns></returns>
        protected object ModifyPastsStatusBySched()
        {
            var json = new JMessage() { success = true, data = "操作成功!" };

            try
            {
                var date = Request.Form["date"];
                if (string.IsNullOrWhiteSpace(date))
                {
                    json.success = false;
                    json.data = "请您选择考勤日期!";
                    return json;
                }

                Yahv.Erm.Services.AttendanceCalc.Current
                    .Calculate(DateTime.Parse(date), AttendCalcStep.ModifyPastsStatusBySched);
            }
            catch (Exception ex)
            {
                json.success = false;
                json.data = $"更新异常!{ex.Message}";
            }

            return json;
        }
    }
}