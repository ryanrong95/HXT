using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.AdvanceMoney.ExpireOrder
{
    public partial class ExpireOrder : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);
            string adminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                
            using (var query = new Needs.Ccs.Services.Views.ExpireOrderView())
            {
                var view = query;
                if (!string.IsNullOrEmpty(adminID))
                {
                    view = view.SearchByAdminID(adminID);
                }                
                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }
    }
}