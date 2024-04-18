using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq.Expressions;
using Needs.Web;

namespace WebApp.Plat.Subordinates
{
    public partial class List : Needs.Web.Sso.Forms.ErpPage
    {
        protected string[] staffs;
        protected void Page_Load(object sender, EventArgs e)
        {
          
        }

        protected void data()
        {
            string key = Request["key"] ?? "";


            Func<NtErp.Services.Models.Admin, bool> exp = item => item.Status==Needs.Erp.Generic.Status.Normal;

            if (!string.IsNullOrEmpty(key))
            {
                exp = exp + ((item => item.ID == key || item.UserName.StartsWith(key) || item.RealName.Contains(key)));
            }
            Response.Paging(Needs.Erp.ErpPlot.Current.Plots.MyStaffs.Where(exp).Select(item => new { item.ID, item.UserName, item.RealName, item.Status }));
        }


        
    }
}