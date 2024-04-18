using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.DeclarantCandidates
{
    public partial class SetDefaultPort : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            this.Model.OrgCodes = Needs.Wl.Admin.Plat.AdminPlat.BaseOrgCodes.OrderBy(item => item.Code).Select(item => new { Value = item.Code, Text = item.Code + "-" + item.Name }).Json();
            this.Model.CustomMaster = Needs.Wl.Admin.Plat.AdminPlat.BaseCustomMaster.OrderBy(item => item.Code).Select(item => new { Value = item.Code, Text = item.Code + "-" + item.Name }).Json();
            this.Model.EntryPort = Needs.Wl.Admin.Plat.AdminPlat.BaseEntryPort.OrderBy(item => item.Code).Select(item => new { Value = item.Code, Text = item.Code + "-" + item.Name }).Json();
        }

        protected object getDropdownlist()
        {
            string value = Request.Form["value"];
            return Needs.Wl.Admin.Plat.AdminPlat.BaseOrgCodes.OrderBy(item => item.Code).Where(item => item.Code.Contains(value)).Take(20).Select(item => new { Value = item.Code, Text = item.Code + "-" + item.Name });
        }

        protected object getCustomMasterlist()
        {
            string value = Request.Form["value"];
            return Needs.Wl.Admin.Plat.AdminPlat.BaseCustomMaster.OrderBy(item => item.Code).Where(item => item.Code.Contains(value)).Take(20).Select(item => new { Value = item.Code, Text = item.Code + "-" + item.Name });
        }

        protected object getEntryPortlist()
        {
            string value = Request.Form["value"];
            return Needs.Wl.Admin.Plat.AdminPlat.BaseEntryPort.OrderBy(item => item.Code).Where(item => item.Code.Contains(value)).Take(20).Select(item => new { Value = item.Code, Text = item.Code + "-" + item.Name });
        }
        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            var customMasterView = new Needs.Ccs.Services.Views.BaseCustomMasterDefaultView();


            Func<Needs.Ccs.Services.Models.BaseCustomMasterDefault, object> convert = item => new
            {
                ID = item.ID,
                Code = item.Code,
                CodeName = item.CodeName,
                IEPortCode = item.IEPortCode,
                IEPortCodeName = item.IEPortCodeName,
                EntyPortCode = item.EntyPortCode,
                EntyPortCodeName = item.EntyPortCodeName,
                OrgCode = item.OrgCode,
                OrgCodeName = item.OrgCodeName,
                VsaOrgCode = item.VsaOrgCode,
                VsaOrgCodeName = item.VsaOrgCodeName,
                InspOrgCode = item.InspOrgCode,
                InspOrgCodeName = item.InspOrgCodeName,
                PurpOrgCode = item.PurpOrgCode,
                PurpOrgCodeName = item.PurpOrgCodeName,
                IsDefault = item.IsDefault
            };

            this.Paging(customMasterView, convert);
        }

        protected void Save()
        {

            string Model = Request.Form["Model"].Replace("&quot;", "\'");
            dynamic model = Model.JsonTo<dynamic>();

            var master = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.MasterDefault[(string)model.ID] ?? new Needs.Ccs.Services.Models.BaseCustomMasterDefault();
            master.Code = model.CustomMasterID;
            master.IEPortCode = model.IEPortCodeID;
            master.EntyPortCode = model.EntyPortCodeID;
            master.OrgCode = model.OrgCodeID;
            master.VsaOrgCode = model.VsaOrgCodeID;
            master.InspOrgCode = model.InspOrgCodeID;
            master.PurpOrgCode = model.PurpOrgCodeID;
            master.IsDefault = model.IsDefault;

            master.EnterSuccess += Master_EnterSuccess;
            master.EnterError += Master_EnterError;
            master.Enter();
        }
        private void Master_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write(new { success = false, message = e.Message });
        }

        private void Master_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功", ID = e.Object }).Json());
        }
    }
}