using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Web;
using Needs.Erp.Models;
using System.Linq.Expressions;
using Needs.Utils.Linq;

namespace WebApp.Plat.AllotStaff
{
    public partial class List : Needs.Web.Sso.Forms.ErpPage
    {
        protected string[] staffs;
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        protected void data()
        {
            var pid = Request["pid"];

            if (string.IsNullOrEmpty(pid))
            {
                throw new NotImplementedException("参数错误");
            }

            string key = Request["key"] ?? "";

            staffs = Needs.Erp.ErpPlot.Current.Limits.GetAdmin(pid).Plots.MyStaffs.Select(item => item.ID).ToArray();

          Expression<Func<Admin, bool>> exp = item => item.ID != Needs.Erp.ErpPlot.Current.ID;

            if (!string.IsNullOrEmpty(key))
            {
                exp = exp.And((item => item.ID == key || item.UserName.StartsWith(key) || item.RealName.Contains(key)));
            }
            Response.Paging(Needs.Erp.ErpPlot.Current.Limits.Admins.Where(exp).ToList().Select(item => new { item.ID, item.UserName, item.RealName,Checked = staffs.Contains(item.ID) ? true : false }));
        }


        protected void enter()
        {
            var id = Request["id"];

            var pid = Request["pid"];
      
            Needs.Erp.ErpPlot.Current.Limits.GetAdmin(pid).Plots.MyStaffs.Bind(id);

        }

        protected void remove()
        {
            var id = Request["id"];
            var pid = Request["pid"];

            Needs.Erp.ErpPlot.Current.Limits.GetAdmin(pid).Plots.MyStaffs.UnBind(id);

        }

    }
}