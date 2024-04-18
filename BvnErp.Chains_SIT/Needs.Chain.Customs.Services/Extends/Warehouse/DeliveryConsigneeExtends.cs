using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class DeliveryConsigneeExtends
    {
        public static Layer.Data.Sqls.ScCustoms.DeliveryConsignees ToLinq(this Models.DeliveryConsignee entity)
        {
            return new Layer.Data.Sqls.ScCustoms.DeliveryConsignees
            {
                ID = entity.ID,
                DeliveryNoticeID = entity.DeliveryNoticeID,
                Supplier = entity.Supplier,
                PickUpDate = entity.PickUpDate,
                Address=entity.Address,
                Contact = entity.Contact,
                Tel = entity.Tel,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary
            };
        }
    }
}
