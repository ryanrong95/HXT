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
    public class RelationsOrigin : Yahv.Linq.UniqueView<Yahv.CrmPlus.Service.Models.Origins.Relation, PvdCrmReponsitory>
    {
        internal RelationsOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal RelationsOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Relation> GetIQueryable()
        {
            var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
            var adminsView = new AdminsAllRoll(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Relations>()
                   join company in enterprisesView on entity.CompanyID equals company.ID
                   join admin in adminsView on entity.OwnerID equals admin.ID
                   select new Relation
                   {
                       ID = entity.ID,
                       Type = (ConductType)entity.Type,
                       OwnerID = entity.OwnerID,
                       CompanyID = entity.CompanyID,
                       Company= company,
                       ClientID = entity.ClientID,
                       Status = (AuditStatus)entity.Status,
                       Summary = entity.Summary,
                       CreatorID = entity.CreatorID,
                       Admin=admin,
                       CreateDate = entity.CreateDate,
                       OfferDate = entity.OfferDate
                   };
        }
    }
}
