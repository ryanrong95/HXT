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
    public partial class ClientsDetail : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var admins = new NtErp.Crm.Services.Views.AdminTopView().Where(item => item.JobType != 0).Select(item => new { item.ID, item.RealName });
                this.Model.Admins = admins.Json();
                this.Model.Year = new NtErp.Crm.Services.Views.AdminScoreAlls().Select(item => new { value = item.Year, text = item.Year }).Distinct().Json();
                this.Model.Month = new NtErp.Crm.Services.Views.AdminScoreAlls().Select(item => new { value = item.Month, text = item.Month }).Distinct().Json();
            }
        }


        /// <summary>
        /// 数据查询
        /// </summary>
        protected void data()
        {
            var adminid = Request.QueryString["AdminID"];
            string Year = Request.QueryString["Year"];
            string Month = Request.QueryString["Month"];
            var reports = new NtErp.Crm.Services.Views.ReportsAlls().Where(item => item.Client != null && item.Status == Status.Normal)
                .Where(item => !item.Context.Contains(@"""Type"":30"));
            if (!string.IsNullOrWhiteSpace(adminid))
            {
                reports = reports.Where(item => item.Admin.ID == adminid);
            }
            var linq = reports.ToList().Select(item => new
            {
                item.ID,
                ClientID = item.Client.ID,
                JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(item.Context).Date,
                Type = JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(item.Context).Type,
                Context = JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(item.Context).Content,
            }).OrderBy(item => item.Date);

            //按照客户分组,取出第一笔数据
            var grouptemp = from item in linq
                            group new { item.ID, item.ClientID, item.Date } by item.ClientID into g
                            select new
                            {
                                g.FirstOrDefault().ID,
                                g.FirstOrDefault().ClientID,
                                g.FirstOrDefault().Date,
                            };
            var ids = grouptemp.Where(item => item.Date.Value.Year == int.Parse(Year)).
                Where(item => item.Date.Value.Month == int.Parse(Month)).Select(item => item.ClientID).ToArray();
            var clients = Needs.Erp.ErpPlot.Current.ClientSolutions.Clients.Where(item => ids.Contains(item.ID));
            var data = clients.ToList().Select(item => new
            {
                item.ID,
                item.Name,
                StatusName = item.Status.GetDescription(),
                item.CreateDate,
                item.UpdateDate,
            });
            this.Paging(data);
        }


        #region 数据导出
        /// <summary>
        /// 数据导出
        /// </summary>
        protected void Export()
        {
            string Year = Request.Form["Year"];
            string Month = Request.Form["Month"];
            var reports = new NtErp.Crm.Services.Views.ReportsAlls().Where(item => item.Client != null && item.Status == Status.Normal)
                .Where(item => !item.Context.Contains(@"""Type"":30"));
            if (!string.IsNullOrWhiteSpace(Request.Form["AdminID"]))
            {
                reports = reports.Where(item => item.Admin.ID == Request.Form["AdminID"]);
            }
            var linq = reports.ToList().Select(item => new
            {
                item.ID,
                ClientID = item.Client.ID,
                JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(item.Context).Date,
                Type = JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(item.Context).Type,
                Context = JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(item.Context).Content,
            });

            //按照客户分组,取出第一笔数据
            var grouptemp = from item in linq
                            group new { item.ID, item.ClientID, item.Date } by item.ClientID into g
                            orderby g.FirstOrDefault().Date descending
                            select new
                            {
                                g.FirstOrDefault().ID,
                                g.FirstOrDefault().ClientID,
                                g.FirstOrDefault().Date,
                            };
            var ids = grouptemp.Where(item => item.Date.Value.Year == int.Parse(Year)).
                Where(item => item.Date.Value.Month == int.Parse(Month)).Select(item => item.ClientID).ToArray();
            var clients = Needs.Erp.ErpPlot.Current.ClientSolutions.Clients.Where(item => ids.Contains(item.ID));
            var data = clients.ToList().Select(item => new
            {
                客户ID = item.ID,
                客户名称 = item.Name,
                状态 = item.Status.GetDescription(),
                创建时间 = item.CreateDate,
                更新时间 = item.UpdateDate,
            });

            var fileurl = NtErp.Crm.Services.NPOIHelper.EnumrableToExcel(data, "新客户考核统计");
            Response.Write(Request.ApplicationPath + fileurl);
        }

        #endregion
    }
}