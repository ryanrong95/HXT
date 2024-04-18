using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 收款通知扩展方法
    /// </summary>
    public static class ReceiptNoticeExtends
    {
        public static Layer.Data.Sqls.ScCustoms.ReceiptNotices ToLinq(this Models.ReceiptNotice entity)
        {
            return new Layer.Data.Sqls.ScCustoms.ReceiptNotices
            {
                ID = entity.ID,
                ClientID = entity.Client?.ID,
                ClearAmount = entity.ClearAmount,
                AvailableAmount = entity.AvailableAmount,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary,
            };
        }
    }
}

