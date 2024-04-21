
using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Origins;
using Yahv.Linq;
using Yahv.Underly;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace YaHv.CrmPlus.Service.Views.Origins
{
    /// <summary>
    /// 地址管理
    /// </summary>
    public class AddressesOrigin : Yahv.Linq.UniqueView<Yahv.CrmPlus.Service.Models.Origins.Address, Layers.Data.Sqls.PvdCrmReponsitory>
    {
        public  AddressesOrigin()
        {


        }
      public   AddressesOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {


        }

        protected override IQueryable<Yahv.CrmPlus.Service.Models.Origins.Address> GetIQueryable()
        {
            var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
            var adminsView = new AdminsAllRoll(this.Reponsitory);

            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Addresses>()
                   join enterprise in enterprisesView on entity.EnterpriseID equals enterprise.ID
                   join admin in adminsView on entity.CreatorID equals admin.ID into g
                   from admin in g.DefaultIfEmpty()
                   select new Address
                   {
                       ID = entity.ID,
                       EnterpriseID = entity.EnterpriseID,
                       Enterprise=enterprise,
                       RelationType = (RelationType)entity.RelationType,
                       AddressType=(AddressType)entity.Type,
                       Title=entity.Title,
                       District=entity.District,
                       Context=entity.Context,
                       Contact=entity.Contact,
                       Phone=entity.Phone,
                       PostZip=entity.PostZip,
                       DyjCode=entity.DyjCode,
                       Status=(AuditStatus)entity.Status,
                       CreatorID=entity.CreatorID,
                       CreateDate=entity.CreateDate,
                       ModifyDate=entity.ModifyDate,
                       Admin= admin
                   };
        }

    }

}
