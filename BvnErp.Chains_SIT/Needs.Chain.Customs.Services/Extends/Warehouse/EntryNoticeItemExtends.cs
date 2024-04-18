using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 入库通知项扩展方法
    /// </summary>
    public static class EntryNoticeItemExtends
    {
        public static Layer.Data.Sqls.ScCustoms.EntryNoticeItems ToLinq(this Models.EntryNoticeItem entity)
        {
            return new Layer.Data.Sqls.ScCustoms.EntryNoticeItems
            {
                ID = entity.ID,
                EntryNoticeID = entity.EntryNoticeID,
                OrderItemID = entity.OrderItem?.ID,
                DecListID = entity.DecList?.ID,
                IsSpotCheck = entity.IsSportCheck,
                EntryNoticeStatus = (int)entity.EntryNoticeStatus,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate
            };
        }
    }
}