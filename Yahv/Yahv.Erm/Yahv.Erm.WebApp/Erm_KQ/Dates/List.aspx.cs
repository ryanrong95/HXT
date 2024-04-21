using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Web.Erp;

namespace Yahv.Erm.WebApp.Erm_KQ.Dates
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitData();
            }
        }
        
        #region 获取数据
        /// <summary>
        /// 数据加载
        /// </summary>
        private void InitData()
        {
            this.Model.Methods = ExtendsEnum.ToDictionary<ScheduleMethod>().Select(item => new { Value = item.Key, Text = item.Value });
            this.Model.Schedulings = Erp.Current.Erm.Schedulings.Where(item => item.IsMain == true).Select(item => new { Value = item.ID, Text = item.Name });
            this.Model.RegionData = new Services.Views.Origins.RegionsAcOrigin().Where(item => item.FatherID != null).Select(item => new { Value = item.ID, Text = item.Name });
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            var query = Erp.Current.Erm.SchedulesPublic.Where(GetPredicate()).ToArray();
            return from entity in query
                   select new
                   {
                       entity.ID,
                       Date = entity.Date.ToString("yyyy-MM-dd"),
                       entity.ShiftName,
                       From = entity.From.GetDescription(),
                       Method = entity.Method == ScheduleMethod.LegalHoliday ? $"{entity.Method.GetDescription()}({entity.Name})" : entity.Method.GetDescription(),
                       MethodID = entity.Method,
                       entity.RegionName,
                       entity.SchedulingName,
                       entity.SalaryMultiple,
                       entity.Name,
                       Week = GetWeekName((int)entity.Date.DayOfWeek),
                   };
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 查询条件
        /// </summary>
        /// <returns></returns>
        private Expression<Func<SchedulePublic, bool>> GetPredicate()
        {
            Expression<Func<SchedulePublic, bool>> predicate = item => true;

            string date = Request.QueryString["s_date"];        //日期
            string method = Request.QueryString["s_method"];        //假期类型
            string scheduling = Request.QueryString["s_scheduling"];        //所属班别
            string region = Request.QueryString["s_region"];        //所属班别

            //获取当月数据
            if (!string.IsNullOrWhiteSpace(date))
            {
                var datetime = Convert.ToDateTime(date);
                predicate = predicate.And(item => item.Date.Month == datetime.Month && item.Date.Year == datetime.Year);
            }

            if (!string.IsNullOrWhiteSpace(method))
            {
                predicate = predicate.And(item => item.Method == (ScheduleMethod)(int.Parse(method)));
            }

            if (!string.IsNullOrWhiteSpace(scheduling))
            {
                predicate = predicate.And(item => item.ShiftID == scheduling);
            }
            if (!string.IsNullOrWhiteSpace(region))
            {
                predicate = predicate.And(item => item.RegionID == region);
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