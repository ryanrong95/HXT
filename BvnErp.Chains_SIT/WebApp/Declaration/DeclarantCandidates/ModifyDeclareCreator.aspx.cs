using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.DeclarantCandidates
{
    public partial class ModifyDeclareCreator : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string OrderID = Request.QueryString["OrderID"]?.Trim();
            string ClientName = Request.QueryString["ClientName"];
            string VoyageID = Request.QueryString["VoyageID"];
            string MyDecNotice = Request.QueryString["MyDecNotice"];

            using (var query = new Needs.Ccs.Services.Views.ModifyDeclareCreatorListView())
            {
                var view = query;

                if (!string.IsNullOrWhiteSpace(OrderID))
                {
                    OrderID = OrderID.Trim();
                    view = view.SearchByOrderID(OrderID);
                }

                if (!string.IsNullOrEmpty(ClientName))
                {
                    ClientName = ClientName.Trim();
                    view = view.SearchByClientName(ClientName);
                }

                if (!string.IsNullOrEmpty(VoyageID))
                {
                    VoyageID = VoyageID.Trim();
                    view = view.SearchByVoyageID(VoyageID);
                }

                bool MyDecNoticeBoolean = false;
                if (bool.TryParse(MyDecNotice, out MyDecNoticeBoolean))
                {
                    if (MyDecNoticeBoolean)
                    {
                        var adminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                        view = view.SearchByDeclareCreatorID(adminID);
                    }
                }

                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }

    }
}