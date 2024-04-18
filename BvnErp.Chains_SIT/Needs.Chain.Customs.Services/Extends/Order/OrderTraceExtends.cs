using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 代理订单轨迹扩展方法
    /// </summary>
    public static class OrderTraceExtends
    {
        public static Layer.Data.Sqls.ScCustoms.OrderTraces ToLinq(this Models.OrderTrace entity)
        {
            return new Layer.Data.Sqls.ScCustoms.OrderTraces
            {
                ID = entity.ID,
                OrderID = entity.OrderID,
                AdminID = entity.Admin?.ID,
                UserID =  entity.User?.ID,
                Step = (int)entity.Step,
                CreateDate = entity.CreateDate,
                Summary = entity.Summary
            };
        }
    }
}
