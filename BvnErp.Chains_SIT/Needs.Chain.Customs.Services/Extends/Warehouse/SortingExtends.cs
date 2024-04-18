using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 分拣结果扩展方法
    /// </summary>
    public static class SortingExtends
    {

        public static Layer.Data.Sqls.ScCustoms.Sortings ToLinq(this Models.Sorting entity)
        {
            return new Layer.Data.Sqls.ScCustoms.Sortings
            {
                ID = entity.ID,
                OrderID = entity.OrderID,
                OrderItemID = entity.OrderItem.ID,
                AdminID = entity.AdminID,
                EntryNoticeItemID = entity.EntryNoticeItemID,
                WarehouseType = (int)entity.WarehouseType,
                WrapType = entity.WrapType,
                //ProductID = entity.OrderItem.ID,
                Quantity = entity.Quantity,
                BoxIndex = entity.BoxIndex,
                NetWeight = entity.NetWeight,
                GrossWeight = entity.GrossWeight,
                DecStatus = (int)entity.DecStatus,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary,
                SZPackingDate=entity.SZPackingDate
            };
        }
    }
}
