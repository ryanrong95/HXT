using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Origins
{
    public class ReferrersOrigin : UniqueView<Models.Referrer, ScCustomsReponsitory>
    {
        public ReferrersOrigin()
        {
        }

        public ReferrersOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Referrer> GetIQueryable()
        {

            var adminsView = new AdminsTopView(this.Reponsitory);
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Referrers>()
                   join admin in adminsView on entity.Creator equals admin.ID into g
                   from admin in g.DefaultIfEmpty()
                   select new Models.Referrer
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       Creator = entity.Creator,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Summary = entity.Summary,
                       CreatorName = admin,
                       Status = (Enums.Status)entity.Status
                   };
        }
    }
}
