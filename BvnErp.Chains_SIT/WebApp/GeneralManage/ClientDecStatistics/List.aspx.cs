using Needs.Ccs.Services;
using Needs.Ccs.Services.Views;
using Needs.Utils;
using Needs.Utils.Npoi;
using Needs.Utils.Serializers;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.GeneralManage.ClientDecStatistics
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string ClientCode = Request.QueryString["ClientCode"];
            string ClientName = Request.QueryString["ClientName"];
            string DDataBegin = Request.QueryString["DDataBegin"];
            string DDataEnd = Request.QueryString["DDataEnd"];

            List<LambdaExpression> lamdas = new List<LambdaExpression>();

            if (!string.IsNullOrWhiteSpace(ClientCode))
            {
                Expression<Func<ClientDecStatisticsListViewModel, bool>> lambda1 = item => item.ClientCode.Contains(ClientCode);
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrWhiteSpace(ClientName))
            {
                Expression<Func<ClientDecStatisticsListViewModel, bool>> lambda1 = item => item.CompanyName.Contains(ClientName);
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrWhiteSpace(DDataBegin))
            {
                DateTime begin = DateTime.Parse(DDataBegin);

                Expression<Func<ClientDecStatisticsListViewModel, bool>> lambda1 = item => item.DDate >= begin;
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrWhiteSpace(DDataEnd))
            {
                DateTime end = DateTime.Parse(DDataEnd);
                end = end.AddDays(1);

                Expression<Func<ClientDecStatisticsListViewModel, bool>> lambda1 = item => item.DDate < end;
                lamdas.Add(lambda1);
            }

            using (var query = new ClientDecStatisticsListView(lamdas.ToArray()))
            {
                var view = query;

                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }

        /// <summary>
        /// 合计行
        /// </summary>
        protected void TotalData()
        {
            //int page, rows;
            //int.TryParse(Request.QueryString["page"], out page);
            //int.TryParse(Request.QueryString["rows"], out rows);

            string ClientCode = Request.Form["ClientCode"];
            string ClientName = Request.Form["ClientName"];
            string DDataBegin = Request.Form["DDataBegin"];
            string DDataEnd = Request.Form["DDataEnd"];

            List<LambdaExpression> lamdas = new List<LambdaExpression>();

            if (!string.IsNullOrWhiteSpace(ClientCode))
            {
                Expression<Func<ClientDecStatisticsListViewModel, bool>> lambda1 = item => item.ClientCode.Contains(ClientCode);
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrWhiteSpace(ClientName))
            {
                Expression<Func<ClientDecStatisticsListViewModel, bool>> lambda1 = item => item.CompanyName.Contains(ClientName);
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrWhiteSpace(DDataBegin))
            {
                DateTime begin = DateTime.Parse(DDataBegin);

                Expression<Func<ClientDecStatisticsListViewModel, bool>> lambda1 = item => item.DDate >= begin;
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrWhiteSpace(DDataEnd))
            {
                DateTime end = DateTime.Parse(DDataEnd);
                end = end.AddDays(1);

                Expression<Func<ClientDecStatisticsListViewModel, bool>> lambda1 = item => item.DDate < end;
                lamdas.Add(lambda1);
            }

            using (var query = new ClientDecStatisticsListView(lamdas.ToArray()))
            {
                var view = query;

                var result = view.ToTotalData();

                var message = "合计：";
                foreach (var r in result)
                {
                    message += "&nbsp;&nbsp;&nbsp;&nbsp;" + r.DecPriceTotal.ToString("0.00") + "&nbsp;" + r.Currency;
                }
                Response.Write(message);
            }
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        protected void ExportExcel() 
        {
            try
            {
                string ClientCode = Request.Form["ClientCode"];
                string ClientName = Request.Form["ClientName"];
                string DDataBegin = Request.Form["DDataBegin"];
                string DDataEnd = Request.Form["DDataEnd"];

                List<LambdaExpression> lamdas = new List<LambdaExpression>();

                if (!string.IsNullOrWhiteSpace(ClientCode))
                {
                    Expression<Func<ClientDecStatisticsListViewModel, bool>> lambda1 = item => item.ClientCode.Contains(ClientCode);
                    lamdas.Add(lambda1);
                }
                if (!string.IsNullOrWhiteSpace(ClientName))
                {
                    Expression<Func<ClientDecStatisticsListViewModel, bool>> lambda1 = item => item.CompanyName.Contains(ClientName);
                    lamdas.Add(lambda1);
                }
                if (!string.IsNullOrWhiteSpace(DDataBegin))
                {
                    DateTime begin = DateTime.Parse(DDataBegin);

                    Expression<Func<ClientDecStatisticsListViewModel, bool>> lambda1 = item => item.DDate >= begin;
                    lamdas.Add(lambda1);
                }
                if (!string.IsNullOrWhiteSpace(DDataEnd))
                {
                    DateTime end = DateTime.Parse(DDataEnd);
                    end = end.AddDays(1);

                    Expression<Func<ClientDecStatisticsListViewModel, bool>> lambda1 = item => item.DDate < end;
                    lamdas.Add(lambda1);
                }

                var query = new ClientDecStatisticsListView(lamdas.ToArray());

                #region 报关量统计
                var detailData = query.ToGetAll().Select(d => new
                {
                    客户编号 = d.ClientCode,
                    客户名称 = d.CompanyName,
                    注册日期 = d.CreateDate.ToString("yyyy-MM-dd"),
                    代理费率 = d.AgentRate,
                    开票类型 = d.InvoiceType == (int)Needs.Ccs.Services.Enums.InvoiceType.Full ? "全额发票" : "服务费发票",
                    报关金额 = d.DecPriceTotal.ToRound(2),
                    币种 = d.Currency,
                    业务员 = d.ServiceManagerName
                });
                #endregion

                //文件
                string filename = "报关量统计表" + DateTime.Now.Ticks + ".xlsx";
                FileDirectory fileDic5 = new FileDirectory(filename);
                fileDic5.SetChildFolder(Needs.Ccs.Services.SysConfig.Export);
                fileDic5.CreateDataDirectory();

                var templatePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Needs.Ccs.Services.SysConfig.ExportClientDeclareTotal);
                using (FileStream file = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
                {
                    XSSFWorkbook workbook = new XSSFWorkbook(file);
                    NPOIHelper npoi = new NPOIHelper(workbook);

                    npoi.SetSheet("报关量统计");
                    npoi.GenerateExcelByTemplate(detailData, 1);
                    npoi.SaveAs(fileDic5.FilePath);
                }

                Response.Write((new { success = true, message = "导出成功", url = fileDic5.FileUrl }).Json());

            }
            catch (Exception ex)
            {

            }

        }

    }
}