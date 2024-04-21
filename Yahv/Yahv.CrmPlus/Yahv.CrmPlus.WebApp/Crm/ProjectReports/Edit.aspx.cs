using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.CrmPlus.Service.Views.Rolls.SalesChances;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Crm.ProjectReports
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
            var entity = new ProjectReportView()[id];
            var product = new ProjectProductRoll().FirstOrDefault(x => x.ProjectID == entity.ProjectID && x.SpnID == entity.SpnID);
            var project = new ProjectRoll()[entity.ProjectID];
            this.Model.Entity = new
            {
                ClientName = entity.ClientName,
                ProjectName = entity.ProjectName,
                EstablishDate = entity.EstablishDate.ToShortDateString(),
                RDDate = entity.RDDate?.ToShortDateString(),
                ProductDate = entity.ProductDate?.ToShortDateString(),
                Contact = entity.ClientContactID,
                OrderClient = project?.OrderClient?.Name,
                Summary = entity.Summary,
                PartNumber = entity.PartNumber,
                Brand = entity.Brand,
                PM = string.Join(",", entity.PMs),
                FAE = string.Join(",", entity.FAe),
                UnitProduceQuantity = product?.UnitProduceQuantity,
                ProduceQuantity = product?.ProduceQuantity,
                Currency = product.Currency.GetDescription(),
                ExpectUnitPrice = product.ExpectUnitPrice,
                ExpectAmount = product.ExpectQuantity * product.ExpectUnitPrice,
                ProjectStatus = product.ProjectStatus.GetDescription()

            };


        }


        protected void btnSubmit_Click(object sender,EventArgs e)
        {
            
            bool result =Convert.ToBoolean(this.Result.Value);
            var id = Request.QueryString["id"];
            var projectCode = Request.Form["projectCode"];
            var summary = Request.Form["summary"];
            // var entity = Erp.Current.CrmPlus.MyProjectReports[id];
            var entity = new ProjectReportView()[id];
            if (result)
            {
                entity.ReportStatus = Underly.ReportStatus.Success;
            }
            else
            {
                entity.ReportStatus = Underly.ReportStatus.Fail;
            }
            entity.ProjectCode = projectCode;
            entity.Summary = summary;
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Approve();
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as ProjectReportModel;
            LogsOperating.LogOperating(Erp.Current, entity.ID, $"报备了客户:{ entity.ClientID}");
            Easyui.Window.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }

    }
}