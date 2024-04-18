using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Client
{
    public partial class Logs : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void data()
        {
            string ID = Request.QueryString["ID"];
            var list = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientLogs.Where(item => item.ClientID == ID).OrderByDescending(item=>item.CreateDate).AsQueryable();
            Func<Needs.Ccs.Services.Models.ClientLog, object> convert = item => new
            {
                ID = item.ID,
                Client = item.ClientID,
                Admin = item.Admin.RealName,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Summary = item.Summary
            };
            this.Paging(list, convert);
        }
    }
}