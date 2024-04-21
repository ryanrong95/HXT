using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Views;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.CrmPlus.Service.Views.Rolls.SalesChances;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Crm.ProjectReports
{
    public partial class Detail : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        public void LoadData()
        {
            var id = Request.QueryString["ID"];
            var entity = new ProjectReportView().ToMyArray().SingleOrDefault(x => x.ID == id);
            var product = new ProjectProductRoll().FirstOrDefault(x => x.ProjectID == entity.ProjectID && x.SpnID == entity.SpnID);
            var project = new ProjectRoll()[entity.ProjectID];
            this.Model.Entity = new
            {
                entity.ID,
                entity.ProjectCode,
                ReportStatus= entity.ReportStatus.GetDescription(),
                Reason= entity.Summary,
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
                UnitProduceQuantity = product.UnitProduceQuantity,
                ProduceQuantity = product.ProduceQuantity,
                Currency = product.Currency.GetDescription(),
                ExpectUnitPrice = product.ExpectUnitPrice,
                ExpectAmount = product.ExpectQuantity * product.ExpectUnitPrice,
                ProjectStatus = product.ProjectStatus.GetDescription()

            };


        }
    }
}