using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 代理订单交货信息扩展方法
    /// </summary>
    public static class OrderConsignorExtends
    {
        public static Layer.Data.Sqls.ScCustoms.OrderConsignors ToLinq(this Models.OrderConsignor entity)
        {
            return new Layer.Data.Sqls.ScCustoms.OrderConsignors
            {
                ID = entity.ID,
                OrderID = entity.OrderID,
                Type = (int)entity.Type,
                Name = entity.Name,
                Contact = entity.Contact,
                Mobile = entity.Mobile,
                Tel = entity.Tel,
                Address = entity.Address,
                IDType = entity.IDType,
                IDNumber = entity.IDNumber,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                Summary = entity.Summary
            };
        }
    }
}
