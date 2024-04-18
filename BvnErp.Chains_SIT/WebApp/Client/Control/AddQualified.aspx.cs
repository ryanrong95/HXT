using Needs.Ccs.Services.Enums;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Client.Control
{
    public partial class AddQualified : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }


        public void LoadData()
        {
            var clientsView = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.SuperAdminClientsView;

            var clients = clientsView.AsQueryable().Where(x => x.Status == Needs.Ccs.Services.Enums.Status.Normal
            && x.ClientStatus == ClientStatus.Confirmed
            && x.ServiceType != ServiceType.Warehouse
            && (x.IsQualified == false || Nullable<bool>.Equals(x.IsQualified, null)));


            this.Model.Clients = clients.Select(item => new { Key = item.ID, Value = item.ClientCode + "-" + item.Company.Name }).Json();
        }


        /// <summary>
        /// 
        /// </summary>
        protected void SaveQualifiedClient()
        {
            try
            {
                string id = Request.Form["ID"];

                var entity = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.SuperAdminClientsView[id];
                entity.IsQualified = true;
                entity.SetQualified();

                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = ex.Message }).Json());

            }
        }
    }
}
