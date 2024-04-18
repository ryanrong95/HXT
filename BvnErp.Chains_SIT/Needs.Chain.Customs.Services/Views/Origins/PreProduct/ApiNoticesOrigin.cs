using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Origins
{
    internal class ApiNoticesOrigin : UniqueView<Models.ApiNotice, ScCustomsReponsitory>
    {
        internal ApiNoticesOrigin()
        {
        }

        internal ApiNoticesOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ApiNotice> GetIQueryable()
        {
            return from notice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ApiNotices>()
                   select new Models.ApiNotice
                   {
                       ID = notice.ID,
                       PushType = (Enums.PushType)notice.PushType,
                       ClientID = notice.ClientID,
                       ItemID = notice.ItemID,
                       PushStatus = (Enums.PushStatus)notice.PushStatus,
                       CreateDate = notice.CreateDate,
                       UpdateDate = notice.UpdateDate
                   };
        }
    }
}
