using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.DollarEquityApply
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Dictionary<string, string> dicIsPaidOption = new Dictionary<string, string>();
            dicIsPaidOption.Add("1", "是");
            dicIsPaidOption.Add("0", "否");
            this.Model.IsPaidOption = dicIsPaidOption.Select(item => new { Value = item.Key, Text = item.Value }).Json();
        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string IsPaid = Request.QueryString["IsPaid"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];

            using (var query = new Needs.Ccs.Services.Views.DollarEquityApplyListView())
            {
                var view = query;

                if (!string.IsNullOrWhiteSpace(IsPaid))
                {
                    bool isPaidBoolean = (IsPaid == "1");
                    view = view.SearchByIsPaid(isPaidBoolean);
                }

                if (!string.IsNullOrWhiteSpace(StartDate))
                {
                    DateTime begin = DateTime.Parse(StartDate);
                    view = view.SearchByStartDate(begin);
                }

                if (!string.IsNullOrWhiteSpace(EndDate))
                {
                    DateTime end = DateTime.Parse(EndDate);
                    end = end.AddDays(1);
                    view = view.SearchByEndDate(end);
                }

                Response.Write(view.ToMyPage(page, rows).Json());
            }

        }

        protected void GetBalanceValue()
        {
            string BalanceValueStr = "";
            var balanceModel = new Needs.Ccs.Services.Views.BalanceValueView().FirstOrDefault();

            if (balanceModel != null)
            {
                BalanceValueStr = Convert.ToString(balanceModel.Balance);
            }

            Response.Write((new { success = true, BalanceValue = BalanceValueStr }).Json());
        }

    }
}