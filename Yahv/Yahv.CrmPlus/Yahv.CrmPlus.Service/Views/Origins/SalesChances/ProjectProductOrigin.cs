using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;
using Yahv.Underly.CrmPlus;

namespace Yahv.CrmPlus.Service.Views.Origins
{
    public class ProjectProductOrigin : Yahv.Linq.UniqueView<ProjectProduct, PvdCrmReponsitory>
    {

        public  ProjectProductOrigin()
        {
        }

        public ProjectProductOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<ProjectProduct> GetIQueryable()
        {
            var applies = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.ApplyTasks>().Where(item => item.Status == (int)ApplyStatus.Waiting);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.ProjectProducts>()
                  
                   join apply in applies on entity.ID equals apply.MainID into _applies
                   select new ProjectProduct
                   {
                       ID=entity.ID,
                       ProjectID=entity.ProjectID,
                    
                       AssignClientID=entity.AssignClientID,
                       SpnID=entity.SpnID,
                       UnitProduceQuantity=entity.UnitProduceQuantity,
                       ProduceQuantity=entity.ProduceQuantity,
                       ExpectQuantity=entity.ExpectQuantity,
                       Currency=(Currency)entity.Currency,
                       ExpectUnitPrice=entity.ExpectUnitPrice,
                       UnitPrice=entity.UnitPrice,
                       Summary=entity.Summary,
                       CreateDate=entity.CreateDate,
                       ModifyDate=entity.ModifyDate,
                       ProjectStatus=(ProductStatus)entity.ProjectStatus,
                       Status=(Yahv.Underly.AuditStatus)entity.Status,
                       IsApr = _applies.Count() > 0,

                   };

        }
    }


    /// <summary>
    /// 状态审批 使用
    /// </summary>
    public class ProjectProductExtendOrigin : Yahv.Linq.UniqueView<ProjectProduct, PvdCrmReponsitory>
    {

        public ProjectProductExtendOrigin()
        {
        }

        public ProjectProductExtendOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<ProjectProduct> GetIQueryable()
        {
            var applies = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.ApplyTasks>().Where(item => item.Status == (int)ApplyStatus.Waiting &&item.ApplyTaskType == (int)ApplyTaskType.ClientProjectStatus);
            var mainids = applies.Select(x => x.MainID).ToArray();
            var productView = Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.ProjectProducts>().Where(x => mainids.Contains(x.ID));
            var standardPartNumberView = new StandardPartNumbersOrigin(this.Reponsitory);
            var projectView = new ProjectOrigin(this.Reponsitory);
            return from entity in productView
                   join partNumber in standardPartNumberView on entity.SpnID equals partNumber.ID
                   join project in projectView on entity.ProjectID equals project.ID
                   join apply in applies on entity.ID equals apply.MainID into _applies
                   select new ProjectProduct
                   {
                       ID = entity.ID,
                       ProjectID = entity.ProjectID,
                       Project = project,
                       AssignClientID = entity.AssignClientID,
                       SpnID = entity.SpnID,
                       StandardPartNumber=partNumber,
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
                       IsApr = _applies.Count() > 0,

                   };

        }
    }
}
