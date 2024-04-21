using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Linq.Extends;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Crm.Projects
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }



        protected object data()
        {

            string id = Request.QueryString["id"];
            var query = new ProjectRoll().Where(Predicate());
            var result = this.Paging(query.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                ClientName = item.Client.Name,
                item.Name,
                EstablishDate = item.EstablishDate.ToString("yyyy-MM-dd"),
                RDDate = item.RDDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                ProductDate = item.ProductDate?.ToString("yyyy-MM-dd"),
                Contact = item.Contact.Name,
                Mobile = item.Contact.Mobile,
                CreteDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            }));

            return result;

        }

        Expression<Func<Project, bool>> Predicate()
        {
            Expression<Func<Project, bool>> predicate = item => true;
            var clientName = Request["client"];
            var projectName = Request["projectName"];
            string startdate = Request.QueryString["startdate"];
            string enddate = Request.QueryString["enddate"];
            string orderclient = Request.QueryString["orderclient"];
            if (!string.IsNullOrWhiteSpace(clientName))
            {
                predicate = predicate.And(item => item.Client.Name.Contains(clientName));
            }
            if (!string.IsNullOrWhiteSpace(projectName))
            {
                predicate = predicate.And(item => item.Name.Contains(projectName));
            }
            if (!string.IsNullOrWhiteSpace(orderclient))
            {
                predicate = predicate.And(item => item.OrderClient.Name.Contains(orderclient));
            }
            DateTime start;
            if (!string.IsNullOrWhiteSpace(startdate) && DateTime.TryParse(startdate, out start))//开始日期
            {
                predicate = predicate.And(item => item.EstablishDate >= start);
            }


            DateTime end;
            if (!string.IsNullOrWhiteSpace(enddate) && DateTime.TryParse(enddate, out end))//结束日期
            {
                predicate = predicate.And(item => item.EstablishDate < end.AddDays(1));
            }
            return predicate;
        }
    }
}