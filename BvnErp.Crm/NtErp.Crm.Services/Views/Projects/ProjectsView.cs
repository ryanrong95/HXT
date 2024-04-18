using Layer.Data.Sqls;
using Needs.Erp.Generic;
using Needs.Linq;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views.Projects
{
    /// <summary>
    /// 销售机会管理数据集
    /// </summary>
    public class ProjectsView : UniqueView<Project, BvCrmReponsitory>, Needs.Underly.IFkoView<Project>
    {
        IGenericAdmin admin;
        Models.AdminTop AdminTop;



        /// <summary>
        /// 构造函数
        /// </summary>
        public ProjectsView()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="adminID">相关adminID</param>
        public ProjectsView(IGenericAdmin admin)
        {
            this.admin = admin;
            this.AdminTop = Extends.AdminExtends.GetTop(admin.ID);
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="adminID">相关adminID</param>
        public ProjectsView(string adminID)
        {
            this.AdminTop = Extends.AdminExtends.GetTop(adminID);
        }

        internal ProjectsView(BvCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        /// <summary>
        /// 基础数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Project> GetIQueryable()
        {
            //if (this.AdminTop != null && this.AdminTop.JobType != JobType.TPM)
            //{
            //    return GetOwn();
            //}
            return GetAll();
        }

        /// <summary>
        /// 获取相关人员包含自己的销售机会（PMAdmin，FAEAdmin，SaleAdmin，PurChaseAdmin，AssistantAdmin）
        /// </summary>
        /// <returns></returns>
        /// <param name="adminID"></param>
        //public IQueryable<Project> GetWith(string adminID)
        //{
        //    var projectIDs = from map in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProject>()
        //                     join product in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.ProductItems>() on map.ProductItemID equals product.ID
        //                     where product.PMAdmin == adminID || product.FAEAdmin == adminID || product.SaleAdmin == adminID || product.PurChaseAdmin == adminID || product.AssistantAdmin == adminID
        //                     select map.ProjectID;

        //    return this.GetIQueryable().Where(item => projectIDs.Distinct().Contains(item.ID));
        //}


        protected IQueryable<Project> GetAll()
        {
            var adminsTopView = new Views.AdminTopView(this.Reponsitory);
            var industriesView = new IndustryAlls(this.Reponsitory);
            var companiesView = new Views.CompanyAlls(this.Reponsitory);
            var clientsView = new Views.ClientAlls(this.Reponsitory);

            var linqs = from project in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Projects>()
                        join map in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProject>() on project.ID equals map.ProjectID
                        join industry in industriesView on map.IndustryID equals industry.ID
                        join admin in adminsTopView on project.AdminID equals admin.ID
                        join company in companiesView on project.CompanyID equals company.ID
                        join client in clientsView on project.ClientID equals client.ID
                        select new Project
                        {
                            ID = project.ID,
                            Name = project.Name,
                            ProductName = project.ProductName,
                            Type = (ProjectType)project.Type,
                            ClientID = project.ClientID,
                            CompanyID = project.CompanyID,
                            Valuation = project.Valuation,
                            Currency = (CurrencyType)project.Currency,
                            AdminID = project.AdminID,
                            StartDate = project.StartDate,
                            EndDate = project.EndDate,
                            ModelDate = project.ModelDate,
                            ProductDate = project.ProductDate,
                            MonthYield = project.MonthYield,
                            Contactor = project.Contactor,
                            Phone = project.Phone,
                            Address = project.Address,
                            CreateDate = project.CreateDate,
                            UpdateDate = project.UpdateDate,
                            Status = (ActionStatus)project.Status,
                            Summary = project.Summary,
                            ExpectTotal = project.ExpectTotal ?? 0,

                            Admin = admin,
                            Client = client,
                            Company = company,
                            Industry = industry,
                        };
            return linqs;
        }

        /// <summary>
        /// 获取自己的
        /// </summary>
        /// <returns></returns>
        public IQueryable<Project> GetOwn()
        {
            var linqs = GetAll();

            //获取所有员工
            var mystaffids = new MyStaffsView(this.AdminTop, Reponsitory);

            var linq1 = from maps in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsClient>()
                        join project in linqs on maps.ClientID equals project.ClientID
                        join admin in mystaffids on maps.AdminID equals admin.ID
                        select project;

            return linq1;
        }

        /// <summary>
        /// 根据品牌获取销售机会
        /// </summary>
        /// <param name="manufactureID"></param>
        /// <returns></returns>
        public IQueryable<Project> GetByManufacture(string manufactureID)
        {
            var projectIDs = from maps in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProject>()
                             where maps.ManufacturerID == manufactureID
                             select maps.ProjectID;

            var linqs = from project in GetAll()
                        join map in projectIDs.Distinct() on project.ID equals map
                        select project;

            return linqs;
        }
    }
}
