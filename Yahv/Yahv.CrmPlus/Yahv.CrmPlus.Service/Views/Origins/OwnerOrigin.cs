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
    public class OwnerOrigin : Yahv.Linq.UniqueView<Yahv.CrmPlus.Service.Models.Origins.Relation, PvdCrmReponsitory>
    {
        internal OwnerOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal OwnerOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Relation> GetIQueryable()
        {
            var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
            var adminsView = new AdminsAllRoll(this.Reponsitory);
            var conductView = new ConductsOrigin(this.Reponsitory);
            var relationView = new RelationsOrigin(this.Reponsitory);
            //var conducts = from conduct in conductView
            //               group conduct by conduct.EnterpriseID into newgroup
            //               orderby newgroup.Key
            //               select new
            //               {
            //                   EnterpriseID = newgroup.Key,
            //                   ConductTypes = string.Join(",", newgroup.Select(x => x.ConductType).Distinct().ToArray())
            //               };

            //var companys = from relation in relationView
            //                group relation by new { relation.ClientID ,relation.OwnerID} into newgroup
            //                orderby newgroup.Key
            //                select new
            //                {
            //                    ClientID = newgroup.Key,
            //                    Companys = string.Join(",", newgroup.Select(x => x.Company.Name).Distinct().ToArray()),

            //                };

            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Relations>()
                   join enterprise1 in enterprisesView on entity.ClientID equals enterprise1.ID
                   join enterprise2 in enterprisesView on entity.CompanyID equals enterprise2.ID
                   join admin in adminsView on entity.OwnerID equals admin.ID into g
                   from admin in g.DefaultIfEmpty()
                   select new Relation
                   {
                       ID = entity.ID,
                       Type = (ConductType)entity.Type,
                       OwnerID = entity.OwnerID,
                       CompanyID = entity.CompanyID,
                       Enterprise = enterprise1,
                       Company = enterprise2,
                       ClientID = entity.ClientID,
                       Status = (AuditStatus)entity.Status,
                       Summary = entity.Summary,
                       CreatorID = entity.CreatorID,
                       Admin = admin,
                       CreateDate = entity.CreateDate,
                       OfferDate = entity.OfferDate
                   };
        }
    }

}
