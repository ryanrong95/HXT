using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Permissions
{
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                load();
            }
        }

        private void load()
        {
            string id = Request.QueryString["ID"];
            Needs.Wl.Admin.Plat.Models.Admin admin = Needs.Wl.Admin.Plat.AdminPlat.Admins[id];

            if (admin != null)
            {
                this.Model.AdminData = new
                {
                    ID = admin.ID,
                    UserName = admin.UserName,
                    RealName = admin.RealName,
                    Tel = admin.Tel,
                    Mobile = admin.Mobile,
                    Email = admin.Email,
                    Summary = admin.Summary
                }.Json();
            }
            else
            {
                this.Model.AdminData = admin.Json();
            }
        }

        protected void Save()
        {
            var id = Request.Form["ID"];
            var admin = Needs.Wl.Admin.Plat.AdminPlat.Admins[id] as Needs.Wl.Admin.Plat.Models.Admin;
            admin.Tel = Request.Form["Tel"];
            admin.Email = Request.Form["Email"];
            admin.Mobile = Request.Form["Mobile"];
            admin.Summary = Request.Form["Summary"];
            admin.EnterError += Admin_EnterError;
            admin.EnterSuccess += Admin_EnterSuccess;
            admin.Enter();
        }

        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Admin_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功" }).Json());
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Admin_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = "保存失败" }).Json());
        }
    }
}