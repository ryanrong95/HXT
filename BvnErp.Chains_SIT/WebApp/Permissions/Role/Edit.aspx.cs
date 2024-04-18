using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Permissions.Role
{
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 初始化基本信息
        /// </summary>
        protected void LoadData()
        {
            string id = Request.QueryString["ID"];

            if (!string.IsNullOrWhiteSpace(id))
            {
                var role = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.Roles[id];

                this.Model.RoleData = new
                {
                    role.ID,
                    role.Name,
                    role.SysCode,
                    role.Summary
                }.Json();
            }
            else
            {
                this.Model.RoleData = new
                {

                }.Json();
            }
        }

        protected void Save()
        {
            var Name = Request.Form["Name"];
            var SysCode = Request.Form["SysCode"];
            var Summary = Request.Form["Summary"];
            var id = Request.Form["ID"];
            var role = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.Roles[id] as
             Needs.Ccs.Services.Models.Role ?? new Needs.Ccs.Services.Models.Role();
            role.Name = Name;
            role.SysCode = SysCode;
            role.Summary = Summary;
            role.EnterSuccess += Role_EnterSuccess;
            role.EnterError += Role_EnterError;
            role.Enter();
        }

        private void Role_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功" }).Json());
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Role_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = "保存失败" }).Json());
        }
    }
}