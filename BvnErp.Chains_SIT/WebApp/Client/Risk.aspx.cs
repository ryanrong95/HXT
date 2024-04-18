using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Client
{
    public partial class Risk : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            string id = Request.QueryString["ID"];

            string Source = Request.QueryString["Source"];
            string CompanyName= Request.QueryString["name"];
            this.Model.CompanyName = CompanyName ?? "";
            this.Model.ID = id ?? "";
            this.Model.Source = Source;

            if (!string.IsNullOrEmpty(id))
            {
                // this.Model.ServiceType = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView[id].ServiceType.GetHashCode();
                var client = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView[id];
                this.Model.entity = new { ID = client.ID, ServiceType = client.ServiceType, client.ClientStatus }.Json();
            }
            else
            {
                this.Model.entity = null;

            }

        }
    }
}