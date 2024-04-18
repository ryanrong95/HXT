using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views.Origins
{
    internal class EntryNoticesOrigin : UniqueView<Models.EntryNotice, ScCustomsReponsitory>
    {
        internal EntryNoticesOrigin()
        {
        }

        internal EntryNoticesOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<EntryNotice> GetIQueryable()
        {
            return from notice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNotices>()
                   select new Models.EntryNotice
                   {
                       ID = notice.ID,
                       OrderID = notice.OrderID,
                       DecHeadID = notice.DecHeadID,
                       ClientCode = notice.ClientCode,
                       SortingRequire = (Enums.SortingRequire)notice.SortingRequire,
                       WarehouseType = (Enums.WarehouseType)notice.WarehouseType,
                       EntryNoticeStatus = (Enums.EntryNoticeStatus)notice.EntryNoticeStatus,
                       Status = (Enums.Status)notice.Status,
                       CreateDate = notice.CreateDate,
                       UpdateDate = notice.UpdateDate,
                       Summary = notice.Summary
                   };
        }
    }
}
