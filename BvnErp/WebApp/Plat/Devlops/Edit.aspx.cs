using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Plat.Devlops
{
    public partial class Edit : Needs.Web.Sso.Forms.ErpPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.PageInit();
            }
        }

        void PageInit()
        {
            string id = Request.QueryString["id"];
            this.Model = Needs.Overall.Devlopers.Currents[id];
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var context = Request["txtContext"];
            string id = Request.QueryString["id"];
            var old = Needs.Overall.Devlopers.Currents[id];
            string template = $@"{Needs.Erp.ErpPlot.Current.RealName},{Needs.Erp.ErpPlot.Current.UserName},{DateTime.Now}
{context.Trim()}

{old.Context}".Trim();

            Needs.Overall.Devlopers.Currents[id].Note(template);
            Alert(this.hSucess.Value, Request.Url);
        }
    }
}