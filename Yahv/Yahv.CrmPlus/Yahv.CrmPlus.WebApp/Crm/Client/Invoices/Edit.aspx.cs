using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Crm.Client.Invoices
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var id = Request.QueryString["enterpriseid"];
                this.Model.entity = Erp.Current.CrmPlus.MyClients[id];
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string address = Request.Form["Address"];
            string tel = Request.Form["Tel"];
            string bank = Request.Form["Bank"];
            string account = Request.Form["Account"].Trim();
            string ID = Request.QueryString["enterpriseid"];

            var entity = new Yahv.CrmPlus.Service.Models.Origins.Invoice();
            entity.EnterpriseID = ID;
            entity.RelationType = Underly.RelationType.Trade;
            entity.Address = address;
            entity.Tel = tel;
            entity.Bank = bank;
            entity.Account = account;
            entity.CreatorID = Erp.Current.ID;
            entity.EnterError += Entity_EnterError;
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();

        }



        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as Invoice;
            LogsOperating.LogOperating(Erp.Current, entity.ID, $"新增发票信息:{ entity.ID}");
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
        private void Entity_EnterError(object sender, Usually.ErrorEventArgs e)
        {
            Easyui.Dialog.Close("保存失败!", Web.Controls.Easyui.AutoSign.Error);
        }

    }
}