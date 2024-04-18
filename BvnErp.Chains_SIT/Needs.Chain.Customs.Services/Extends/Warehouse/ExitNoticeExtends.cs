using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 出库通知扩展方法
    /// </summary>
    public static class ExitNoticeExtends
    {
        public static Layer.Data.Sqls.ScCustoms.ExitNotices ToLinq(this Models.ExitNotice entity)
        {
            return new Layer.Data.Sqls.ScCustoms.ExitNotices
            {
                ID = entity.ID,
                OrderID = entity.Order.ID,
                AdminID = entity.Admin.ID,
                DecHeadID = entity.DecHead?.ID,
                WarehouseType = (int)entity.WarehouseType,
                ExitType = (int)entity.ExitType,
                ExitNoticeStatus = (int)entity.ExitNoticeStatus,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary,
                IsPrint= entity.IsPrint==null?0:(int)entity.IsPrint
            };
        }
    }
}
