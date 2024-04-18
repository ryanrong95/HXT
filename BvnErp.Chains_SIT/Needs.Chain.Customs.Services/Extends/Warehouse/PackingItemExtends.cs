using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 装箱结果项扩展方法
    /// </summary>
    public static class PackingItemExtends
    {

        public static Layer.Data.Sqls.ScCustoms.PackingItems ToLinq(this Models.PackingItem entity)
        {
            return new Layer.Data.Sqls.ScCustoms.PackingItems
            {
                ID = entity.ID,
                PackingID = entity.PackingID,
                SortingID = entity.Sorting.ID,           
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate
            };
        }

    }
}
