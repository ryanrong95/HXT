using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 代理订单税率扩展方法
    /// </summary>
    public static class OrderItemTaxExtends
    {
        public static Layer.Data.Sqls.ScCustoms.OrderItemTaxes ToLinq(this Models.OrderItemTax entity)
        {
            return new Layer.Data.Sqls.ScCustoms.OrderItemTaxes
            {
                ID = entity.ID,
                OrderItemID = entity.OrderItemID,
                Type = (int)entity.Type,
                Rate = entity.Rate,
                ReceiptRate = entity.ReceiptRate,
                Value = entity.Value,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                Summary = entity.Summary
            };
        }
    }
}
