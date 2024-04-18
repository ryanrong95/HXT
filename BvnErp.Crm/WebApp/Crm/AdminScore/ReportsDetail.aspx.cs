using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.AdminScore
{
    public partial class ReportsDetail : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                var admins = new NtErp.Crm.Services.Views.AdminTopView().Where(item => item.JobType != 0).Select(item => new { item.ID, item.RealName });
                this.Model.Admins = admins.Json();
                this.Model.Year = new NtErp.Crm.Services.Views.AdminScoreAlls().Select(item => new { value = item.Year, text = item.Year }).Distinct().Json();
                this.Model.Month = new NtErp.Crm.Services.Views.AdminScoreAlls().Select(item => new { value = item.Month, text = item.Month }).Distinct().Json();
            }
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        protected void data()
        {
            string AdminID = Request.QueryString["AdminID"];
            string Year = Request.QueryString["Year"];
            string Month = Request.QueryString["Month"];

            var reports = new NtErp.Crm.Services.Views.ReportsAlls().Where(item =>item.Client != null && item.Status == Status.Normal)
                .Where(item=>!item.Context.Contains(@"""Type"":30"));
            if (!string.IsNullOrWhiteSpace(AdminID))
            {
                reports = reports.Where(item => item.Admin.ID == AdminID);
            }
            var data = reports.ToList().Select(item => new
            {
                item.ID,
                JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(item.Context).Date,
                JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(item.Context).NextDate,
                AdminName = item.Admin.RealName,
                ClientName = item.Client.Name,
                Type = JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(item.Context).Type,
                TypeName = JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(item.Context).Type.GetDescription(),
                Context = JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(item.Context).Content,
            });
            data = data.Where(item => item.Date.Value.Year == int.Parse(Year))
                .Where(item => item.Date.Value.Month == int.Parse(Month));
            this.Paging(data);
        }

        #region 数据导出
        /// <summary>
        /// 数据导出
        /// </summary>
        protected void Export()
        {
            var reports = new NtErp.Crm.Services.Views.ReportsAlls().Where(item => item.Client != null && item.Status == Status.Normal)
                .Where(item => !item.Context.Contains(@"""Type"":30"));
            if (!string.IsNullOrWhiteSpace(Request.Form["AdminID"]))
            {
                reports = reports.Where(item => item.Admin.ID == Request.Form["AdminID"]);
            }
            var data = reports.ToList().Select(item => new
            {
                item.ID,
                跟进日期 = JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(item.Context).Date,
                下次跟进日期 = JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(item.Context).NextDate,
                跟进人 = item.Admin.RealName,
                客户名称 = item.Client.Name,
                跟进方式 = JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(item.Context).Type.GetDescription(),
            });
            data = data.Where(item => item.跟进日期.Value.Year == int.Parse(Request.Form["Year"]))
                .Where(item => item.跟进日期.Value.Month == int.Parse(Request.Form["Month"]));
            var fileurl = NtErp.Crm.Services.NPOIHelper.EnumrableToExcel(data, "员工跟踪记录考核统计");
            Response.Write(Request.ApplicationPath + fileurl);
        }

        #endregion
    }
}