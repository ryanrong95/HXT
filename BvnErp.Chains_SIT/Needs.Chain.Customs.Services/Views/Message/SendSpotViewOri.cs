using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class SendSpotViewOri : UniqueView<Models.SendSpotModel, ScCustomsReponsitory>
    {
        public SendSpotViewOri()
        {
        }

        internal SendSpotViewOri(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.SendSpotModel> GetIQueryable()
        {
            return from spot in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SendSpot>()

                   select new Models.SendSpotModel
                   {
                       ID = spot.ID,
                       Name = spot.Name,
                       Type = (SpotName)spot.Type,
                       SystemID = (BusinessType)spot.SystemID,
                       SendMessage = spot.SendMessage,
                       Status = (Status)spot.Status,
                       CreateDate = spot.CreateDate,
                       UpdateDate = spot.UpdateDate,
                       Summary = spot.Summary,
                   };
        }
    }
}
