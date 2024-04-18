using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Utils;

namespace WebApp.Client.Approval
{
    public partial class AssignServiceManager : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        protected void LoadData()
        {
            //参数
            string id = Request.QueryString["ID"];
            this.Model.ID = id;
            var ServiceIDs = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRoles.Where(manager => manager.Role.Name == "业务员").Select(item => item.Admin.ID).ToArray();
            this.Model.ServiceManager = Needs.Wl.Admin.Plat.AdminPlat.Admins.Where(item => ServiceIDs.Contains(item.ID)).Select(item => new { Key = item.ID, Value = item.RealName }).ToArray().Json();
            var admins = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientAdmins.Where(t => t.ClientID == id && t.Status == Needs.Ccs.Services.Enums.Status.Normal).ToList();
            this.Model.ClientAssignData = admins.Json();
        }

        /// <summary>
        /// 
        /// </summary>
        protected void SaveClientAssign()
        {
            try
            {
                string Model = Request.Form["Model"].Replace("&quot;", "\'");
                dynamic model = Model.JsonTo<dynamic>();

                var client = Needs.Wl.Admin.Plat.AdminPlat.Clients[(string)model.ID];
                var adminid = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                client.Admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create((string)adminid);
                client.ServiceManager = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create((string)model.ServiceManagerID);
                client.SetServicer(client.Admin, client.ServiceManager);
                Response.Write((new { success = true, message = "保存成功"}).Json());
            }
            catch (Exception e)
            {

                Response.Write((new { success = false, message = e.Message }).Json());
            }
           

        }

        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientAssign_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = e.Message }).Json());
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientAssign_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功", ID = e.Object }).Json());
        }
    }
}