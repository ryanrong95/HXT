using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.DeclarantCandidates
{
    public partial class ModifyCustomSubmiter : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string ContrNO = Request.QueryString["ContrNo"];
            string OrderID = Request.QueryString["OrderID"];

            using (var query = new Needs.Ccs.Services.Views.ModifyCustomSubmiterListView())
            {
                var view = query;

                if (!string.IsNullOrWhiteSpace(ContrNO))
                {
                    ContrNO = ContrNO.Trim();
                    view = view.SearchByContractID(ContrNO);
                }

                if (!string.IsNullOrWhiteSpace(OrderID))
                {
                    OrderID = OrderID.Trim();
                    view = view.SearchByOrderID(OrderID);
                }

                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }

    }
}