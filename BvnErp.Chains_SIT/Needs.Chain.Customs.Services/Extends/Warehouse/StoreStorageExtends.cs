using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 出库人信息扩展方法
    /// </summary>
    public static class StoreStorageExtends
    {
        public static Layer.Data.Sqls.ScCustoms.StoreStorages ToLinq(this Models.StoreStorage entity)
        {
            return new Layer.Data.Sqls.ScCustoms.StoreStorages
            {
                ID = entity.ID,                
                OrderItemID = entity.OrderItemID,
                SortingID = entity.Sorting.ID,
                Purpose = (int)entity.Purpose,
                StockCode = entity.StockCode,
                Quantity = entity.Quantity,
                BoxIndex = entity.BoxIndex,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,  
                Summary = entity.Summary,              
            };
        }
    }
}
