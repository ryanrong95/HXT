using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 订单产品/产地/品牌变更通知
    /// </summary>
    public static class OrderItemChangeNoticeExtends
    {
        public static Layer.Data.Sqls.ScCustoms.OrderItemChangeNotices ToLinq(this Models.OrderItemChangeNotice entity)
        {
            return new Layer.Data.Sqls.ScCustoms.OrderItemChangeNotices
            {
                ID = entity.ID,
                OrderItemID = entity.OrderItemID,
                AdminID = entity.Sorter?.ID,
                Type = (int)entity.Type,
                OldValue = entity.OldValue,
                NewValue = entity.NewValue,
                IsSplited = entity.IsSplited,
                ProcessStatus = (int)entity.ProcessState,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                TriggerSource = (int)entity.TriggerSource,
            };
        }

     
    }

   

}


