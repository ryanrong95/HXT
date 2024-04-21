using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.CrmPlus.Service.Views.Rolls.SalesChances;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Underly.CrmPlus;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Crm.Client.Approvals.SalesChances
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            this.Model.Clients = Erp.Current.CrmPlus.MyClients.Where(x => x.Status == Underly.AuditStatus.Normal && x.IsDraft == false).Select(x => new { value = x.ID, text = x.Name });
        }



        protected object data()
        {
            string id = Request.QueryString["id"];
            var query = new ProjectProductExtendRoll().Where(Predicate());
            var result = this.Paging(query.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                ClientName = item.Project.Client.Name,
                item.Project.Name,
                item.StandardPartNumber.PartNumber,
                item.StandardPartNumber.Brand,
                item.ProjectStatus,
                ProjectStatusDes=item.ProjectStatus.GetDescription(),
                EstablishDate = item.Project.EstablishDate.ToString("yyyy-MM-dd HH:mm:ss"),
                RDDate = item.Project.RDDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                ProductDate = item.Project.ProductDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                Contact = item.Project.Contact.Name,
                Mobile = item.Project.Contact.Mobile,
                CreteDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            }));

            return result;

        }

        Expression<Func<ProjectProduct, bool>> Predicate()
        {
            Expression<Func<ProjectProduct, bool>> predicate = item => true;
            var clientName = Request["clientName"];
            var projectName = Request["projectName"];
            string partNumber = Request.QueryString["partNumber"];
            if (!string.IsNullOrWhiteSpace(clientName))
            {
                predicate = predicate.And(item => item.Project.Client.Name.Contains(clientName));
            }
            if (!string.IsNullOrWhiteSpace(projectName))
            {
                predicate = predicate.And(item => item.Project.Name.Contains(projectName));
            }
            if (!string.IsNullOrWhiteSpace(partNumber))
            {
                predicate = predicate.And(item => item.StandardPartNumber.PartNumber.Contains(partNumber));
            }

            return predicate;
        }

    }
}