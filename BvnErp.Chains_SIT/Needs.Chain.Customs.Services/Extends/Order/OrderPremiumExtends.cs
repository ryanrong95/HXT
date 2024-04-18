using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 代理订单附加费用扩展方法
    /// </summary>
    public static class OrderPremiumExtends
    {
        public static Layer.Data.Sqls.ScCustoms.OrderPremiums ToLinq(this Models.OrderPremium entity)
        {
            return new Layer.Data.Sqls.ScCustoms.OrderPremiums
            {
                ID = entity.ID,
                OrderID = entity.OrderID,
                OrderItemID = entity.OrderItemID,
                AdminID = entity.Admin.ID,
                Type = (int)entity.Type,
                Name = entity.Name,
                Count = entity.Count,
                UnitPrice = entity.UnitPrice,
                Currency = entity.Currency,
                Rate = entity.Rate,
                StandardID = entity.StandardID,
                StandardPrice = entity.StandardPrice,
                StandardCurrency = entity.StandardCurrency,
                StandardRemark = entity.StandardRemark,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                Summary = entity.Summary
            };
        }
    }
}
