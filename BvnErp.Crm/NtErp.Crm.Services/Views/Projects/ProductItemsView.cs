using Layer.Data.Sqls;
using Needs.Erp.Generic;
using Needs.Linq;
using NtErp.Crm.Services.Models.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views.Projects
{
    /// <summary>
    /// 产品型号查询视图
    /// </summary>
    public class ProductItemsView : UniqueView<ProductItem, BvCrmReponsitory>, Needs.Underly.IFkoView<ProductItem>
    {
        string projectID;
        
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public ProductItemsView()
        {

        }

        /// <summary>
        /// 销售机会下产品型号
        /// </summary>
        /// <param name="projectID">销售机会ID</param>
        public ProductItemsView(string projectID)
        {
            this.projectID = projectID;
        }

        internal ProductItemsView(BvCrmReponsitory reponsitory) : base(reponsitory)
        {

        }
        /// <summary>
        /// 基础数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<ProductItem> GetIQueryable()
        {

            if (string.IsNullOrEmpty(this.projectID))
            {
                return this.Queryable();
            }
            else
            {
                return this.GetMapQueryable().Where(item => item.ProjectID == this.projectID);
            }
        }

        /// <summary>
        /// 获取所有人员的产品
        /// </summary>
        /// <returns></returns>
        public IQueryable<ProjectProductItem> GetAll()
        {
            var linqs = from product in this.Queryable()
                        join map in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProject>() on product.ID equals map.ProductItemID
                        join project in new ProjectsView(this.Reponsitory) on map.ProjectID equals project.ID
                        select new ProjectProductItem
                        {
                            Project = project,
                            ProductItem = product,
                        };

            return linqs;
        }

        /// <summary>
        /// 获取相关人员包含自己的产品（PMAdmin，FAEAdmin，SaleAdmin，PurChaseAdmin，AssistantAdmin）
        /// </summary>
        /// <param name="adminID"></param>
        /// <returns></returns>
        public IQueryable<ProjectProductItem> GetWith(string adminID)
        {
            Models.AdminTop Admin;
            Admin = Extends.AdminExtends.GetTop(adminID);

            if (Admin.JobType == Enums.JobType.TPM)
            {
                return GetAll();
            }
            else
            { 
                var linqs = from product in this.Queryable()
                            join map in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProject>() on product.ID equals map.ProductItemID
                            join project in new ProjectsView(this.Reponsitory) on map.ProjectID equals project.ID
                            where product.PMAdminID == adminID || product.FAEAdminID == adminID || product.SaleAdminID == adminID || product.PurchaseAdminID == adminID || product.AssistantAdiminID == adminID
                            select new ProjectProductItem
                            {
                                Project = project,
                                ProductItem = product
                            };

                return linqs;
            }
        }

        /// <summary>
        /// 获取带Map关系视图
        /// </summary>
        /// <returns></returns>
        protected IQueryable<ProductItem> GetMapQueryable()
        {
            return from map in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProject>()
                   join entity in this.Queryable() on map.ProductItemID equals entity.ID
                   select new ProductItem
                   {
                       ID = entity.ID,
                       StandardID = entity.StandardID,
                       CompeteID = entity.CompeteID,
                       RefUnitQuantity = entity.RefQuantity,
                       RefQuantity = entity.RefQuantity,
                       RefUnitPrice = entity.RefUnitPrice,
                       ExpectRate = entity.ExpectRate,
                       ExpectQuantity = entity.ExpectQuantity,
                       ExpectDate = entity.ExpectDate,
                       Status = (Enums.ProductStatus)entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       PMAdminID = entity.PMAdminID,
                       FAEAdminID = entity.FAEAdminID,
                       SaleAdminID = entity.SaleAdminID,
                       PurchaseAdminID = entity.PurchaseAdminID,
                       AssistantAdiminID = entity.AssistantAdiminID,
                       Summary = entity.Summary,
                       Voucher = entity.Voucher,

                       StandardProduct = entity.StandardProduct,
                       CompeteProduct = entity.CompeteProduct,
                       Sample = entity.Sample,
                       SaleAdmin = entity.SaleAdmin,
                       FAEAdmin = entity.FAEAdmin,
                       PMAdmin = entity.PMAdmin,
                       PurChaseAdmin = entity.PurChaseAdmin,
                       AssistantAdmin = entity.AssistantAdmin,
                       IsSample = entity.IsSample,
                       Enquires = entity.Enquires,
                       ProjectID = map.ProjectID,
                   }; ;
        }

        /// <summary>
        /// 基础视图
        /// </summary>
        /// <returns></returns>
        private IQueryable<ProductItem> Queryable()
        {
            var standardsView = new StandardProductAlls(this.Reponsitory);
            var competesView = new CompeteProductAlls(this.Reponsitory);
            var companyall = new CompanyAlls(this.Reponsitory);//公司视图
            var adminsTopview = new AdminTopView(this.Reponsitory);
            var filesView = new ProductItemFileAlls(this.Reponsitory).Where(item=>item.Type == Models.FileType.Item && item.Status == Enums.Status.Normal);

            var samplesView = from sample in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.ProductItemSamples>()
                              select new Models.Sample
                              {
                                  ID = sample.ID,
                                  Type = (Models.SampleType)sample.Type,
                                  UnitPrice = sample.UnitPrice,
                                  Quantity = sample.Quantity,
                                  TotalPrice = sample.TotalPrice,
                                  Date = sample.Date,
                                  Contactor = sample.Contactor,
                                  Phone = sample.Phone,
                                  Address = sample.Address,
                                  CreateDate = sample.CreateDate,
                                  UpdateDate = sample.UpdateDate
                              };

            var enquiriesView = new ProductItemEnquiriesView(this.Reponsitory).GetMapQueryable();

            var linqs = from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.ProductItems>()
                        join standard in standardsView on entity.StandardID equals standard.ID
                        join _sample in samplesView on entity.ID equals _sample.ID into samples
                        from sample in samples.DefaultIfEmpty()
                        join _compete in competesView on entity.CompeteID equals _compete.ID into competes
                        from compete in competes.DefaultIfEmpty()

                        join _sale in adminsTopview on entity.SaleAdmin equals _sale.ID into sales
                        from sale in sales.DefaultIfEmpty()
                        join _pm in adminsTopview on entity.PMAdmin equals _pm.ID into pms
                        from pm in pms.DefaultIfEmpty()
                        join _fea in adminsTopview on entity.FAEAdmin equals _fea.ID into feas
                        from fea in feas.DefaultIfEmpty()
                        join _purchase in adminsTopview on entity.PurChaseAdmin equals _purchase.ID into purchases
                        from purchase in purchases.DefaultIfEmpty()
                        join _assistant in adminsTopview on entity.AssistantAdmin equals _assistant.ID into assistants
                        from assistant in assistants.DefaultIfEmpty()

                        join _voucher in filesView on entity.ID equals _voucher.ProductItemID into vouchers
                        from voucher in vouchers.DefaultIfEmpty()

                        join enquiry in enquiriesView on entity.ID equals enquiry.ProductItemID into enquires

                        select new ProductItem
                        {
                            ID = entity.ID,
                            StandardID = entity.StandardID,
                            CompeteID = entity.CompeteID,
                            RefUnitQuantity = entity.RefQuantity,
                            RefQuantity = entity.RefQuantity,
                            RefUnitPrice = entity.RefUnitPrice,
                            ExpectRate = entity.ExpectRate,
                            ExpectQuantity = entity.ExpectQuantity,
                            ExpectDate = entity.ExpectDate,
                            Status = (Enums.ProductStatus)entity.Status,
                            CreateDate = entity.CreateDate,
                            UpdateDate = entity.UpdateDate,
                            PMAdminID = entity.PMAdmin,
                            FAEAdminID = entity.FAEAdmin,
                            SaleAdminID = entity.SaleAdmin,
                            PurchaseAdminID = entity.PurChaseAdmin,
                            AssistantAdiminID = entity.AssistantAdmin,
                            Summary = entity.Summary,
                            Voucher = voucher,

                            StandardProduct = standard,
                            CompeteProduct = compete,
                            Sample = sample,
                            SaleAdmin = sale,
                            FAEAdmin = fea,
                            PMAdmin = pm,
                            PurChaseAdmin = purchase,
                            AssistantAdmin = assistant,
                            IsSample = sample == null ? false : true,
                            Enquires = enquires
                        };
            return linqs;
        }
    }
}
