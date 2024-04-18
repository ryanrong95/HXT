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
    internal class EntryNoticeItemsOrigin : UniqueView<Models.EntryNoticeItem, ScCustomsReponsitory>
    {
        internal EntryNoticeItemsOrigin()
        {
        }

        internal EntryNoticeItemsOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<EntryNoticeItem> GetIQueryable()
        {
            return from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNoticeItems>()
                   select new Models.EntryNoticeItem
                   {
                       ID = item.ID,
                       EntryNoticeID = item.EntryNoticeID,
                       OrderItemID = item.OrderItemID,
                       DecListID = item.DecListID,
                       IsSportCheck = item.IsSpotCheck.GetValueOrDefault(),
                       EntryNoticeStatus = (Enums.EntryNoticeStatus)item.EntryNoticeStatus,
                       Status = (Enums.Status)item.Status,
                       CreateDate = item.CreateDate,
                       UpdateDate = item.UpdateDate
                   };
        }
    }
}
