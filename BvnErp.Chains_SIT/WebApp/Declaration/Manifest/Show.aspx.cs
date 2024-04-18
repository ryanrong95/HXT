using Layer.Data.Sqls.BvScsm;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.Manifest
{
    public partial class Show : Uc.PageBase
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
            SetDropDownList();
            LoadData();
        }
        protected void LoadData()
        {
            var ID = Request.QueryString["ID"];
            var manifestHeads = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Manifests[ID];
            //舱单编辑
            if (manifestHeads != null)
            {
                this.Model.Manifest = manifestHeads.Json();
            }
            else
            {
                this.Model.Manifest = "".Json();
            }

        }
        private void SetDropDownList()
        {
            this.Model.CustomsCodeData = Needs.Wl.Admin.Plat.AdminPlat.BaseCustomMaster.Select(item => new { Value = item.Code, Text = item.Code + "-" + item.Name }).OrderBy(item => item.Value).Json();
            this.Model.ConditionCodeData = getConditionCode().Select(item => new { Value = item.Key, Text = item.Value }).OrderBy(item => item.Value).Json();
            this.Model.PaymentTypeData = getPaymentType().Select(item => new { Value = item.Key, Text = item.Value }).OrderBy(item => item.Value).Json();
            this.Model.GovProcedureData = Needs.Wl.Admin.Plat.AdminPlat.BaseGovProcedure.Select(item => new { Value = item.Code, Text = item.Code + "-" + item.Name }).OrderBy(item => item.Value).Json();
            this.Model.PackTypeData = Needs.Wl.Admin.Plat.AdminPlat.BaseWrapTypesView.Select(item => new { Value = item.Code, Text = item.Code + "-" + item.Name }).OrderBy(item => item.Value).Json();
            this.Model.CurrencyData = Needs.Wl.Admin.Plat.AdminPlat.Currencies.Select(item => new { Value = item.Code, Text = item.Code + "-" + item.Name }).OrderBy(item => item.Value).Json();
        }
        private Dictionary<string, string> getConditionCode()
        {
            Dictionary<string, string> mark = new Dictionary<string, string>();
            mark.Add("10", "10: port to port港到港");
            mark.Add("27", "27: door to door门到门");
            mark.Add("28", "28: door to pier门到点");
            mark.Add("29", "29: pier to door点到门");
            mark.Add("30", "30: pier to pier点到点");
            return mark;
        }
        private Dictionary<string, string> getPaymentType()
        {
            Dictionary<string, string> mark = new Dictionary<string, string>();
            mark.Add("1", "1-Direct payment");
            return mark;
        }
    }
}