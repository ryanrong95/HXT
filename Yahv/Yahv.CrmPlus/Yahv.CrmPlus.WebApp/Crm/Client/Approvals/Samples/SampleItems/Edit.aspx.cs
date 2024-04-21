using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Rolls.Samples;
using Yahv.Underly;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Crm.Client.Approvals.Samples.SampleItems
{
    public partial class Edit : ErpParticlePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                loadData();

            }
        }


        public void loadData()
        {
            var id = Request.QueryString["ID"];
           var entity = new SampleItemRoll()[id];

            this.Model.Entity = new { entity.SpnID,  entity.Price, entity.SampleType,entity.Quantity, Total=(entity.Price *entity.Quantity)} ;

            // this.Model.Clients = Erp.Current.CrmPlus.MyClients.Where(x => x.Status == Underly.AuditStatus.Normal && x.IsDraft == false).Select(x => new { value = x.ID, text = x.Name });
            //this.Model.StandardPartNumber = new CrmPlus.Service.Views.Rolls.StandardPartNumbersRoll().Select(x => new { PartNumber = x.PartNumber, SpnID = x.ID, Brand = x.Brand });
            //this.Model.SampleTypes = ExtendsEnum.ToDictionary<SampleType>().Select(item => new
            //{
            //    value = int.Parse(item.Key),
            //    text = item.Value.ToString()
            //});
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {

            var projectProductID = Request.QueryString["ProjectProductID"];
            var projectID = Request.QueryString["ProjectID"];
            var partNumber = Request.Form["PartNumber"];
            var unitPrice = Request.Form["UnitPrice"];
            var qty = Request.Form["Qty"];
            var id = Request.QueryString["ID"];
            var sampleType = Request.Form["SampleType"];
            var product = new SampleItemRoll()[id];
            product.SpnID = partNumber;
            product.SampleType = (SampleType)int.Parse(sampleType);
            product.Quantity = int.Parse(qty);
            product.Price = int.Parse(unitPrice);
            product.AuditStatus = AuditStatus.Waiting;
            product.EnterSuccess += Entity_EnterSuccess;
            product.Enter();

        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as SampleItem;
            Service.LogsOperating.LogOperating(Erp.Current, entity.ID, $"编辑了样品明细ID:{entity.ID}");
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}