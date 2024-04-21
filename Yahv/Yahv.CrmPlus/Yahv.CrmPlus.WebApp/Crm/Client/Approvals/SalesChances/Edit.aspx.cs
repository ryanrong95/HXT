using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Rolls.SalesChances;
using Yahv.Underly;
using Yahv.Underly.CrmPlus;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Crm.Client.Approvals.SalesChances
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        public void LoadData()
        {
            var id = Request.QueryString["ID"];
            var entity = new ProjectProductExtendRoll()[id];
            //  var product = new ProjectProductRoll().FirstOrDefault(x => x.ProjectID == entity.Project.ID && x.SpnID == entity.StandardPartNumber.ID);
            this.Model.Entity = new
            {
                entity.ID,
                ClientName = entity.Project.Client.Name,
                ProjectName = entity.Project.Name,
                EstablishDate = entity.Project.EstablishDate.ToShortDateString(),
                RDDate = entity.Project.RDDate?.ToShortDateString(),
                ProductDate = entity.Project.ProductDate?.ToShortDateString(),
                Contact = entity.Project.Contact.Name,
                OrderClient = entity.Project.OrderClient?.Name??"",
                Summary = entity.Project.Summary,
                PartNumber = entity.StandardPartNumber.PartNumber,
                Brand = entity.StandardPartNumber.Brand,
                //PM = entity.PM.RealName,
                //FAE = entity.Project.FAE?.RealName,
                UnitProduceQuantity = entity.UnitProduceQuantity,
                ProduceQuantity = entity.ProduceQuantity,
                Currency = entity.Currency.GetDescription(),
                ExpectUnitPrice = entity.ExpectUnitPrice,
                ExpectAmount = (entity.ExpectQuantity) * (entity.ExpectUnitPrice)??0M,
                ProjectStatus = entity.ProjectStatus.GetDescription()

            };


        }


        protected void Approval()
        {
            var id = Request.Form["id"];
            var result = Request.Form["result"];
            var projectCode = Request.Form["projectCode"];
            var summary = Request.Form["summary"];
            var entity = new ProjectProductExtendRoll()[id];
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Status = AuditStatus.Normal;
            entity.Approve();
            var applytask = new ApplyTask();
            if (result == "true")
            {
                applytask.Status = ApplyStatus.Allowed;
            }
            else
            {
                applytask.Status = ApplyStatus.Voted;
            }
            applytask.MainID = entity.ID;
            applytask.MainType = Underly.MainType.Clients;
            applytask.ApproverID = Erp.Current.ID;
            applytask.ApplyTaskType = Underly.ApplyTaskType.ClientProjectStatus;
            applytask.Approve();

        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as ProjectProduct;

            LogsOperating.LogOperating(Erp.Current, entity.ID, $"审批了销售状态变更:{ entity.ID}");
            Response.Write((new { success = true, message = "操作成功" }).Json());
        }
    }
}