using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 代理订单付汇供应商扩展方法
    /// </summary>
    public static class OrderPayExchangeSupplierExtends
    {
        public static Layer.Data.Sqls.ScCustoms.OrderPayExchangeSuppliers ToLinq(this Models.OrderPayExchangeSupplier entity)
        {
            return new Layer.Data.Sqls.ScCustoms.OrderPayExchangeSuppliers
            {
                ID = entity.ID,
                OrderID = entity.OrderID,
                ClientSupplierID = entity.ClientSupplier.ID,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                Summary = entity.Summary
            };
        }
    }
}
