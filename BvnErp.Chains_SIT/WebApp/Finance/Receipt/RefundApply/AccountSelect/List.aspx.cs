using System;
using Needs.Ccs.Services.Enums;
using Needs.Utils.Serializers;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Receipt.RefundApply.AccountSelect
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void LoadComboboxData()
        {
            var applyStatus = Needs.Utils.Descriptions.EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.RefundApplyStatus>().Select(item => new { item.Key, item.Value });
            this.Model.RefundApplyStatus = applyStatus.Json();
        }
        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
            string AccountName = Request.QueryString["ClientName"];
           


            List<LambdaExpression> lamdas = new List<LambdaExpression>();

            using (var query = new Needs.Ccs.Services.Views.RefundApplyView())
            {
                var view = query;
                view = view.SearchByApplyStatus(RefundApplyStatus.Approved);
                view = view.SearchPayeeAccountIsNull();

                if (!string.IsNullOrWhiteSpace(StartDate))
                {
                    StartDate = StartDate.Trim();
                    DateTime dtFrom = Convert.ToDateTime(StartDate);
                    view = view.SearchByFrom(dtFrom);
                }

                if (!string.IsNullOrWhiteSpace(EndDate))
                {
                    EndDate = EndDate.Trim();
                    DateTime dtTo = Convert.ToDateTime(EndDate).AddDays(1);
                    view = view.SearchByTo(dtTo);
                }

                if (!string.IsNullOrEmpty(AccountName))
                {
                    AccountName = AccountName.Trim();
                    view = view.SearchByAccountName(AccountName);
                }

                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }
    }
}