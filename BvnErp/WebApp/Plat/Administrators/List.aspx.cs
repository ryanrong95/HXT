using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NtErp.Services.Views;
using NtErp.Services.Models;
using System.Linq.Expressions;
using Needs.Web;
using Needs.Utils.Linq;

namespace WebApp.Plat.Administrators
{
    public partial class List : Needs.Web.Sso.Forms.ErpPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            string key = Request["key"] ?? "";


            Expression<Func<Admin, bool>> exp = item => item.UserName != "sa";

            if (!string.IsNullOrEmpty(key))
            {
                exp = exp.And((item => item.ID == key || item.UserName.StartsWith(key) || item.RealName.Contains(key)));
            }

            Response.Paging(Needs.Erp.ErpPlot.Current.Plots.Admins.Where(item => item.Status == Needs.Erp.Generic.Status.Normal)
                .Where(exp).ToList().Select(item => new { item.ID, item.UserName, item.RealName, CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"), UpdateDate = item.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss"), item.Status, item.Summary }));
        }

        protected void del()
        {
            try
            {
                var id = Request["id"];

                var entity = Needs.Erp.ErpPlot.Current.Plots.Admins[id];
                if (entity != null)
                {

                    entity.Abandon();
                }
                Response.Write("1");
            }
            catch
            {

            }

        }
    }
}