using System;

namespace Needs.Wl.Models
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
                CreateDate = DateTime.Now,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary,
                ApplyStatus = (int)entity.ApplyStatus,
            };
        }
    }
}