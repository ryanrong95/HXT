using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.DeclarantCandidates
{
    public partial class ModifyCustomSubmiterWindow : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string Params = Request.QueryString["Params"];

            this.Model.Params = Params;

            string[] paramArray = Params.Split(',');

            string tipStr = "";
            if (paramArray.Length > 1)
            {
                tipStr = "已经选中 " + paramArray.Length + " 个报关单，请选择录入及申报员";
            }
            else if (paramArray.Length == 1)
            {
                tipStr = "订单号：" + paramArray[0].Split('|')[1] + "，请选择录入及申报员";
            }

            this.Model.TipStr = tipStr;

            this.Model.CandidateData = new Needs.Ccs.Services.Views.SelectableCandidatesView().GetUseCandidates(Needs.Ccs.Services.Enums.DeclarantCandidateType.CustomSubmiter)
                    .Select(item => new { value = item.AdminID, text = item.AdminName }).Json();
        }

        /// <summary>
        /// 保存
        /// </summary>
        protected void Save()
        {
            try
            {
                string Params = Request.Form["Params"];
                string AdminID = Request.Form["AdminID"];

                string[] paramArray = Params.Split(',');
                string[] decHeads = paramArray.Select(t => t.Split('|')[0]).ToArray();

                new Needs.Ccs.Services.Models.ModifyDeclarantHandler().ModifyCustomSubmiter(decHeads, AdminID);

                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }

    }
}