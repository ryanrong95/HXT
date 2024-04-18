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
    public static class DeliveryNoticeExtends
    {
        public static Layer.Data.Sqls.ScCustoms.DeliveryNotices ToLinq(this Models.DeliveryNotice entity)
        {

            return new Layer.Data.Sqls.ScCustoms.DeliveryNotices
            {
                ID = entity.ID,
                AdminID = entity.Admin.ID,
                OrderID = entity.Order.ID,
                DeliverNoticeStatus=(int)entity.DeliveryNoticeStatus,
                Status =(int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary
            };

        }

    }
}
