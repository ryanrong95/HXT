using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Ccs.ServiceApplies.Apply
{
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            string id = Request.QueryString["ID"];
            this.Model.ID = id;

            if (!string.IsNullOrEmpty(id))
            {
                var apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyServiceApplies[id];
                this.Model.ApplyData = apply.Json();
            }
            else
            {
                this.Model.ApplyData = null;
            }
        }

        protected void SaveApplyHandle()
        {
            string ID = Request.Form["ID"];
            var apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyServiceApplies[ID];
            apply.Admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
            apply.Status = Needs.Ccs.Services.Enums.HandleStatus.Processed;
            apply.EnterSuccess += Apply_EnterSuccess;
            apply.EnterError += Apply_EnterError;
            apply.Enter();
        }

        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Apply_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write(new { success = false, message = e.Message });
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Apply_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功", ID = e.Object }).Json());
        }
    }
}