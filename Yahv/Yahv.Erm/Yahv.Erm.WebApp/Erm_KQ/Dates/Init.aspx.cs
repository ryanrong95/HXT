using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Utils.Http;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Erm.WebApp.Erm_KQ.Dates
{
    public partial class Init : ErpParticlePage
    {
        const string Schedule_B = "B班";
        const string Schedule_C = "C班";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitData();
            }
        }

        #region 加载数据
        private void InitData()
        {
            this.Model.Years = GetYears().OrderBy(item => item.Key).Select(item => new { text = item.Key, value = item.Value });
            this.Model.Schedulings = Erp.Current.Erm.Schedulings.Where(item => item.IsMain == true).Select(item => new { text = item.Name, value = item.ID });
            this.Model.Regions = Erp.Current.Erm.RegionsAc.Where(item => item.FatherID != null).Select(item => new { text = item.Name, value = item.ID });       //区域
        }

        /// <summary>
        /// 百度接口获取日期格式数据
        /// </summary>
        /// <returns></returns>
        protected object getHolidays()
        {
            var result = HttpHelper.Get("https://sp0.baidu.com/8aQDcjqpAAV3otqbppnN2DJv/api.php?query=" + Request.QueryString["year"] + "&resource_id=6018", Encoding.Default);
            return result;
        }
        #endregion

        #region 功能函数
        /// <summary>
        /// 初始化
        /// </summary>
        protected string Submit()
        {
            JMessage json = new JMessage()
            {
                success = true,
            };

            try
            {
                int year = int.Parse(Request.Form["year"]);     //年份
                string shiftID = Request.Form["ShiftID"];     //所属班别ID
                string shiftName = Erp.Current.Erm.Schedulings[shiftID]?.Name;      //所属班别名称
                string regionID = Request.Form["RegionID"];
                string reginName = Erp.Current.Erm.RegionsAc[regionID]?.Name;
                string schedule_c_id = Erp.Current.Erm.Schedulings.FirstOrDefault(item => item.Name == Schedule_C)?.ID;
                var calendars = Erp.Current.Erm.Calendars.Where(item => item.Year == year).ToArray();
                var holidays = GetGovernmentHolidays();


                using (var reponsitory = LinqFactory<PvbErmReponsitory>.Create())
                {
                    if (Erp.Current.Erm.SchedulesPublic.Any(item => item.Date.Year == year && item.RegionID == regionID && item.ShiftID == shiftID))
                    {
                        json.success = false;
                        json.data = $"{year}，{reginName}区域下的该班别已经有数据，不能再初始化!";

                        return json.Json();
                    }


                    //创建这个年份的自然日
                    if (calendars == null || !calendars.Any())
                    {
                        CreateCalendars(reponsitory, year);
                        calendars = Erp.Current.Erm.Calendars.Where(item => item.Year == year).ToArray();
                    }

                    DataTable dt_Schedule = this.GetSchedules();
                    DataTable dt_SchedulePublic = this.GetSchedulesPublic();

                    DataRow dr_Schedule;
                    DataRow dr_SchedulePublic;
                    string id = string.Empty;
                    string adminID = Erp.Current.ID;

                    foreach (var calendar in calendars)
                    {
                        id = PKeySigner.Pick(PKeyType.Sched);

                        dr_Schedule = dt_Schedule.NewRow();
                        dr_SchedulePublic = dt_SchedulePublic.NewRow();

                        #region Schedule

                        dr_Schedule["ID"] = id;
                        dr_Schedule["Date"] = calendar.ID;
                        dr_Schedule["CreateDate"] = DateTime.Now;
                        dr_Schedule["CreatorID"] = adminID;
                        dr_Schedule["ModifyDate"] = DateTime.Now;
                        dr_Schedule["Type"] = (int)ScheduleType.Public;
                        dt_Schedule.Rows.Add(dr_Schedule);
                        #endregion

                        #region SchedulePublic

                        dr_SchedulePublic["ID"] = id;
                        dr_SchedulePublic["RegionID"] = regionID;
                        dr_SchedulePublic["SalaryMultiple"] = 1;
                        dr_SchedulePublic["ShiftID"] = shiftID;

                        //A 班
                        if (shiftName != Schedule_B)
                        {
                            dr_SchedulePublic["Method"] = (int)GetSchedMethod_A(calendar.Date, holidays);
                            dr_SchedulePublic["Name"] = GetScheduleName(calendar.Date, holidays);
                            dr_SchedulePublic["From"] = (int)ScheduleFrom.Government;
                            dr_SchedulePublic["SchedulingID"] = shiftID;
                        }
                        //B 班
                        else
                        {
                            dr_SchedulePublic["Method"] = (int)GetSchedMethod_B(calendar.Date, holidays);
                            dr_SchedulePublic["Name"] = GetScheduleName_B(calendar.Date, holidays);
                            dr_SchedulePublic["From"] = (int)GetScheduleFrom_B(calendar.Date, holidays);
                            dr_SchedulePublic["SchedulingID"] = GetScheduling_B(calendar.Date, shiftID, schedule_c_id, holidays);
                        }

                        dt_SchedulePublic.Rows.Add(dr_SchedulePublic);

                        #endregion
                    }

                    if (dt_Schedule != null && dt_Schedule.Rows.Count > 0 && dt_SchedulePublic != null && dt_SchedulePublic.Rows.Count > 0)
                    {
                        reponsitory.SqlBulkCopyByDatatable("Schedules", dt_Schedule);
                        reponsitory.SqlBulkCopyByDatatable("SchedulesPublic", dt_SchedulePublic);
                    }
                }
            }
            catch (Exception ex)
            {
                json.success = false;
                json.data = ex.Message;
            }

            return json.Json();
        }
        #endregion

        #region 自定义函数
        /// <summary>
        /// 获取日程安排表结构
        /// </summary>
        /// <returns></returns>
        private DataTable GetSchedules()
        {
            DataTable dt = new DataTable("Schedules");

            dt.Columns.Add("ID");
            dt.Columns.Add("Date");
            dt.Columns.Add("Type");
            dt.Columns.Add("CreatorID");
            dt.Columns.Add("ModifyID");
            dt.Columns.Add("CreateDate");
            dt.Columns.Add("ModifyDate");

            return dt;
        }

        /// <summary>
        /// 公共日程安排表结构
        /// </summary>
        /// <returns></returns>
        private DataTable GetSchedulesPublic()
        {
            DataTable dt = new DataTable("SchedulesPublic");

            dt.Columns.Add("ID");
            dt.Columns.Add("Method");
            dt.Columns.Add("Name");
            dt.Columns.Add("From");
            dt.Columns.Add("RegionID");
            dt.Columns.Add("PostionID");
            dt.Columns.Add("SalaryMultiple");
            dt.Columns.Add("ShiftID");
            dt.Columns.Add("SchedulingID");

            return dt;
        }

        /// <summary>
        /// 日历表结构
        /// </summary>
        /// <returns></returns>
        private DataTable GetCalendars()
        {
            DataTable dt = new DataTable("SchedulesPublic");
            dt.Columns.Add("ID");
            dt.Columns.Add("Year");
            dt.Columns.Add("Month");
            dt.Columns.Add("Day");
            dt.Columns.Add("Week");
            return dt;
        }

        /// <summary>
        /// 获取节假日
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string[]> GetGovernmentHolidays()
        {
            var result = new Dictionary<string, string[]>();

            try
            {
                //调休工作日（周六日）
                string[] working_day = GetDateStrings(Request.Form["working_day"]);
                //元旦
                string[] new_years_day = GetDateStrings(Request.Form["new_years_day"]);
                //春节
                string[] spring_festival = GetDateStrings(Request.Form["spring_festival"]);
                //清明节
                string[] tomb_sweeping_day = GetDateStrings(Request.Form["tomb_sweeping_day"]);
                //劳动节
                string[] labour_day = GetDateStrings(Request.Form["labour_day"]);
                //端午
                string[] dragon_boat_festival = GetDateStrings(Request.Form["dragon_boat_festival"]);
                //中秋
                string[] mid_autumn_festival = GetDateStrings(Request.Form["mid_autumn_festival"]);
                //国庆节
                string[] national_day = GetDateStrings(Request.Form["national_day"]);


                result.Add(CalendarType.WorkingDay.GetDescription(), working_day);
                result.Add(CalendarType.NewYearsDay.GetDescription(), new_years_day);
                result.Add(CalendarType.SpringFestival.GetDescription(), spring_festival);
                result.Add(CalendarType.TombSweepingDay.GetDescription(), tomb_sweeping_day);
                result.Add(CalendarType.LabourDay.GetDescription(), labour_day);
                result.Add(CalendarType.DragonBoatFestival.GetDescription(), dragon_boat_festival);
                result.Add(CalendarType.MidAutumnFestival.GetDescription(), mid_autumn_festival);
                result.Add(CalendarType.NationalDay.GetDescription(), national_day);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// 年份
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetYears()
        {
            //只能初始化当年或第二年
            var result = new Dictionary<string, string>();
            result.Add((DateTime.Now.Year - 1).ToString(), (DateTime.Now.Year - 1).ToString());
            result.Add(DateTime.Now.Year.ToString(), DateTime.Now.Year.ToString());
            result.Add((DateTime.Now.Year + 1).ToString(), (DateTime.Now.Year + 1).ToString());
            return result;
        }

        /// <summary>
        /// 根据年份创建 自然日
        /// </summary>
        /// <param name="year"></param>
        private void CreateCalendars(PvbErmReponsitory reponsitory, int year)
        {
            var data = GetCalendars();
            DateTime dateTime = DateTime.Parse(year + "-01-01");
            DataRow dr;

            while (dateTime.Year == year)
            {
                dr = data.NewRow();

                dr["ID"] = dateTime.ToString("yyyy-MM-dd");
                dr["Year"] = year;
                dr["Month"] = dateTime.Month;
                dr["Day"] = dateTime.Day;
                dr["Week"] = ((int)dateTime.DayOfWeek - 1) > 0 ? ((int)dateTime.DayOfWeek - 1) : 6;
                data.Rows.Add(dr);

                dateTime = dateTime.AddDays(1);
            }

            if (data != null && data.Rows.Count > 0)
            {
                reponsitory.SqlBulkCopyByDatatable("Calendars", data);
            }
        }

        /// <summary>
        /// 获取日程安排名称（节假日名称）
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private string GetScheduleName(string date, Dictionary<string, string[]> holidays)
        {
            var day = holidays.FirstOrDefault(item => item.Value.Contains(date));
            if (string.IsNullOrEmpty(day.Key))
            {
                //是否周六日
                if (IsWeekend(date))
                {
                    return CalendarType.Holiday.GetDescription();
                }
                else
                {
                    return CalendarType.WorkingDay.GetDescription();
                }
            }
            else
            {
                return day.Key;
            }
        }

        /// <summary>
        /// 是否周末
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private bool IsWeekend(string date)
        {
            return Convert.ToDateTime(date).DayOfWeek == DayOfWeek.Sunday ||
                    Convert.ToDateTime(date).DayOfWeek == DayOfWeek.Saturday;
        }

        /// <summary>
        /// 获取日程安排方式
        /// </summary>
        /// <remarks>工作日、公休日、法定假日</remarks>
        /// <param name="date"></param>
        /// <returns></returns>
        private ScheduleMethod GetSchedMethod_A(string date, Dictionary<string, string[]> holidays)
        {
            var day = holidays.FirstOrDefault(item => item.Value.Contains(date));

            //国家制定公休日为工作日
            if (day.Key == CalendarType.WorkingDay.GetDescription())
            {
                return ScheduleMethod.Work;
            }

            //法定假期
            if (!string.IsNullOrWhiteSpace(day.Key))
            {
                return ScheduleMethod.LegalHoliday;
            }

            //周六日
            if (IsWeekend(date))
            {
                return ScheduleMethod.PublicHoliday;
            }

            return ScheduleMethod.Work;
        }


        /// <summary>
        /// 获取日程安排名称（节假日名称）
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private string GetScheduleName_B(string date, Dictionary<string, string[]> holidays)
        {
            var day = holidays.FirstOrDefault(item => item.Value.Contains(date));
            if (string.IsNullOrEmpty(day.Key))
            {
                //是否周六日
                if (IsWeekend(date) && Convert.ToDateTime(date).DayOfWeek == DayOfWeek.Sunday)
                {
                    return CalendarType.Holiday.GetDescription();
                }
                else
                {
                    return CalendarType.WorkingDay.GetDescription();
                }
            }
            else
            {
                return day.Key;
            }
        }

        /// <summary>
        /// 获取日程安排方式
        /// </summary>
        /// <remarks>工作日、公休日、法定假日</remarks>
        /// <param name="date"></param>
        /// <returns></returns>
        private ScheduleMethod GetSchedMethod_B(string date, Dictionary<string, string[]> holidays)
        {
            var result = GetSchedMethod_A(date, holidays);
            /*由于C班不记录考勤，所以B班的初始化与A班一致*/
            //if (result == ScheduleMethod.PublicHoliday && Convert.ToDateTime(date).DayOfWeek == DayOfWeek.Saturday)
            //{
            //    result = ScheduleMethod.Work;
            //}

            return result;
        }


        /// <summary>
        /// 获取B班，实际班别
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private string GetScheduling_B(string date, string shiftID, string schedule_c_id, Dictionary<string, string[]> holidays)
        {
            string schedulingID = shiftID;

            /*由于C班不记录考勤，所以B班周六的实际班别也初始化成B班*/
            //var method = GetSchedMethod_A(date, holidays);
            //if (method == ScheduleMethod.PublicHoliday && Convert.ToDateTime(date).DayOfWeek == DayOfWeek.Saturday)
            //{
            //    schedulingID = schedule_c_id;
            //}

            return schedulingID;
        }


        /// <summary>
        /// 获取B班日程安排来源
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private ScheduleFrom GetScheduleFrom_B(string date, Dictionary<string, string[]> holidays)
        {
            var result = ScheduleFrom.Government;

            var method = GetSchedMethod_A(date, holidays);
            if (method == ScheduleMethod.PublicHoliday && Convert.ToDateTime(date).DayOfWeek == DayOfWeek.Saturday)
            {
                result = ScheduleFrom.Company;
            }

            return result;
        }

        /// <summary>
        /// 检验字符串是否为日期格式
        /// </summary>
        /// <param name="dates"></param>
        /// <returns></returns>
        private string[] GetDateStrings(string dates)
        {
            string[] result = dates.Trim().Split(new string[] { ",", "，" }, StringSplitOptions.RemoveEmptyEntries);
            return result.Select(item => DateTime.Parse(item)).Select(item => item.ToString("yyyy-MM-dd")).ToArray();
        }

        /// <summary>
        /// 获取节假日
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string[]> GetGovernmentHolidays_2020()
        {
            var result = new Dictionary<string, string[]>();

            //调休工作日（周六日
            string[] working_day = new string[] { "2020-01-19", "2020-04-26", "2020-05-09", "2020-06-28", "2020-09-27", "2020-10-10" };
            //元旦
            string[] new_years_day = new string[] { "2020-01-01" };
            //春节
            string[] spring_festival = new string[] { "2020-01-24", "2020-01-25", "2020-01-26", "2020-01-27", "2020-01-28", "2020-01-29", "2020-01-30", "2020-01-31", "2020-02-01", "2020-02-02" };
            //清明节
            string[] tomb_sweeping_day = new string[] { "2020-04-04", "2020-04-05", "2020-04-06" };
            //劳动节
            string[] labour_day = new string[] { "2020-05-01", "2020-05-02", "2020-05-03", "2020-05-04", "2020-05-05" };
            //端午
            string[] dragon_boat_festival = new string[] { "2020-06-25", "2020-06-26", "2020-06-27", };
            //中秋
            string[] mid_autumn_festival = new string[] { "" };
            //国庆节
            string[] national_day = new string[] { "2020-10-01", "2020-10-02", "2020-10-03", "2020-10-04", "2020-10-05", "2020-10-06", "2020-10-07", "2020-10-08", };


            result.Add(CalendarType.WorkingDay.GetDescription(), working_day);
            result.Add(CalendarType.NewYearsDay.GetDescription(), new_years_day);
            result.Add(CalendarType.SpringFestival.GetDescription(), spring_festival);
            result.Add(CalendarType.TombSweepingDay.GetDescription(), tomb_sweeping_day);
            result.Add(CalendarType.LabourDay.GetDescription(), labour_day);
            result.Add(CalendarType.DragonBoatFestival.GetDescription(), dragon_boat_festival);
            result.Add(CalendarType.MidAutumnFestival.GetDescription(), mid_autumn_festival);
            result.Add(CalendarType.NationalDay.GetDescription(), national_day);

            return result;
        }
        #endregion
    }
}