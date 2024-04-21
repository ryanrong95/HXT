using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Underly;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.Service.Views.Origins
{
    public class ProjectReportOrigin : Yahv.Linq.UniqueView<ProjectReport, PvdCrmReponsitory>
    {

        internal ProjectReportOrigin()
        {
        }

        protected override IQueryable<ProjectReport> GetIQueryable()
        {
            var pmAdminTopview = new AdminsAllRoll(this.Reponsitory).Where(x => x.RoleID == FixedRole.PM.GetFixedID() || x.RoleID==FixedRole.PMa.GetFixedID());
            var FAETopView = new AdminsAllRoll(this.Reponsitory).Where(x => x.RoleID == FixedRole.FAE.GetFixedID());
            var projectView = new ProjectOrigin(this.Reponsitory);
            var standardPartNumberView = new StandardPartNumbersOrigin(this.Reponsitory);
            var vbrandView = new vBrandOrigin(this.Reponsitory);
           // var nbrandView = new AgentBrandsRoll(this.Reponsitory);
            var brandView = new BrandsOrigin(this.Reponsitory);
            var projectProductView = new ProjectProductOrigin(this.Reponsitory);
            var clientView = new ClientsOrigin(this.Reponsitory).Where(x => x.IsDraft == false && x.Status == Underly.AuditStatus.Normal);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.ProjectReports>()
                   join project in  projectView on entity.ProjectID  equals project.ID
                   join standardPartNumber in standardPartNumberView on entity.SpnID equals standardPartNumber.ID
                   join brand in brandView on standardPartNumber.Brand equals brand.Name
                   join vbrand in vbrandView on brand.ID equals vbrand.BrandID
                   join PmAdmin in pmAdminTopview on vbrand.AdminID equals PmAdmin.ID into pms
                   from PmAdmin in pms.DefaultIfEmpty()
                   join FAEAdmin in FAETopView on vbrand.AdminID equals FAEAdmin.ID into faes
                   from FAEAdmin in faes.DefaultIfEmpty()
                   select new ProjectReport
                   {
                       ID = entity.ID,
                       ClientID=entity.ClientID,
                       ProjectID=entity.ProjectID,
                       SpnID=entity.SpnID,
                       StandardPartNumber=standardPartNumber,
                       ReporterID=entity.ReporterID,
                       CreateDate=entity.CreateDate,
                       ModifyDate=entity.ModifyDate,
                       ProjectCode=entity.ProjectCode,
                       ReportStatus=(ReportStatus)entity.Status,
                       Project = project,
                      // ProjectProduct=projectProduct,
                       PM=PmAdmin,
                       FAE=FAEAdmin,
                       Summary=entity.Summary
                   };
        }

    }
}
