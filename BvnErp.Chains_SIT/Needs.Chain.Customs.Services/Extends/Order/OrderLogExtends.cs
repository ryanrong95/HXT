using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 代理订单日志扩展方法
    /// </summary>
    public static class OrderLogExtends
    {
        public static Layer.Data.Sqls.ScCustoms.OrderLogs ToLinq(this Models.OrderLog entity)
        {
            return new Layer.Data.Sqls.ScCustoms.OrderLogs
            {
                ID = entity.ID,
                OrderID = entity.OrderID,
                OrderItemID = entity.OrderItemID,
                AdminID = entity.Admin?.ID,
                UserID = entity.User?.ID,
                OrderStatus = (int)entity.OrderStatus,
                CreateDate = entity.CreateDate,
                Summary = entity.Summary
            };
        }
    }
}
