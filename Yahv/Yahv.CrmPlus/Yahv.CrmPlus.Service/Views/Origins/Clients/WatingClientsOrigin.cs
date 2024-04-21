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
    /// 待审批客户
    /// </summary>

    public class WaitingClientsOrigin : Yahv.Linq.UniqueView<Models.Origins.WatingClient, PvdCrmReponsitory>
    {

        internal WaitingClientsOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal WaitingClientsOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<WatingClient> GetIQueryable()
        {
            var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
            var adminsView = new AdminsAllRoll(this.Reponsitory);
            var registerView = new EnterpriseRegistersOrigin(this.Reponsitory);
            var conductsView = new ConductsOrigin(this.Reponsitory);
            var relationsView = new RelationsOrigin(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Clients>()
                   join enterprise in enterprisesView on entity.ID equals enterprise.ID
                   join register in registerView on entity.ID equals register.ID
                   join conducts in conductsView on entity.ID equals conducts.EnterpriseID
                   join relation in relationsView on entity.ID equals relation.ClientID
                   where entity.Enterprises.IsDraft && entity.Status == (int)AuditStatus.Waiting
                   select new WatingClient
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
                       Status = enterprise.Status,
                       Conduct = conducts,
                       Relation = relation,

                       //企业信息
                       Name = enterprise.Name,
                       IsDraft = enterprise.IsDraft,
                       Place = enterprise.Place,
                       District = enterprise.District,
                       Summary = enterprise.Summary,
                       // 工商信息
                       EnterpriseRegister = register,
                       ClientStatus = (AuditStatus)entity.Status,

                   };

        }

    }

}
