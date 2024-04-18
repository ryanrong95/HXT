using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.AdminScore
{
    public partial class WeeklyScore : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitDropData();
            }
        }

        /// <summary>
        /// 查询下拉框数据加载
        /// </summary>
        protected void InitDropData()
        {
            var admins = new NtErp.Crm.Services.Views.AdminTopView().Where(item => item.JobType != 0).Select(item => new { item.ID, item.RealName });
            this.Model.Admins = admins.Json();
            this.Model.JobTypeData = EnumUtils.ToDictionary<NtErp.Crm.Services.Enums.ScoreType>().Select(item => new { value = item.Key, text = item.Value }).Json();
            this.Model.Year = new NtErp.Crm.Services.Views.AdminScoreAlls().Select(item => new { value = item.Year, text = item.Year }).Distinct().Json();
            this.Model.Month = new NtErp.Crm.Services.Views.AdminScoreAlls().Select(item => new { value = item.Month, text = item.Month }).Distinct().Json();
        }


        /// <summary>
        /// 查询数据
        /// </summary>
        protected void Data()
        {
            string AdminID = Request.QueryString["AdminID"];
            string JobType = Request.QueryString["JobType"];
            string Year = Request.QueryString["Year"];
            string Month = Request.QueryString["Month"];

            var AdminScores = new NtErp.Crm.Services.Views.AdminScoreAlls().AsQueryable();

            if (!string.IsNullOrWhiteSpace(AdminID))
            {
                AdminScores = AdminScores.Where(item => item.Admin.ID == AdminID);
            }
            if (!string.IsNullOrWhiteSpace(JobType))
            {
                AdminScores = AdminScores.Where(item => item.ScoreType == (NtErp.Crm.Services.Enums.ScoreType)int.Parse(JobType));
            }
            if (!string.IsNullOrWhiteSpace(Year))
            {
                AdminScores = AdminScores.Where(item => item.Year == Year);
            }
            if (!string.IsNullOrWhiteSpace(Month))
            {
                AdminScores = AdminScores.Where(item => item.Month == Month);
            }

            //筛选页面需要的数据
            Func<NtErp.Crm.Services.Models.Score, object> convert = item => new
            {
                item.ID,
                item.ReportScore,
                item.DIScore,
                item.DWScore,
                item.Year,
                item.Month,
                ClientScore = item.ScoreType == NtErp.Crm.Services.Enums.ScoreType.Sales ? item.ClientScore : item.ProjectScore,
                AdminID = item.Admin.ID,
                AdminName = item.Admin.RealName,
                ScoreTypeName = item.ScoreType.GetDescription(),
                item.TotalScore,
                item.Bonus,
                DistrictName = string.Join(",", item.DistrictNames),
            };
            var b = DateTime.Now;
            this.Paging(AdminScores, convert);
            var c = DateTime.Now;
        }

        #region 数据导出
        /// <summary>
        /// 数据导出
        /// </summary>
        protected void Export()
        {
            var AdminScores = new NtErp.Crm.Services.Views.AdminScoreAlls().AsQueryable(); ;
            if (!string.IsNullOrWhiteSpace(Request.Form["AdminID"]))
            {
                AdminScores = AdminScores.Where(item => item.Admin.ID == Request.Form["AdminID"]);
            }
            if (!string.IsNullOrWhiteSpace(Request.Form["JobType"]))
            {
                AdminScores = AdminScores.Where(item => item.ScoreType == (NtErp.Crm.Services.Enums.ScoreType)int.Parse(Request.Form["JobType"]));
            }
            if (!string.IsNullOrWhiteSpace(Request.Form["Year"]))
            {
                AdminScores = AdminScores.Where(item => item.Year == Request.Form["Year"]);
            }
            if (!string.IsNullOrWhiteSpace(Request.Form["Month"]))
            {
                AdminScores = AdminScores.Where(item => item.Month == Request.Form["Month"]);
            }
            var linq = AdminScores.ToList().Select(item => new
            {
                用户名 = item.Admin.RealName,
                考核角色 = item.ScoreType.GetDescription(),
                地区 = string.Join(",", item.DistrictNames),
                客户拜访数 = item.ReportScore,
                新客户数销售机会数 = item.ScoreType == NtErp.Crm.Services.Enums.ScoreType.Sales ? item.ClientScore : item.ProjectScore,
                DI个数 = item.DIScore,
                DW个数 = item.DWScore,
                绩效分 = item.TotalScore,
                绩效工资 = item.Bonus,
                年份 = item.Year,
                月份 = item.Month,
            });
            //var data = this.ToDataTable(linq);
            //var URL = NtErp.Crm.Services.NPOIHelper.DataToExcel(data, "员工绩效考核统计");
            var fileurl = NtErp.Crm.Services.NPOIHelper.EnumrableToExcel(linq, "员工绩效考核统计");
            Response.Write(Request.ApplicationPath + fileurl);
        }

        #endregion
    }
}