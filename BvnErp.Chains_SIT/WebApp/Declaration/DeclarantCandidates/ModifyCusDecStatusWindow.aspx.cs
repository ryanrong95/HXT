using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.DeclarantCandidates
{
    public partial class ModifyCusDecStatusWindow : Uc.PageBase
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
            this.Model.DecheadStatus = Needs.Ccs.Services.MultiEnumUtils.ToDictionary<Needs.Ccs.Services.Enums.CusDecStatus>()
                .Select(item => new { Value = item.Key, Text = item.Value }).Json();

            this.Model.CusDecStatus = new Needs.Ccs.Services.Views.DecHeadsView().Where(t => t.ID == id).FirstOrDefault().CusDecStatus;
        }

        /// <summary>
        /// 
        /// </summary>
        protected void SaveDecheadStatus()
        {
            string Model = Request.Form["Model"].Replace("&quot;", "\'");
            dynamic model = Model.JsonTo<dynamic>();

            Needs.Ccs.Services.Models.DecHead headinfo = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareHead[(string)model.ID];
            headinfo.CusDecStatus = (string)model.DecheadStatusID;
            headinfo.SaveDecheadStatus();

            Response.Write((new { success = true, message = "保存成功"}).Json());
        }
    }
}