using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 代理订单接货信息扩展方法
    /// </summary>
    public static class OrderConsigneeExtends
    {
        public static Layer.Data.Sqls.ScCustoms.OrderConsignees ToLinq(this Models.OrderConsignee entity)
        {
            return new Layer.Data.Sqls.ScCustoms.OrderConsignees
            {
                ID = entity.ID,
                OrderID = entity.OrderID,
                ClientSupplierID = entity.ClientSupplier.ID,
                Type = (int)entity.Type,
                Contact = entity.Contact,
                Mobile = entity.Mobile,
                Tel = entity.Tel,
                Address = entity.Address,
                PickUpTime = entity.PickUpTime,
                WayBillNo = entity.WayBillNo,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                Summary = entity.Summary,
                CarrierID = entity.CarrierID,
            };
        }
    }
}
