using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Client
{
    public partial class Index : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            string id = Request.QueryString["ID"];

            string Source = Request.QueryString["Source"];
            string CompanyName = Request.QueryString["name"];
            string AdminID = Request.QueryString["adminId"];

            this.Model.CompanyName = CompanyName ?? "";
            this.Model.ID = id ?? "";
            this.Model.Source = Source ?? "";
            this.Model.AdminID = AdminID ?? "";

            if (!string.IsNullOrEmpty(id))
            {
                var adminId = AdminID == null ? Needs.Wl.Admin.Plat.AdminPlat.Current.ID : AdminID;

                this.Model.ServiceType = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView[id].ServiceType.GetHashCode();
                if (Source == "View")
                {
                    this.Model.isServiceManager = adminId == Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView[id].Merchandiser?.ID;
                    //this.Model.isStorageServiceManager = adminId == Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView[id].StorageMerchandiser?.ID;
                }
                else
                {
                    this.Model.isServiceManager = adminId == Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView[id].ServiceManager?.ID;
                    //this.Model.isStorageServiceManager = adminId == Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView[id].StorageServiceManager?.ID;
                }

            }
            else
            {
                this.Model.ServiceType = null;
                this.Model.isServiceManager = false;
                this.Model.isStorageServiceManager = false;
            }
        }
    }
}
