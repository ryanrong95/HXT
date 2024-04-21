using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Views;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Crm.Client.ProjectReports
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        protected object data()
        {
            string id = Request.QueryString["id"];
            //  var qq=  ProjectReportView.SearchByClientName()
            using (var view = new ProjectReportView())
            {
                var query = view;
                query = query.SearchByClientID(id);
                var result = query.ToMyArray().Select(item => new
                {
                    item.ID,
                    ClientName = item.ClientName,
                    ProjectName = item.ProjectName,
                    EstablishDate = item.EstablishDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    item.PartNumber,
                    item.Brand,
                    item.ReportStatus,
                    ReportStatusDes = item.ReportStatus.GetDescription(),
                    PM = string.Join(",", item.PMs.ToArray()),
                    FAE = string.Join(",", item.FAe.ToArray()),
                    CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
                });

                return result;
            }
            //var query = Erp.Current.CrmPlus.MyProjectReports.Where(x => x.ClientID == id && x.ReportStatus == ReportStatus.Success);
            //var result = this.Paging(query.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            //{
            //    item.ID,
            //    ClientName = item.Project.Client.Name,
            //    ProjectName = item.Project.Name,
            //    EstablishDate = item.Project.EstablishDate.ToString("yyyy-MM-dd HH:mm:ss"),
            //    item.StandardPartNumber?.PartNumber,
            //    item.StandardPartNumber?.Brand,
            //    item.ReportStatus,
            //    ReportStatusDes = item.ReportStatus.GetDescription(),
            //    PM = item.PM?.RealName,
            //    PMID = item.PM.ID,
            //    FAE = item.FAE?.RealName,
            //    //  ProjectStatus=  item.ProjectProduct.ProjectStatus,
            //    CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            //}));
        }


        //Expression<Func<ProjectReport, bool>> Predicate()
        //{
        //    Expression<Func<ProjectReport, bool>> predicate = item => true;
        //    var clientName = Request["ClientName"];
        //    var projectName = Request["ProjectName"];
        //    var standardPartNumber = Request["StandardPartNumber"];
        //    var status = Request["Status"];
        //    if (!string.IsNullOrWhiteSpace(clientName))
        //    {
        //        predicate = predicate.And(item => item.Project.Client.Enterprise.Name.Contains(clientName));
        //    }
        //    if (!string.IsNullOrWhiteSpace(projectName))
        //    {
        //        predicate = predicate.And(item => item.Project.Name.Contains(projectName));
        //    }

        //    if (!string.IsNullOrWhiteSpace(standardPartNumber))
        //    {
        //        predicate = predicate.And(item => item.StandardPartNumber.PartNumber.Contains(standardPartNumber));
        //    }

        //    ReportStatus dataStatus;
        //    if (Enum.TryParse(status, out dataStatus) && dataStatus != 0)
        //    {
        //        predicate = predicate.And(item => item.ReportStatus == dataStatus);
        //    }
        //    return predicate;
        //}

    }
}