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

    public class ProjectCompeleteModel : IEntity
    {

        public string ID { get; set; }
        /// <summary>
        /// 销售机会ID
        /// </summary>

        public string ProjectID { get; set; }
        /// <summary>
        /// 销售机会产品项ID
        /// </summary>
        public string ProjectProductID { get; set; }
        /// <summary>
        /// 标准型号
        /// </summary>
        public string SpnName { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public decimal UnitPrice { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ModifyDate { get; set; }

        public DataStatus DataStatus { get; set; }

    }
    public class ProjectCompeleteView : vDepthView<Models.Origins.ProjectCompelete, ProjectCompeleteModel, PvdCrmReponsitory>
    {
        public ProjectCompeleteView()
        {

        }

        string ProjectProductID;
        public ProjectCompeleteView(string id)
        {
            this.ProjectProductID = id;
        }
        public ProjectCompeleteView(IQueryable<Models.Origins.ProjectCompelete> iQueryable) : base(iQueryable)
        {
        }
        protected ProjectCompeleteView(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.Origins.ProjectCompelete> GetIQueryable()
        {
            if (!string.IsNullOrEmpty(ProjectProductID))
            {
                return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.ProjectCompeletes>()
                       where entity.ProjectProductID == ProjectProductID && entity.Status == (int)DataStatus.Normal
                       select new Models.Origins.ProjectCompelete
                       {
                           ID = entity.ID,
                           ProjectID = entity.ProjectID,
                           ProjectProductID = entity.ProjectProductID,
                           ProductID = entity.ProductID,
                           SpnID = entity.SpnID,
                           CreatorID = entity.CreatorID,
                           UnitPrice = entity.UnitPrice,
                           DataStatus = (DataStatus)entity.Status,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate
                       };


            }
            else
            {
                return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.ProjectCompeletes>()
                       where entity.Status == (int)DataStatus.Normal
                       select new Models.Origins.ProjectCompelete
                       {
                           ID = entity.ID,
                           ProjectID = entity.ProjectID,
                           ProjectProductID = entity.ProjectProductID,
                           ProductID = entity.ProductID,
                           SpnID = entity.SpnID,
                           CreatorID = entity.CreatorID,
                           UnitPrice = entity.UnitPrice,
                           DataStatus = (DataStatus)entity.Status,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate
                       };
            }
        }


        protected override IEnumerable<ProjectCompeleteModel> OnMyPage(IQueryable<Models.Origins.ProjectCompelete> iquery)
        {
            var data = iquery.ToArray();
            var spnsId = data.Select(item => item.SpnID).Distinct();
            var linq_standardPartNumber =
                from standardPartNumber in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.StandardPartNumbers>()
                join brand in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.StandardBrands>() on standardPartNumber.BrandID equals brand.ID
                where spnsId.Contains(standardPartNumber.ID)
                select new
                {
                    ID = standardPartNumber.ID,
                    PartNumber = standardPartNumber.PartNumber,
                    standardPartNumber.BrandID,
                    BrandName = brand.Name
                };

              
            var standardPartNumberArr = linq_standardPartNumber.ToArray();

            return from entity in data
                   join standardPartNumber in standardPartNumberArr on entity.SpnID equals standardPartNumber.ID
                   select new ProjectCompeleteModel
                   {
                       ID = entity.ID,
                       ProjectID = entity.ProjectID,
                       ProjectProductID = entity.ProjectProductID,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       DataStatus = (DataStatus)entity.DataStatus,
                       UnitPrice = entity.UnitPrice,
                       Brand = standardPartNumber.BrandName,
                       SpnName = standardPartNumber.PartNumber
                   };

        }
    }
}
