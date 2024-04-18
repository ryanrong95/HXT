using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace WebApp.ReportManage.ClassifiedStatistics
{
    /// <summary>
    /// 归类统计页面
    /// </summary>
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 初始化归类产品数据
        /// </summary>
        protected void data()
        {
            //int page, rows;
            //int.TryParse(Request.QueryString["page"], out page);
            //int.TryParse(Request.QueryString["rows"], out rows);
            //归类时间
            DateTime startDate = DateTime.Parse(Request.QueryString["StartDate"]);
            DateTime endDate = DateTime.Parse(Request.QueryString["EndDate"]);

            var query = new Needs.Ccs.Services.Models.ClassifiedStatisticExcel().Query(startDate, endDate.AddDays(1));
            Response.Write(new
            {
                rows = query.ToArray(),
                //total = total
            }.Json());
            //Response.Write(new { rows = 0, total = 0 }.Json());
        }

        /// <summary>
        /// 导出归类统计数据
        /// </summary>
        protected void ExportExcel()
        {
            try
            {
                //归类时间
                DateTime startDate = DateTime.Parse(Request.Form["StartDate"]);
                DateTime endDate = DateTime.Parse(Request.Form["EndDate"]);
                //导出Excel
                var excelUrl = new Needs.Ccs.Services.Models.ClassifiedStatisticExcel().Export(startDate, endDate.AddDays(1));

                Response.Write(new { result = true, info = "导出成功", url = excelUrl }.Json());
            }
            catch (Exception ex)
            {
                Response.Write(new { result = false, info = $"导出失败：{ex.Message}" }.Json());
            }
        }
    }
}