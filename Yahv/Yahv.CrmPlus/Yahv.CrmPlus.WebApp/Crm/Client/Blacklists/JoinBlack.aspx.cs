using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Crm.Client.BlackLists
{
    public partial class JoinBlack : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string summary = Request.Form["Summary"];
            string enterpriseid = Request.QueryString["id"];
            var enterprise = new Service.Views.Rolls.EnterprisesRoll()[enterpriseid];
            enterprise.Summary = summary;
            enterprise.EnterSuccess += Enterprise_EnterSuccess;
            enterprise.JoinBlack();
        }

        private void Enterprise_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as Enterprise;
            Service.LogsOperating.LogOperating(Erp.Current, entity.ID, $"加入企业黑名单:{entity.Name}");
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}