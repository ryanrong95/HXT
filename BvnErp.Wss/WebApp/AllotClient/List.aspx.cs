using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Web;
using System.Linq.Expressions;
using Needs.Utils.Linq;

namespace WebApp.AllotClient
{
    public partial class List : Needs.Web.Sso.Forms.ErpPage
    {

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            string key = Request["key"] ?? "";
            var pid = Request["pid"];
            //var relations = Needs.Erp.ErpPlot.Current.Limits.AdminsAll[pid].Websites.MyClients.Select(item => item.ID).ToArray();
            var relations = Needs.Erp.ErpPlot.Current.Plots.MapAdminClients.Where(item=>item.AdminID==pid).Select(item => item.ClientID).ToArray();
            Expression<Func<NtErp.Wss.Services.Generic.Models.ClientTop, bool>> exp = (item => item.Status == Needs.Erp.Generic.Status.Normal);
            if (!string.IsNullOrEmpty(key))
            {
                exp = exp.And((item => item.ID == key || item.UserName.StartsWith(key) || item.Mobile == key || item.Email.StartsWith(key)));
            }
            Response.Paging(Needs.Erp.ErpPlot.Current.Websites.ClientAll.Where(exp).Select(item => new { item.ID, item.UserName, item.Mobile, item.Email, item.Status, Checked = relations.Contains(item.ID) ? true : false }));
        }

        protected void Enter()
        {
            var id = Request["id"];

            var pid = Request["pid"];
            //Needs.Erp.ErpPlot.Current.Limits.AdminsAll[pid].Plots.Bind(id);
            Needs.Erp.ErpPlot.Current.Websites.MyClients.Bind(id,pid);
        }

        protected void Remove()
        {
            var id = Request["id"];
            var pid = Request["pid"];
            Needs.Erp.ErpPlot.Current.Websites.MyClients.UnBind(id,pid);
            //Needs.Erp.ErpPlot.Current.Limits.AdminsAll[pid].Plots.UnBind(id);
        }

    }
}