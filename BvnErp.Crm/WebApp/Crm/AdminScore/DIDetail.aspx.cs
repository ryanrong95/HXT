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
    public partial class DIDetail : Uc.PageBase
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
        /// 查询数据
        /// </summary>
        protected void data()
        {
            string AdminID = Request.QueryString["AdminID"];
            string Year = Request.QueryString["Year"];
            string Month = Request.QueryString["Month"];
            var products = new NtErp.Crm.Services.Views.MyExamProductView(AdminID);
            //获取当月状态审批为DI的产品
            var applies = new NtErp.Crm.Services.Views.ApplyAlls().Where(item => item.Status == ApplyStatus.Approval).
                Where(item => item.Type == ApplyType.DIApply).Where(item => item.UpdateDate.Year == int.Parse(Year)).
                Where(item => item.UpdateDate.Month == int.Parse(Month)).ToArray();
            var DIids = applies.Select(item => item.MainID);

            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            var data = products.GetPageList(page, rows, item => DIids.Contains(item.ID));
            var linq = from product in data
                       join apply in applies on product.ID equals apply.MainID
                       select new
                       {
                           product.ProjectName,
                           product.ClientName,
                           product.AdminName,
                           product.StandProductName,
                           product.ManufactureName,
                           product.RefUnitQuantity,
                           product.RefQuantity,
                           product.RefUnitPrice,
                           product.PMAdminName,
                           product.FaeAdminName,
                           StatusName = product.Status.GetDescription(),
                           ApproveDate = apply.UpdateDate.ToString("yyyy-MM-dd hh:mm:ss"),
                       };
            Response.Write(new
            {
                rows = linq.ToArray(),
                total = linq.Count(),
            }.Json());
        }

        #region 数据导出
        /// <summary>
        /// 数据导出
        /// </summary>
        protected void Export()
        {
            string AdminID = Request.Form["AdminID"];
            string Year = Request.Form["Year"];
            string Month = Request.Form["Month"];
            var products = new NtErp.Crm.Services.Views.MyExamProductView(AdminID);
            //获取当月状态审批为DW的产品
            var applies = new NtErp.Crm.Services.Views.ApplyAlls().Where(item => item.Status == ApplyStatus.Approval).
                Where(item => item.Type == ApplyType.DIApply).Where(item => item.UpdateDate.Year == int.Parse(Year)).
                Where(item => item.UpdateDate.Month == int.Parse(Month)).ToArray();
            var DIids = applies.Select(item => item.MainID);

            var data = products.GetTop(10000, item => DIids.Contains(item.ID));
            var linq = from product in data
                       join apply in applies on product.ID equals apply.MainID
                       select new
                       {
                           审批时间 = apply.UpdateDate.ToString("yyyy-MM-dd hh:mm:ss"),
                           销售机会 = product.ProjectName,
                           客户 = product.ClientName,
                           机会建立人 = product.AdminName,
                           PM = product.PMAdminName,
                           FAE = product.FaeAdminName,
                           产品型号 = product.StandProductName,
                           品牌 = product.ManufactureName,
                           单机用量 = product.RefUnitQuantity,
                           项目用量 = product.RefQuantity,
                           参考单价 = product.RefUnitPrice,
                           销售状态 = product.Status.GetDescription(),
                       };

            var fileurl = NtErp.Crm.Services.NPOIHelper.EnumrableToExcel(linq, "员工产品DI数量考核统计");
            Response.Write(Request.ApplicationPath + fileurl);
        }

        #endregion
    }
}