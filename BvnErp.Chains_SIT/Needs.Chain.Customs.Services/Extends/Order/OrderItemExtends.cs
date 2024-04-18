using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 代理订单项扩展方法
    /// </summary>
    public static class OrderItemExtends
    {
        public static Layer.Data.Sqls.ScCustoms.OrderItems ToLinq(this Models.OrderItem entity)
        {
            return new Layer.Data.Sqls.ScCustoms.OrderItems
            {
                ID = entity.ID,
                OrderID = entity.OrderID,
                Name=entity.Name,
                Model=entity.Model,
                Manufacturer=entity.Manufacturer,
                Batch=entity.Batch,
                Origin = entity.Origin,
                Quantity = entity.Quantity,
                DeclaredQuantity = entity.DeclaredQuantity,
                Unit = entity.Unit,
                UnitPrice = entity.UnitPrice,
                TotalPrice = entity.TotalPrice,
                GrossWeight = entity.GrossWeight,
                IsSampllingCheck = entity.IsSampllingCheck,
                ClassifyStatus = (int)entity.ClassifyStatus,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                Summary = entity.Summary,
                ProductUniqueCode = entity.ProductUniqueCode,
            };
        }
    }

    public static class OrderItemChangeLogExtends
    {
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="order"></param>
        /// <param name="summary"></param>
        public static void Log(this Models.OrderItem entity, Enums.OrderItemChangeType type, string summary)
        {
            OrderItemChangeLog log = new OrderItemChangeLog();
            log.OrderItemID = entity.ID;
            log.OrderID = entity.OrderID;
            log.Type = type;
            log.Admin = entity.SorterAdmin;
            log.Summary = summary;
            log.Enter();
        }
    }
}
