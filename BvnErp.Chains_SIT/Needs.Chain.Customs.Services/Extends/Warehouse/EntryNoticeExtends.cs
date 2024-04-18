using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 入库通知扩展方法
    /// </summary>
    public static partial class EntryNoticeExtends
    {
        public static Layer.Data.Sqls.ScCustoms.EntryNotices ToLinq(this Models.EntryNotice entity)
        {
            return new Layer.Data.Sqls.ScCustoms.EntryNotices
            {
                ID = entity.ID,
                OrderID = entity.Order?.ID,
                DecHeadID = entity.DecHead?.ID,
                WarehouseType = (int)entity.WarehouseType,
                ClientCode = entity.ClientCode,
                SortingRequire = (int)entity.SortingRequire,
                EntryNoticeStatus = (int)entity.EntryNoticeStatus,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary
            };
        }
    }

    public static partial class HKEntryNoticeExtends
    {
        public static Layer.Data.Sqls.ScCustoms.EntryNotices ToLinq(this Models.HKEntryNotice entity)
        {
            return new Layer.Data.Sqls.ScCustoms.EntryNotices
            {
                ID = entity.ID,
                OrderID = entity.Order?.ID,
                WarehouseType = (int)entity.WarehouseType,
                ClientCode = entity.ClientCode,
                SortingRequire = (int)entity.SortingRequire,
                EntryNoticeStatus = (int)entity.EntryNoticeStatus,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary
            };
        }
    }

    /// <summary>
    /// 入库通知扩展方法
    /// </summary>
    public static partial class SZEntryNoticeExtends
    {
        public static Layer.Data.Sqls.ScCustoms.EntryNotices ToLinq(this Models.SZEntryNotice entity)
        {
            return new Layer.Data.Sqls.ScCustoms.EntryNotices
            {
                ID = entity.ID,
                OrderID = entity.Order?.ID,
                DecHeadID = entity.DecHead.ID,
                WarehouseType = (int)entity.WarehouseType,
                ClientCode = entity.ClientCode,
                SortingRequire = (int)entity.SortingRequire,
                EntryNoticeStatus = (int)entity.EntryNoticeStatus,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary
            };
        }
    }
}
