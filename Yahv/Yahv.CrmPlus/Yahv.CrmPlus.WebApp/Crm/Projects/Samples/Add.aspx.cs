using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.CrmPlus.Service.Views.Rolls.SalesChances;
using Yahv.CrmPlus.Service.Views.Rolls.Samples;
using Yahv.Underly;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Crm.Projects.Samples
{
    public partial class Add : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                var projectID = Request.QueryString["ProjectID"];
                var id = Request.QueryString["ID"];
                this.Model.project=  new ProjectRoll()[projectID];
                var entity = new ProjectProductRoll()[id];
                this.Model.Entity = entity;
                //this.Model.project = entity;

            }
            //  this.Model.project = Erp.Current.CrmPlus.MyProjects["projectid"];
            //this.Model.SampleTypes = ExtendsEnum.ToDictionary<SampleType>().Select(item => new
            //{
            //    value = int.Parse(item.Key),
            //    text = item.Value.ToString()
            //});
        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var deliveryDate = Request.Form["DeliveryDate"];
            var contact = Request.Form["Contact"];
            var address = Request.Form["Address"];
            var projectProductID = Request.QueryString["ID"];
            // var partNumber = Request.Form["PartNumber"];
            var unitPrice = Request.Form["UnitPrice"];
            var qty = Request.Form["Qty"];
            var sampleType = Request.Form["SampleType"];
            var product = new ProjectProductRoll()[projectProductID];
            var entity = new Sample();
            if (!string.IsNullOrEmpty(deliveryDate))
                entity.DeliveryDate = Convert.ToDateTime(deliveryDate);
            entity.ProjectID = product.ProjectID;
            entity.AddressID = address;
            entity.ContactID = contact;
            entity.ApplierID = Erp.Current.ID;
            entity.AuditStatus = AuditStatus.Waiting;
            entity.SampleItem = new SampleItem
            {
                SpnID = product.SpnID,
                SampleType = (SampleType)int.Parse(sampleType),
                Quantity = int.Parse(qty),
                Price = int.Parse(unitPrice),
                AuditStatus = AuditStatus.Waiting
            };

            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var sample = sender as Sample;
            (new ApplyTask
            {
                MainID = sample.ID,
                MainType = Underly.MainType.Clients,
                ApplierID = Erp.Current.ID,
                ApplyTaskType = Underly.ApplyTaskType.ClientSample,
            }).Enter();
            Service.LogsOperating.LogOperating(Erp.Current, sample.ID, $"新增送样申请:ID:{sample.ID}");
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}