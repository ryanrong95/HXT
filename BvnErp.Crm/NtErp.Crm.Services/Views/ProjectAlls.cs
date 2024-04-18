using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Layer.Data.Sqls;
using Needs.Erp.Generic;
using Needs.Linq;
using Needs.Linq.Generic;
using Needs.Overall;
using Needs.Utils.Converters;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;
using NtErp.Crm.Services.Models.Generic;

namespace NtErp.Crm.Services.Views
{
    public class ProjectAlls : UniqueView<Project, BvCrmReponsitory>, Needs.Underly.IFkoView<Project>
    {
        //人员对象
        IGenericAdmin Admin;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        public ProjectAlls()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="admin">人员对象</param>
        internal ProjectAlls(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        internal ProjectAlls(BvCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        internal ProjectAlls(BvCrmReponsitory reponsitory, IQueryable<Project> iquery) : base(reponsitory, iquery)
        {

        }

        /// <summary>
        /// 获取项目数据集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Project> GetIQueryable()
        {
            return from project in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Projects>()
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
                   };
        }

        /// <summary>
        /// 根据客户ID查询
        /// </summary>
        /// <param name="ClientID"></param>
        /// <returns></returns>
        public ProjectAlls SearchByClientID(string ClientID)
        {

            var iQuery = this.OnReadShips(this.IQueryable.Where(item => item.ClientID == ClientID));

            return new ProjectAlls(this.Reponsitory, iQuery);
        }

        /// <summary>
        /// 根据ID查询数据
        /// </summary>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        public Project SearchByID(string ProjectID)
        {
            var project = this[ProjectID];

            if (project != null)
            {
                project.Industry = (from maps in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProject>()
                                    join industry in new IndustryAlls(this.Reponsitory) on maps.IndustryID equals industry.ID
                                    where maps.ProjectID == ProjectID
                                    select industry).FirstOrDefault();
            }

            return project;
        }

        /// <summary>
        /// 根据产品项ID查询
        /// </summary>
        /// <param name="ProductItemID"></param>
        /// <returns></returns>
        public Project SearchByItemID(string ProductItemID)
        {
            var projectid = from maps in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProject>()
                            where maps.ProductItemID == ProductItemID
                            select maps.ProjectID;

            return this.SearchByID(projectid.FirstOrDefault());
        }

        /// <summary>
        /// 查询关联数据
        /// </summary>
        /// <param name="projects"></param>
        /// <returns></returns>
        private IQueryable<Project> OnReadShips(IQueryable<Project> projects)
        {
            var linq = from map in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProject>()
                       join industry in new IndustryAlls(this.Reponsitory) on map.IndustryID equals industry.ID
                       select new
                       {
                           ProjectID = map.ProjectID,
                           Industry = industry,
                       };


            return from project in projects
                   join map in linq on project.ID equals map.ProjectID into maps
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
                       Industry = maps.FirstOrDefault().Industry,
                   };
        }

        /// <summary>
        /// 为导入销售机会时提供支持
        /// </summary>
        /// <returns></returns>
        private IQueryable<Project> GetProductAlls()
        {
            var linq = from map in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProject>()
                       join industry in new IndustryAlls(this.Reponsitory) on map.IndustryID equals industry.ID
                       select new
                       {
                           ProjectID = map.ProjectID,
                           Industry = industry,
                       };

            var linqs = from project in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Projects>()
                        join map in linq on project.ID equals map.ProjectID
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
                            Industry = map.Industry,
                        };

            return linqs;
        }

        /// <summary>
        /// 报备，送样，询价数据插入
        /// </summary>
        /// <param name="project"></param>
        /// <param name="productItems"></param>
        public int ExcelReportSampleEnquiryDataEnter(ProjectDossier[] projectDossiers)
        {
            var linqs = GetProductAlls();

            foreach (var item in projectDossiers)
            {
                // 检查导入的项目在系统中是否已存在,如存在获取其项目的ID
                var project = linqs.SingleOrDefault(p => p.Name == item.Project.Name && p.ClientID == item.Project.ClientID && p.ProductName == item.Project.ProductName && p.Currency == item.Project.Currency && p.CompanyID == item.Project.CompanyID && p.Industry.Name == item.Project.Industry.Name && p.Type == item.Project.Type);
                if (project != null)
                {
                    item.Project.ID = project.ID;
                }
            }

            var insertProductItemCount = 0;
            foreach (var dossier in projectDossiers)
            {
                var productItemsView = new ProductItemAlls(this.Reponsitory).SearchByProjectID(dossier.Project.ID);

                foreach (var standard in dossier.Products)
                {
                    var productItem = productItemsView.SingleOrDefault(item => item.standardProduct.Name == standard.standardProduct.Name && item.standardProduct.Manufacturer.Name == standard.standardProduct.Manufacturer.Name);
                    bool isUpdate = false;

                    if (productItem == null)
                    {
                        // 没有对应的产品型号，品牌，不进行更新
                        continue;
                    }

                    #region 更新送样信息
                    if (standard.Sample != null)
                    {
                        if (productItem.Sample != null)
                        {
                            standard.Sample.ID = productItem.Sample.ID;
                        }
                        standard.Sample.ProductItemID = productItem.ID;
                        standard.Sample.Enter();
                        isUpdate = true;
                    }
                    #endregion

                    #region 导入询价信息
                    if (standard.Enquiries.Count() > 0)
                    {
                        Enquiry enquiry = new Enquiry(productItem.ID);
                        enquiry.ReportDate = standard.Enquiry.ReportDate;
                        enquiry.ReplyPrice = standard.Enquiry.ReplyPrice;
                        enquiry.ReplyDate = standard.Enquiry.ReplyDate;
                        enquiry.RFQ = standard.Enquiry.RFQ;
                        enquiry.OriginModel = standard.Enquiry.OriginModel ?? string.Empty;
                        enquiry.MOQ = standard.Enquiry.MOQ;
                        enquiry.MPQ = standard.Enquiry.MPQ;
                        enquiry.Currency = standard.Enquiry.Currency;
                        enquiry.ExchangeRate = standard.Enquiry.ExchangeRate;
                        enquiry.TaxRate = standard.Enquiry.TaxRate;
                        enquiry.Tariff = standard.Enquiry.Tariff;
                        enquiry.OtherRate = standard.Enquiry.OtherRate;
                        enquiry.Cost = standard.Enquiry.Cost;
                        enquiry.Validity = standard.Enquiry.Validity;
                        enquiry.ValidityCount = standard.Enquiry.ValidityCount;
                        enquiry.SalePrice = standard.Enquiry.SalePrice;
                        enquiry.CreateDate = standard.Enquiry.CreateDate ?? DateTime.Now;
                        enquiry.UpdateDate = standard.Enquiry.UpdateDate ?? DateTime.Now;
                        enquiry.Summary = standard.Enquiry.Summary;
                        enquiry.Enter();
                        isUpdate = true;
                    }
                    #endregion

                    if (isUpdate)
                    {
                        insertProductItemCount++;
                    }
                }
            }
            return insertProductItemCount;
        }
        /// <summary>
        /// 机会导入
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string ExcelDataEnter2(ProjectExcelData data)
        {
            var linqs = GetProductAlls();
            List<Layer.Data.Sqls.BvCrm.Projects> projectList = new List<Layer.Data.Sqls.BvCrm.Projects>();
            // 检查导入的项目在系统中是否已存在,如存在获取其项目的ID
            var project = linqs.FirstOrDefault(p => p.Name == data.Project.Name && p.ClientID == data.Project.ClientID && p.ProductName == data.Project.ProductName && p.Currency == data.Project.Currency && p.CompanyID == data.Project.CompanyID && p.Industry.Name == data.Project.Industry.Name && p.Type == data.Project.Type);

            if (project != null)
            {
                data.Project.ID = project.ID;
                project.UpdateDate = DateTime.Now;
                project.Enter();
            }
            else
            {
                // 要导入的项目在系统中不存在时保存其内容
                var newProject = new Layer.Data.Sqls.BvCrm.Projects
                {
                    ID = data.Project.ID = PKeySigner.Pick(PKeyType.Project),
                    Name = data.Project.Name,
                    ProductName = data.Project.ProductName,
                    Type = (int)data.Project.Type,
                    ClientID = data.Project.ClientID,
                    CompanyID = data.Project.CompanyID,
                    Currency = (int)data.Project.Currency,
                    AdminID = data.Project.AdminID,
                    Contactor = data.Project.Contactor,
                    Phone = data.Project.Phone,
                    Address = data.Project.Address,
                    Valuation = 0,
                    CreateDate = data.Project.CreateDate,
                    UpdateDate = data.Project.UpdateDate,
                    Status = (int)data.Project.Status,
                    Summary = data.Project.Summary ?? string.Empty,
                };
                data.Project.ID = newProject.ID;

                Reponsitory.Insert(newProject);
            }
            var productItemsView = new ProductItemAlls(this.Reponsitory).SearchByProjectID(data.Project.ID);
            var productItem = productItemsView.FirstOrDefault(item => item.standardProduct.Name == data.ProductItem.standardProduct.Name && item.standardProduct.Manufacturer.Name == data.ProductItem.standardProduct.Manufacturer.Name);

            // 当前项目下已经存在的型号不予导入
            if (productItem == null)
            {
                data.ProductItem.StandardEnter(Reponsitory);
                if (data.ProductItem.CompeteProduct != null)
                {
                    Reponsitory.Insert(new Layer.Data.Sqls.BvCrm.CompeteProducts()
                    {
                        ID = data.ProductItem.CompeteProduct.ID = Guid.NewGuid().ToString(),
                        Name = data.ProductItem.CompeteProduct.Name ?? string.Empty,
                        ManufacturerID = data.ProductItem.CompeteProduct.ManufacturerID ?? string.Empty,
                        UnitPrice = data.ProductItem.CompeteProduct.UnitPrice,
                        CreateDate = DateTime.Now,
                    });
                }
                Reponsitory.Insert(new Layer.Data.Sqls.BvCrm.ProductItems
                {
                    ID = data.ProductItem.ID = PKeySigner.Pick(PKeyType.Product),
                    StandardID = data.ProductItem.standardProduct.ID,
                    CompeteID = data.ProductItem.CompeteProduct?.ID,
                    RefUnitQuantity = data.ProductItem.RefUnitQuantity,
                    RefQuantity = data.ProductItem.RefQuantity,
                    RefUnitPrice = data.ProductItem.RefUnitPrice,
                    ExpectRate = data.ProductItem.ExpectRate,
                    //Status = (int)data.ProductItem.Status,
                    Status = (int)ProductStatus.DO,
                    ExpectDate = data.ProductItem.ExpectDate,
                    ExpectQuantity = data.ProductItem.ExpectQuantity,
                    CreateDate = data.ProductItem.CreateDate,
                    UpdateDate = data.ProductItem.UpdateDate,
                    PMAdmin = data.ProductItem.PMAdminID,
                    FAEAdmin = data.ProductItem.FAEAdminID,
                    SaleAdmin = data.ProductItem.SaleAdminID,
                    PurChaseAdmin = data.ProductItem.PurchaseAdminID,
                    AssistantAdmin = data.ProductItem.AssistantAdiminID,
                });
                if (data.ProductItem.Sample != null)
                {
                    data.ProductItem.Sample.ProductItemID = data.ProductItem.ID;
                    data.ProductItem.Sample.Enter();
                }

                this.BindingItem(data.Project.ID, new ProductItem[] { data.ProductItem });
                this.BindingIndustry(data.Project.ID, data.Project.Industry);
            }
            else
            {
                return "品牌型号已存在";
            }

            return "";
        }
        /// <summary>
        /// 报备，送样，询价数据插入
        /// </summary>
        /// <param name="project"></param>
        /// <param name="productItems"></param>
        public string ExcelReportEnter(ProjectExcelData data)
        {
            var linqs = GetProductAlls();

            var project = this.GetProductAlls().FirstOrDefault(p => p.Name == data.Project.Name && p.ClientID == data.Project.ClientID && p.ProductName == data.Project.ProductName && p.Currency == data.Project.Currency && p.CompanyID == data.Project.CompanyID && p.Industry.Name == data.Project.Industry.Name && p.Type == data.Project.Type);
            if (project == null)
            {
                return "数据不存在";
            }
            else
            {
                data.Project.ID = project.ID;
            }

            var productItemsView = new ProductItemAlls(this.Reponsitory).SearchByProjectID(data.Project.ID);

            var productItem = productItemsView.FirstOrDefault(item => item.standardProduct.Name == data.ProductItem.standardProduct.Name && item.standardProduct.Manufacturer.Name == data.ProductItem.standardProduct.Manufacturer.Name);

            if (productItem == null)
            {
                return "产品型号或品牌不存在";
            }
            bool isExcute = false;
            #region 更新送样信息
            if (data.ProductItem.Sample != null)
            {
                if (productItem.Sample != null)
                {
                    data.ProductItem.Sample.ID = productItem.Sample.ID;
                }
                data.ProductItem.Sample.ProductItemID = productItem.ID;
                data.ProductItem.Sample.Enter();

                isExcute = true;
            }
            #endregion

            #region 导入询价信息
            if (data.ProductItem.Enquiries.Count() > 0)
            {
                Enquiry enquiry = new Enquiry(productItem.ID);
                enquiry.ReportDate = data.ProductItem.Enquiry.ReportDate;
                enquiry.ReplyPrice = data.ProductItem.Enquiry.ReplyPrice;
                enquiry.ReplyDate = data.ProductItem.Enquiry.ReplyDate;
                enquiry.RFQ = data.ProductItem.Enquiry.RFQ;
                enquiry.OriginModel = data.ProductItem.Enquiry.OriginModel ?? string.Empty;
                enquiry.MOQ = data.ProductItem.Enquiry.MOQ.HasValue && data.ProductItem.Enquiry.MOQ > 0 ? data.ProductItem.Enquiry.MOQ : productItem.Enquiry?.MOQ;
                enquiry.MPQ = data.ProductItem.Enquiry.MPQ;
                enquiry.Currency = data.ProductItem.Enquiry.Currency;
                enquiry.ExchangeRate = data.ProductItem.Enquiry.ExchangeRate;
                enquiry.TaxRate = data.ProductItem.Enquiry.TaxRate;
                enquiry.Tariff = data.ProductItem.Enquiry.Tariff;
                enquiry.OtherRate = data.ProductItem.Enquiry.OtherRate;
                enquiry.Cost = data.ProductItem.Enquiry.Cost;
                enquiry.Validity = data.ProductItem.Enquiry.Validity;
                enquiry.ValidityCount = data.ProductItem.Enquiry.ValidityCount;
                enquiry.SalePrice = data.ProductItem.Enquiry.SalePrice;
                enquiry.CreateDate = data.ProductItem.Enquiry.CreateDate ?? DateTime.Now;
                enquiry.UpdateDate = data.ProductItem.Enquiry.UpdateDate ?? DateTime.Now;
                enquiry.Summary = data.ProductItem.Enquiry.Summary;
                enquiry.Enter();
                isExcute = true;
            }
            #endregion
            if (isExcute)
            {
                project.Enter();
            }
            return "";
        }

        /// <summary>
        /// 产品绑定
        /// </summary>
        /// <param name="projectId">销售机会ID</param>
        /// <param name="item">产品</param>
        public void BindingItem(string projectId, ProductItem[] items)
        {
            if (items.Count() == 0)
            {
                return;
            }

            using (BvCrmReponsitory reponsitory = new BvCrmReponsitory())
            {
                foreach (var item in items)
                {
                    reponsitory.Delete<Layer.Data.Sqls.BvCrm.MapsProject>(c => c.ProjectID == projectId && c.ProductItemID == item.ID);

                    reponsitory.Insert(new Layer.Data.Sqls.BvCrm.MapsProject
                    {
                        ID = string.Concat(projectId, item.ID, item.standardProduct.Manufacturer.ID).MD5(),
                        ProjectID = projectId,
                        ProductItemID = item.ID,
                        ManufacturerID = item.standardProduct.Manufacturer.ID
                    });
                }
            }
        }

        /// <summary>
        /// 绑定行业
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <param name="industry">行业</param>
        public void BindingIndustry(string projectId, Industry industry)
        {
            if (industry == null)
            {
                return;
            }
            using (BvCrmReponsitory reponsitory = new BvCrmReponsitory())
            {
                reponsitory.Delete<Layer.Data.Sqls.BvCrm.MapsProject>(c => c.ProjectID == projectId && c.IndustryID != null);

                reponsitory.Insert(new Layer.Data.Sqls.BvCrm.MapsProject
                {
                    ID = string.Concat(projectId, industry.ID).MD5(),
                    ProjectID = projectId,
                    IndustryID = industry.ID,
                });
            }
        }
    }
}
