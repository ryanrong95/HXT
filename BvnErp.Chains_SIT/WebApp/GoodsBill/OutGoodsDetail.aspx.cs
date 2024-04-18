using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.GoodsBill
{
    public partial class OutGoodsDetail : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string currentDate = Request.QueryString["CurrentDate"];
            string previousDate = Request.QueryString["PreviousDate"];
            string ClientID = Request.QueryString["ClientID"];

            List<LambdaExpression> lamdas = new List<LambdaExpression>();

            using (var query = new Needs.Ccs.Services.Views.OutGoodsDetailView())
            {
                var view = query;

                view = view.SearchByClientID(ClientID);


                if (!string.IsNullOrWhiteSpace(currentDate))
                {
                    currentDate = currentDate.Trim() + "-01";
                    DateTime dtFrom = Convert.ToDateTime(currentDate);
                    view = view.SearchByCurrent(dtFrom);
                }

                if (!string.IsNullOrWhiteSpace(previousDate))
                {
                    previousDate = previousDate.Trim() + "-01"; ;
                    DateTime dtTo = Convert.ToDateTime(previousDate);
                    view = view.SearchByPrevious(dtTo);
                }

                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }
    }
}