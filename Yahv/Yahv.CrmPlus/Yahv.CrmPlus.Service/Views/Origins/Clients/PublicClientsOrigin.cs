using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.Service.Views.Origins
{
    /// <summary>
    /// 公海客户
    /// </summary>

    public class PublicClientsOrigin : Yahv.Linq.UniqueView<Models.Origins.PublicClient, PvdCrmReponsitory>
    {

        public PublicClientsOrigin()
        {
        }
        protected override IQueryable<Models.Origins.PublicClient> GetIQueryable()
        {
            var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Clients>()
                   join enterprise in enterprisesView on entity.ID equals enterprise.ID
                   join pc in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.PublicClientIDTopView>() on enterprise.ID equals pc.ID
                   select new PublicClient
                   {
                       ID = entity.ID,
                       ClientGrade = (ClientGrade)entity.Grade,
                       ClientType = (Yahv.Underly.CrmPlus.ClientType)entity.Type,
                       Vip = (Yahv.Underly.VIPLevel)entity.Vip,
                       Source = entity.Source,
                       IsMajor = entity.IsMajor,
                       IsSpecial = entity.IsSpecial,
                       Industry = entity.Industry,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       IsSupplier = entity.IsSupplier,
                       ProfitRate = entity.ProfitRate,
                       //企业
                       Name = enterprise.Name,
                       IsDraft = enterprise.IsDraft,
                       Place = enterprise.Place,
                       District = enterprise.District,
                       Grade = enterprise.Grade,
                       Summary = enterprise.Summary,
                       EnterpriseRegister = enterprise.EnterpriseRegister,
                       Status = (AuditStatus)enterprise.Status,
                       ClientStatus = (AuditStatus)entity.Status
                   };
        }

    }

    /// <summary>
    /// 认领审批时 使用
    /// </summary>
    public class PublicClientsExtendOrigin : Yahv.Linq.UniqueView<Models.Origins.PublicClient, PvdCrmReponsitory>
    {

        public PublicClientsExtendOrigin()
        {
        }

        public PublicClientsExtendOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {


        }
        protected override IQueryable<Models.Origins.PublicClient> GetIQueryable()
        {
            // var AdminTopview = new AdminsAllRoll(this.Reponsitory);
            var relationView = new RelationsOrigin(this.Reponsitory).Where(x => x.Status == AuditStatus.Waiting);
            var conductView = new ConductsOrigin(this.Reponsitory);
            var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
            var registerView = new EnterpriseRegistersOrigin(this.Reponsitory);
            var applyTaskView = Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.ApplyTasks>();
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Clients>()
                   join enterprise in enterprisesView on entity.ID equals enterprise.ID
                   join applyTask in applyTaskView on entity.ID  equals applyTask.MainID
                   join relation in relationView on applyTask.ApplierID equals relation.OwnerID
                  join conduct in conductView on entity.ID equals conduct.EnterpriseID
                   into _conducts
                   from conduct in _conducts.DefaultIfEmpty()
                   where applyTask.ApplyTaskType==(int)ApplyTaskType.ClientPublic
                   select new PublicClient
                   {
                       ID = entity.ID,
                       ClientGrade = (ClientGrade)entity.Grade,
                       ClientType = (Yahv.Underly.CrmPlus.ClientType)entity.Type,
                       Vip = (Yahv.Underly.VIPLevel)entity.Vip,
                       Source = entity.Source,
                       IsMajor = entity.IsMajor,
                       IsSpecial = entity.IsSpecial,
                       Industry = entity.Industry,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       IsSupplier = entity.IsSupplier,
                       ProfitRate = entity.ProfitRate,

                       Name = enterprise.Name,
                       IsDraft = enterprise.IsDraft,
                       Place = enterprise.Place,
                       District = enterprise.District,
                       Grade = enterprise.Grade,
                       Summary = enterprise.Summary,

                       EnterpriseRegister = enterprise.EnterpriseRegister,
                       Status = (AuditStatus)entity.Status,
                       Relation = relation,
                       Conduct = conduct
                   };
        }

    }
}
