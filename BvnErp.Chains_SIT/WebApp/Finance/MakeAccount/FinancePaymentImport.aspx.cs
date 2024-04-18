using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.MakeAccount
{
    public partial class FinancePaymentImport : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Load_Data();
        }

        protected void Load_Data()
        {

            
        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);
         
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];

          

            using (var query = new Needs.Ccs.Services.Views.FinancePaymentViewRJ())
            {
                var view = query;

                view = view.SearchByCreSta(false);
                view = view.SearchByPayeeName("暂收款");

                if (!string.IsNullOrWhiteSpace(StartDate))
                {
                    StartDate = StartDate.Trim();
                    var from = DateTime.Parse(StartDate);
                    view = view.SearchByFrom(from);
                }

                if (!string.IsNullOrWhiteSpace(EndDate))
                {
                    EndDate = EndDate.Trim();
                    var to = DateTime.Parse(EndDate);
                    view = view.SearchByTo(to);
                }

                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }

        /// <summary>
        /// 生成凭证
        /// </summary>
        protected void MakeAccount()
        {

            string Model = Request.Form["Model"].Replace("&quot;", "\'");
            var model = Model.JsonTo<List<FinPReportItem>>();

            var result = new Needs.Ccs.Services.Models.FinancePimport(model).Make();

            Response.Write((new { success = result }).Json());

        }

        protected void MakeAccountAll()
        {
            string startDate = Request.Form["StartDate"];
            string endDate = Request.Form["EndDate"];

            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                DateTime from = DateTime.Parse(startDate);
                DateTime to = DateTime.Parse(endDate).AddDays(1);
                TimeSpan day = to.Subtract(from);
                if (day.TotalDays > 31)
                {
                    Response.Write((new { success = false, msg = "不能一次生成超一个月的数据" }).Json());
                    return;
                }
            }
            else
            {
                Response.Write((new { success = false, msg = "必须勾选开始结束日期" }).Json());
                return;
            }

            DateTime sDate = DateTime.Parse(startDate);
            DateTime eDate = DateTime.Parse(endDate).AddDays(1);
            var query = new Needs.Ccs.Services.Views.FinancePaymentViewRJ().
                            Where(t => t.FinPCreSta == false && t.PayeeName.Contains("暂收款") && t.PayDate > sDate && t.PayDate < eDate).ToArray();

            List<FinPReportItem> model = new List<FinPReportItem>();

            foreach (var item in query)
            {
                model.Add(new FinPReportItem
                {
                    ID = item.ID,
                    Amount = item.Amount,
                    BankName = item.BankName,
                    FinanceAccount = item.FinanceAccountName,
                    PayDate = item.PayDate
                });
            }

            var result = new Needs.Ccs.Services.Models.FinancePimport(model).Make();
            Response.Write((new { success = result }).Json());
        }
    }
}