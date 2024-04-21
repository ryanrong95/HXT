using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views;
using Yahv.CrmPlus.Service.Views.Origins;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Underly.Erps;
using Yahv.Usually;
using YaHv.CrmPlus.Services.Models.Origins;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.Service.Views
{
    public class ProjectReportModel : IEntity
    {
        #region  属性
        public string ID { get; set; }
        /// <summary>
        /// 终端客户
        /// </summary>

        public string ClientID { get; set; }

        public string ClientName { get; set; }

        #region project
        public string ProjectID { get; set; }

        public string ProjectName { get; set; }

        /// <summary>
        /// （实际终端）客户
        /// </summary>

        public string EndClientID { get; set; }
        public string AssignClientID { get; set; }
        /// <summary>
        /// 量产日期
        /// </summary>
        public DateTime? ProductDate { get; set; }
        /// <summary>
        /// 预计研发日期
        /// </summary>
        public DateTime? RDDate { get; set; }
        /// <summary>
        /// (客户)联系人ID
        /// </summary>
        public string ClientContactID { get; set; }
        public string Summary { set; get; }
        #endregion

        /// <summary>
        /// 标准型号ID
        /// </summary>
        public string SpnID { get; set; }
        /// <summary>
        /// 型号名称
        /// </summary>
        public string PartNumber { get; set; }


        public string Brand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string[] PMs { get; set; }

        public string[] FAe { get; set; }
        public DateTime EstablishDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ReportStatus ReportStatus { get; set; }


        /// <summary>
        /// 原厂编号
        /// </summary>
        public string ProjectCode { get; set; }

        public void Approve()
        {

            {
                using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
                {
                    repository.Update<Layers.Data.Sqls.PvdCrm.ProjectReports>(new
                    {
                        Status = this.ReportStatus,
                        ProjectCode=this.ProjectCode,
                        Summary = this.Summary
                    }, item => item.ID == this.ID);
                    if (this != null && this.EnterSuccess != null)
                    {
                        this.EnterSuccess(this, new SuccessEventArgs(this));
                    }
                }
            }
        }

        #endregion

        #region 事件
        public event SuccessHanlder EnterSuccess;



        public event SuccessHanlder AbandonSuccess;
        /// <summary>
        /// EnterError
        /// </summary>

        public event ErrorHanlder EnterError;
        #endregion
    }


    public class ProjectReportView : vDepthView<ProjectReport, ProjectReportModel, PvdCrmReponsitory>
    {
        public ProjectReportView()
        {

        }
        /// <summary>
        /// 筛选数据
        /// </summary>
        /// <param name="reponsitory"></param>
        /// <param name="iQueryable"></param>
        internal ProjectReportView(PvdCrmReponsitory reponsitory, IQueryable<ProjectReport> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        public ProjectReportView(IQueryable<ProjectReport> iQueryable) : base(iQueryable)
        {
        }
        protected ProjectReportView(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.Origins.ProjectReport> GetIQueryable()
        {
            //if (!string.IsNullOrEmpty(id))
            //{
            //    return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.ProjectReports>()
            //           where entity.ID.Contains(id)
            //           select new Models.Origins.ProjectReport
            //           {
            //               ID = entity.ID,
            //               ClientID = entity.ClientID,
            //               ProjectID = entity.ProjectID,
            //               SpnID = entity.SpnID,
            //               ReporterID = entity.ReporterID,
            //               CreateDate = entity.CreateDate,
            //               ModifyDate = entity.ModifyDate,
            //               ProjectCode = entity.ProjectCode,
            //               ReportStatus = (ReportStatus)entity.Status,
            //               Summary = entity.Summary
            //           };

            //}

            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.ProjectReports>()
                   select new Models.Origins.ProjectReport
                   {
                       ID = entity.ID,
                       ClientID = entity.ClientID,
                       ProjectID = entity.ProjectID,
                       SpnID = entity.SpnID,
                       ReporterID = entity.ReporterID,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       ProjectCode = entity.ProjectCode,
                       ReportStatus = (ReportStatus)entity.Status,
                       Summary = entity.Summary
                   };
        }


        protected override IEnumerable<ProjectReportModel> OnMyPage(IQueryable<ProjectReport> iquery)
        {
            var data = iquery.ToArray();
            var spnsId = data.Select(item => item.SpnID).Distinct().ToArray();
            var linq_standardPartNumber =
               from spn in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.StandardPartNumbers>()
               join brand in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.StandardBrands>() on spn.BrandID equals brand.ID
               where spnsId.Contains(spn.ID)
               select new
               {
                   ID = spn.ID,
                   PartNumber = spn.PartNumber,
                   spn.BrandID,
                   BrandName = brand.Name

               };
            var standardPartNumbers = linq_standardPartNumber.ToArray();

            var brandsId = linq_standardPartNumber.Select(item => item.BrandID).Distinct().ToArray();
        
            var linq_vbrand = from vbrand in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.vBrands>()
                              join admin in new AdminsAllRoll(this.Reponsitory) on vbrand.AdminID equals admin.ID
                              where brandsId.Contains(vbrand.BrandID)
                              select new
                              {
                                  vbrand.ID,
                                  vbrand.AdminID,
                                  vbrand.BrandID,
                                  admin.RoleID,
                                  admin.RealName,
                              };

            var linq_admins = linq_vbrand.ToArray();

            var projectsId = data.Select(item => item.ProjectID).Distinct();
            var linq_project = from project in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Projects>()
                               join  enterprise in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Enterprises>() on project.EndClientID  equals enterprise.ID
                               where projectsId.Contains(project.ID) 
                               select new
                               {
                                   ID = project.ID,
                                   project.Name,
                                   project.EstablishDate,
                                   project.RDDate,
                                   project.ProductDate,
                                   project.ClientContactID,
                                   project.AssignClientID,
                                   project.EndClientID,
                                   project.Summary,
                                   EnterpriseName=enterprise.Name
                               };
            var projects = linq_project.ToArray();
            //var clientsId = data.Select(item => item.ClientID).Distinct();

            //var linq_clients = from client in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Enterprises>()
            //                   where clientsId.Contains(client.ID)
            //                   select new
            //                   {
            //                       client.ID,
            //                       client.Name
            //                   };
            //var clients = linq_clients.ToArray();


            return from entity in data
                   join project in projects on entity.ProjectID equals project.ID
                 //  join client in clients on project.EndClientID equals client.ID
                   join spn in standardPartNumbers on entity.SpnID equals spn.ID
                   select new ProjectReportModel
                   {

                       ID = entity.ID,
                       ClientID = entity.ClientID,
                       ProjectID = entity.ProjectID,
                       ProjectName = project.Name,
                       EstablishDate = project.EstablishDate,
                       ClientName = project.EnterpriseName,
                       SpnID = entity.SpnID,
                       PartNumber = spn.PartNumber,
                       Brand = spn.BrandName,
                       PMs = linq_admins.Where(item => item.BrandID == spn.BrandID && (item.RoleID == FixedRole.PM.GetFixedID() || item.RoleID == FixedRole.PMa.GetFixedID())).Select(item => item.RealName).ToArray(),
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       ProjectCode = entity.ProjectCode,
                       ReportStatus = entity.ReportStatus,
                       FAe = linq_admins.Where(item => item.BrandID == spn.BrandID && item.RoleID == FixedRole.FAE.GetFixedID()).Select(item => item.RealName).ToArray(),
                       Summary = entity.Summary
                   };

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public ProjectReportView Search(Expression<Func<ProjectReport, bool>> predicate)
        {
            var linq = this.IQueryable.Cast<ProjectReport>().Where(predicate);
            return new ProjectReportView(Reponsitory, linq);
        }

        public ProjectReportView SearchByClientName(string clientname)
        {
            var clientsView = from client in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Enterprises>()
                              where client.Name.StartsWith(clientname)
                              select client;
            var clientId = clientsView.Select(item => item.ID).ToArray();

            var linq = this.IQueryable.Cast<ProjectReport>().Where(item => clientId.Contains(item.ClientID));

            return new ProjectReportView(this.Reponsitory, linq);
        }


        public ProjectReportView SearchByProjectName(string projectname)
        {
            var projectView = from project in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Projects>()
                              where project.Name.StartsWith(projectname)
                              select project;

            var projectId = projectView.Select(item => item.ID).ToArray();
            var linq = this.GetIQueryable().Where(item => projectId.Contains(item.ProjectID));
            return new ProjectReportView(this.Reponsitory, linq);

        }
        public ProjectReportView SearchBySpn(string spn)
        {
            var spnViewView = from Spn in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.StandardPartNumbers>()
                              where Spn.PartNumber.StartsWith(spn)
                              select Spn;

            var SpnsId = spnViewView.Select(item => item.ID).ToArray();
            var linq = this.GetIQueryable().Where(item => SpnsId.Contains(item.SpnID));
            return new ProjectReportView(this.Reponsitory, linq);

        }


        public ProjectReportView SearchByStatus(ReportStatus reportStatus)
        {
            var linq = this.GetIQueryable().Where(item => item.ReportStatus == reportStatus);
            return new ProjectReportView(this.Reponsitory, linq);

        }

        public ProjectReportView SearchByClientID(string  clientid)
        {
            var linq = this.GetIQueryable().Where(item => item.ClientID ==clientid);
            return new ProjectReportView(this.Reponsitory, linq);

        }

        //public ProjectReportView SearchByID(string clientid)
        //{
        //    var linq = this.GetIQueryable().Where(item => item.ClientID == clientid);
        //    return new ProjectReportView(this.Reponsitory, linq);

        //}
        /// <summary>
        /// 想要查询我的报备
        /// </summary>
        /// <param name="pm"></param>
        /// <returns></returns>
        public ProjectReportView SearchByPm(string pm)
        {
            var linq_vbrand = from vbrand in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.vBrands>()
                              join brand in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.StandardBrands>() on vbrand.BrandID equals brand.ID
                              where vbrand.AdminID == pm
                              select new
                              {
                                  vbrand.ID,
                                  vbrand.BrandID,
                                  brand.Name,
                              };

            var linq_report = from report in this.IQueryable
                              join spn in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.StandardPartNumbers>() on report.SpnID equals spn.ID
                              join brand in linq_vbrand on spn.BrandID equals brand.BrandID
                              select report;

            //vbrand.BrandID , BrandID , vbrand.AdminID
            //vbrand.BrandID , AdminID , vbrand.BrandID

            return new ProjectReportView(this.Reponsitory, linq_report);
        }
    }


    public class MyProjectReportRoll : ProjectReportView
    {
        /*
通过Product获取品牌名称
         
*/


        IErpAdmin admin;

        public MyProjectReportRoll(IErpAdmin admin)
        {
            this.admin = admin;

            if (admin.Role.Type == RoleType.Compose)
            {
                admin.Contains(FixedRole.PM);
                admin.Contains(FixedRole.PMa);
            }


            //admin.Fixed == FixedRole.PM 
        }

    }

}
