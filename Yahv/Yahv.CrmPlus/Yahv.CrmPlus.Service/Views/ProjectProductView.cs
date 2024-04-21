using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.CrmPlus.Service.Views
{

    public class ProjectProductModel : IEntity
    {

        public string ID { get; set; }

        public string ProjectID { get; set; }

        public Project Project { get; set; }

        /// <summary>
        /// （实际下单）客户
        /// </summary>

        public string AssignClientID { get; set; }

        /// <summary>
        /// 标准型号
        /// </summary>
        public string SpnName { get; set; }
        /// <summary>
        /// 品牌名称
        /// </summary>
        public string BrandName { get; set; }
        /// <summary>
        /// 单机用量
        /// </summary>
        public int UnitProduceQuantity { get; set; }
        /// <summary>
        /// 项目用量
        /// </summary>
        public int ProduceQuantity { get; set; }
        /// <summary>
        /// 预计成交量
        /// </summary>
        public int? ExpectQuantity { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 参考单价
        /// </summary>
        public decimal ExpectUnitPrice { set; get; }
        /// <summary>
        /// 报价单价
        /// </summary>
        public decimal? UnitPrice { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 项目状态
        /// </summary>
        public ProductStatus ProjectStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Yahv.Underly.AuditStatus Status { get; set; }

    }
    public class ProjectProductView : vDepthView<Models.Origins.ProjectProduct, ProjectProductModel, PvdCrmReponsitory>
    {
        public ProjectProductView()
        {

        }

        string ProjectID;
        public ProjectProductView(string id)
        {
            this.ProjectID = id;
        }
        public ProjectProductView(IQueryable<Models.Origins.ProjectProduct> iQueryable) : base(iQueryable)
        {
        }
        protected ProjectProductView(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.Origins.ProjectProduct> GetIQueryable()
        {

            if (!string.IsNullOrEmpty(ProjectID))
            {
                return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.ProjectProducts>()
                       where entity.ProjectID == ProjectID
                       select new Models.Origins.ProjectProduct
                       {
                           ID = entity.ID,
                           ProjectID = entity.ProjectID,
                           AssignClientID = entity.AssignClientID,
                           SpnID = entity.SpnID,
                           UnitProduceQuantity = entity.UnitProduceQuantity,
                           ProduceQuantity = entity.ProduceQuantity,
                           ExpectQuantity = entity.ExpectQuantity,
                           Currency = (Currency)entity.Currency,
                           ExpectUnitPrice = entity.ExpectUnitPrice,
                           UnitPrice = entity.UnitPrice,
                           Summary = entity.Summary,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,
                           ProjectStatus = (ProductStatus)entity.ProjectStatus,
                           Status = (Yahv.Underly.AuditStatus)entity.Status,
                       };


            }
            else
            {

                return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.ProjectProducts>()
                       select new Models.Origins.ProjectProduct
                       {
                           ID = entity.ID,
                           ProjectID = entity.ProjectID,
                           AssignClientID = entity.AssignClientID,
                           SpnID = entity.SpnID,
                           UnitProduceQuantity = entity.UnitProduceQuantity,
                           ProduceQuantity = entity.ProduceQuantity,
                           ExpectQuantity = entity.ExpectQuantity,
                           Currency = (Currency)entity.Currency,
                           ExpectUnitPrice = entity.ExpectUnitPrice,
                           UnitPrice = entity.UnitPrice,
                           Summary = entity.Summary,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,
                           ProjectStatus = (ProductStatus)entity.ProjectStatus,
                           Status = (Yahv.Underly.AuditStatus)entity.Status,
                       };

            }

        }


        protected override IEnumerable<ProjectProductModel> OnMyPage(IQueryable<Models.Origins.ProjectProduct> iquery)
        {
            var data = iquery.ToArray();
            var spnIds = data.Select(x => x.SpnID).Distinct();
            var linq_standardPartNumber =
                from standardPartNumber in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.StandardPartNumbers>()
                join brand in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.StandardBrands>() on standardPartNumber.BrandID equals brand.ID
                where spnIds.Contains(standardPartNumber.ID)
                select new
                {
                    ID = standardPartNumber.ID,
                    PartNumber = standardPartNumber.PartNumber,
                    BrandName = brand.Name,
                    standardPartNumber.BrandID
                };
            var standardPartNumberArr = linq_standardPartNumber.ToArray();
            return from entity in data
                   join standardPartNumber in standardPartNumberArr on entity.SpnID equals standardPartNumber.ID
                   select new ProjectProductModel
                   {
                       ID = entity.ID,
                       ProjectID = entity.ProjectID,
                       AssignClientID = entity.AssignClientID,
                       SpnName = standardPartNumber.PartNumber,
                       BrandName = standardPartNumber.BrandName,
                       UnitProduceQuantity = entity.UnitProduceQuantity,
                       ProduceQuantity = entity.ProduceQuantity,
                       ExpectQuantity = entity.ExpectQuantity,
                       Currency = (Currency)entity.Currency,
                       ExpectUnitPrice = entity.ExpectUnitPrice,
                       UnitPrice = entity.UnitPrice,
                       Summary = entity.Summary,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       ProjectStatus = (ProductStatus)entity.ProjectStatus,
                       Status = (Yahv.Underly.AuditStatus)entity.Status
                   };

        }
    }
}
