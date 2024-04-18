using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 装箱结果扩展方法
    /// </summary>
    public static class PackingExtends
    {
        public static Layer.Data.Sqls.ScCustoms.Packings ToLinq(this Models.Packing entity)
        {
            return new Layer.Data.Sqls.ScCustoms.Packings
            {
                ID = entity.ID,
                OrderID = entity.OrderID,
                AdminID = entity.AdminID,
                BoxIndex = entity.BoxIndex,
                PackingDate = entity.PackingDate,
                Weight = entity.Weight,
                WrapType = entity.WrapType,
                PackingStatus = (int)entity.PackingStatus,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary
            };
        }
    }
}
