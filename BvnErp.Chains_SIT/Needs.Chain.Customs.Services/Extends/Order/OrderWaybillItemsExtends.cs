using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 订单运单
    /// </summary>
    public static class OrderWaybillItemExtends
    {
        public static Layer.Data.Sqls.ScCustoms.OrderWaybillItems ToLinq(this Models.OrderWaybillItem entity)
        {
            return new Layer.Data.Sqls.ScCustoms.OrderWaybillItems
            {
                ID = entity.ID,
                OrderWaybillID = entity.OrderWaybillID,
                SortingID = entity.Sorting.ID
            };
        }
    }
}