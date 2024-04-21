using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views;
using Yahv.CrmPlus.Service.Views.Rolls.ProjectReports;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Crm.ProjectReports
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }




        protected object data()
        {
            var clientName = Request["ClientName"];
            var projectName = Request["ProjectName"];
            var standardPartNumber = Request["StandardPartNumber"];
            //var status = Request["Status"];

            using (var view = new ProjectReportView())
            {

                var query = view;
               query = query.SearchByPm(Erp.Current.ID);

                if (!string.IsNullOrWhiteSpace(clientName))
                {
                    query = query.SearchByClientName(clientName);
                }

                if (!string.IsNullOrWhiteSpace(projectName))
                {
                    query = query.SearchByProjectName(projectName);
                }
                if (!string.IsNullOrWhiteSpace(standardPartNumber))
                {
                    query = query.SearchBySpn(standardPartNumber);
                }
                ReportStatus status;
                if (Enum.TryParse(Request["Status"], out status))
                {
                    query.SearchByStatus(status);
                }

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
        //        predicate = predicate.And(item => item.Project.Client.Name.Contains(clientName));
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