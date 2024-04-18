using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Catalogues
{
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                LoadData();
            }
        }

        protected void LoadData()
        {
            string id = Request.QueryString["ID"];
            var catelogue = Needs.Erp.ErpPlot.Current.ClientSolutions.Catelogues[id];
            this.Model = catelogue.Json();
        }

        /// <summary>
        /// 保存
        /// </summary>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string id = Request.QueryString["ID"];
            var catelogue = Needs.Erp.ErpPlot.Current.ClientSolutions.Catelogues[id] as NtErp.Crm.Services.Models.Catelogue
                ?? new NtErp.Crm.Services.Models.Catelogue();
            catelogue.Summary = Request.Form["Summary"];
            catelogue.EnterSuccess += Catelogue_EnterSuccess;
            catelogue.Enter();
        }

        /// <summary>
        /// 保存成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Catelogue_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            var url = Request.UrlReferrer ?? Request.Url;
            Alert("保存成功!", url, true);
        }

    }
}