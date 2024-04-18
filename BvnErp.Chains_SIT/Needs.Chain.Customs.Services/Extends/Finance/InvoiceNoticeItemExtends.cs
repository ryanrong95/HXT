using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 开票通知明细扩展方法
    /// </summary>
    public static class InvoiceNoticeItemExtends
    {
        public static Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems ToLinq(this Models.InvoiceNoticeItem entity)
        {
            return new Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems
            {
                ID = entity.ID,
                InvoiceNoticeID = entity.InvoiceNoticeID,
                OrderID = entity.OrderID,
                OrderItemID = entity.OrderItem?.ID,
                UnitPrice = entity.UnitPrice,
                Amount = entity.Amount,
                Difference = entity.Difference,
                InvoiceCode = entity.InvoiceCode,
                InvoiceNo = entity.InvoiceNo,
                InvoiceDate = entity.InvoiceDate,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary,
            };
        }
    }
}
