using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 
    /// </summary>
    public static class PayExchangeApplieItemExtends
    {
        public static Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems ToLinq(this Models.PayExchangeApplyItem entity)
        {
            return new Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems
            {
                ID = entity.ID,
                OrderID = entity.OrderID,
                Amount = entity.Amount,
                PayExchangeApplyID = entity.PayExchangeApplyID,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                Summary = entity.Summary
            };
        }
    }
}