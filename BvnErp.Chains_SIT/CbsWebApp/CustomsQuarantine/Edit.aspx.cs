using Needs.Cbs.Services.Enums;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Needs.Cbs.WebApp.CustomsQuarantine
{
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
            DropDownList();
        }

        private void DropDownList()
        {
            this.Model.OriginData = Needs.Wl.Admin.Plat.AdminPlat.Current.Cbs.CustomsSettings[BaseType.Country].Select(item => new { value = item.Code, text = item.Code + "-" + item.Name }).Json();
        }

        protected void LoadData()
        {
            string id = Request.QueryString["ID"];

            if (!string.IsNullOrWhiteSpace(id))
            {
                var quarantines = Needs.Wl.Admin.Plat.AdminPlat.Current.Cbs.CustomsQuarantines[id];

                this.Model.RoleData = new
                {
                    quarantines.ID,
                    quarantines.Origin,
                    quarantines.StartDate,
                    quarantines.EndDate,
                    quarantines.Summary
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
            var Origin = Request.Form["Origin"];
            var StartDate = Request.Form["StartDate"];
            var EndDate = Request.Form["EndDate"];
            var Summary = Request.Form["Summary"];
            var id = Request.Form["ID"];
            var quarantines = Needs.Wl.Admin.Plat.AdminPlat.Current.Cbs.CustomsQuarantines[id] as Services.Models.Origins.CustomsQuarantine ?? 
                              new Services.Models.Origins.CustomsQuarantine();
            quarantines.Origin = Origin;
            quarantines.StartDate = Convert.ToDateTime(StartDate);
            quarantines.EndDate = Convert.ToDateTime(EndDate);
            quarantines.Summary = Summary;
            quarantines.EnterSuccess += Quarantine_EnterSuccess;
            quarantines.EnterError += Quarantine_EnterError;
            quarantines.Enter();
        }

        private void Quarantine_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功" }).Json());
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Quarantine_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = "保存失败" }).Json());
        }
    }
}