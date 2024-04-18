using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 出库通知项扩展方法
    /// </summary>
    public static class ExitNoticeItemExtends
    {
        public static Layer.Data.Sqls.ScCustoms.ExitNoticeItems ToLinq(this Models.ExitNoticeItem entity)
        {
            return new Layer.Data.Sqls.ScCustoms.ExitNoticeItems
            {
                ID = entity.ID,
                ExitNoticeID = entity.ExitNoticeID,
                DecListID = entity.DecList?.ID,
                SortingID = entity.Sorting?.ID,
                Quantity = entity.Quantity,
                ExitNoticeStatus = (int)entity.ExitNoticeStatus,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate
            };
        }
    }
}
