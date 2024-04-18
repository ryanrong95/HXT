using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 付款申请扩展方法
    /// </summary>
    public static class PaymentNoticeItemExtends
    {
        public static Layer.Data.Sqls.ScCustoms.PaymentNoticeItems ToLinq(this Models.PaymentNoticeItem entity)
        {
            return new Layer.Data.Sqls.ScCustoms.PaymentNoticeItems
            {
                ID = entity.ID,
                PaymentNoticeID = entity.PaymentNoticeID,
                OrderID = entity.OrderID,
                FeeType = (int)entity.PayFeeType,
                Amount = entity.Amount,
                Currency = entity.Currency,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary,
            };
        }
    }
}
