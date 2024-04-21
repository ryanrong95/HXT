using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.Service.Views.Origins
{
    public class SupplierDishonestsOrigin : Yahv.Linq.UniqueView<SupplierDisHonest, PvdCrmReponsitory>
    {
        public SupplierDishonestsOrigin()
        {


        }
        public SupplierDishonestsOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {


        }

        protected override IQueryable<SupplierDisHonest> GetIQueryable()
        {
            var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
            var adminsView = new AdminsAllRoll(this.Reponsitory);

            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Dishonests>()
                   join enterprise in enterprisesView on entity.EnterpriseID equals enterprise.ID
                   join supplier in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Suppliers>() on entity.EnterpriseID equals supplier.ID
                   join admin in adminsView on entity.CreatorID equals admin.ID into g
                   from admin in g.DefaultIfEmpty()
                   select new SupplierDisHonest
                   {
                       ID = entity.ID,
                       SupplierID = supplier.ID,
                       EnterpriseID = entity.EnterpriseID,
                       EnterpriseName = enterprise.Name,
                       CreateDate = entity.CreateDate,
                       Code = entity.Code,
                       OccurTime = entity.OccurTime,
                       Reason = entity.Reason,
                       Summary = entity.Summary,
                       Creator = admin,
                       Status = (Underly.DataStatus)entity.Status
                   };
        }
    }
}
