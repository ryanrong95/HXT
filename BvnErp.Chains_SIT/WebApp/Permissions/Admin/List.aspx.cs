using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Permissions
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        protected void data()
        {
            var view = Needs.Wl.Admin.Plat.AdminPlat.Admins.AsQueryable();

            string name = Request.QueryString["Name"];

            if (!string.IsNullOrEmpty(name))
            {
                view = view.Where(item => item.RealName.Contains(name));
            }
            Func<Needs.Wl.Admin.Plat.Models.Admin, object> convert = item => new
            {
                ID = item.ID,
                UserName = item.UserName,
                RealName = item.RealName,
                Email = item.Email,
                Tel = item.Tel,
                Mobile = item.Mobile,
                DepartmentName = item.DepartmentName,
                Summary = item.Summary
            };
            this.Paging(view, convert);
        }

        /// <summary>
        /// 数据删除
        /// </summary>
        protected void Delete()
        {
            string id = Request.Form["ID"];
            var del = Needs.Wl.Admin.Plat.AdminPlat.Admins[id];
            if (del != null)
            {
                //  del.Abandon();
            }
        }

        /// <summary>
        /// 删除成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Del_AbandonSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Alert("删除成功!");
        }
    }
}