using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Permissions.Admin
{
    public partial class SetRole : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetDropDownList();
            }
        }

        private void SetDropDownList()
        {
            this.Model.RoleData = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.Roles.Select(item => new { Value = item.ID, Text = item.Name }).Json();
            string id = Request.QueryString["ID"];
            var adminrole = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRoles.Where(item => item.Admin.ID == id).FirstOrDefault();
            this.Model.AdminData = new
            {
                ID=id,
                RoleID=adminrole?.Role.ID,
            }.Json();
        }
        protected void Save()
        {
            var id = Request.Form["ID"];
            var roleid = Request.Form["Set"];           
            var adminrole = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRoles.Where(item=>item.Admin.ID ==id).FirstOrDefault()
                as Needs.Ccs.Services.Models.AdminRoles??
                new Needs.Ccs.Services.Models.AdminRoles();
            adminrole.Admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(id);
            adminrole.Role = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.Roles[roleid];
            adminrole.EnterError += AdminRole_EnterError;
            adminrole.EnterSuccess += AdminRole_EnterSuccess;
            adminrole.Enter();
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
        private void AdminRole_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功" }).Json());
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdminRole_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = "保存失败" }).Json());
        }
    }
}