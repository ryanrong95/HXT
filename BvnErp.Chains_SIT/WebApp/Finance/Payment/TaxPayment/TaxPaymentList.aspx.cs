using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Payment.TaxPayment
{
    public partial class TaxPaymentList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var decTaxType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.DecTaxType>().Select(item => new { item.Key, item.Value });
            this.Model.DecTaxType = decTaxType.Json();
        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string DDateStartDate = Request.QueryString["DDateStartDate"];
            string DDateEndDate = Request.QueryString["DDateEndDate"];
            string TaxType = Request.QueryString["TaxType"];

            using (var query = new Needs.Ccs.Services.Views.TaxPaymentListView())
            {
                var view = query;

                if (!string.IsNullOrEmpty(DDateStartDate))
                {
                    DateTime begin = DateTime.Parse(DDateStartDate);
                    view = view.SearchByDDateStartDate(begin);
                }
                if (!string.IsNullOrEmpty(DDateEndDate))
                {
                    DateTime end = DateTime.Parse(DDateEndDate);
                    end = end.AddDays(1);
                    view = view.SearchByDDateEndDate(end);
                }
                if (!string.IsNullOrEmpty(TaxType))
                {
                    Needs.Ccs.Services.Enums.DecTaxType decTaxType = (Needs.Ccs.Services.Enums.DecTaxType)int.Parse(TaxType);
                    view = view.SearchByDecTaxType(decTaxType);
                }

                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }



    }
}