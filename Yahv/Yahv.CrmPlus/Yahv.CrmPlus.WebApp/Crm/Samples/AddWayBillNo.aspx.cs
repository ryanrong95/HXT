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

namespace Yahv.CrmPlus.WebApp.Crm.Samples
{
    public partial class AddWayBillNo : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var id = Request.QueryString["ID"];
                this.Model.Entity = Erp.Current.CrmPlus.MySamples[id];

            }
           
        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var id = Request.QueryString["ID"];
            var WaybillCode = Request.Form["WaybillCode"];
            var Summary = Request.Form["Summary"];
            var DeliveryDate = Request.Form["DeliveryDate"];
            var entity = Erp.Current.CrmPlus.MySamples[id];
            entity.WaybillCode = WaybillCode;
            entity.DeliveryDate =Convert.ToDateTime(DeliveryDate);
            entity.Summary = Summary;
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.UpdateWaybillNo();
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as Sample;
            LogsOperating.LogOperating(Erp.Current, entity.ID, $"维护了运单号:{ entity.WaybillCode}");
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}