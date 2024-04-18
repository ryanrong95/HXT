using System;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 
    /// </summary>
    public static class ClientFeeSettlementExtends
    {
        public static Layer.Data.Sqls.ScCustoms.ClientFeeSettlements ToLinq(this ClientFeeSettlement entity)
        {
            return new Layer.Data.Sqls.ScCustoms.ClientFeeSettlements
            {
                ID = entity.ID,
                AgreementID = entity.AgreementID,
                FeeType = (int)entity.FeeType,
                PeriodType = (int)entity.PeriodType,
                ExchangeRateType = (int)entity.ExchangeRateType,
                ExchangeRateValue = entity.ExchangeRateValue,
                DaysLimit = entity.DaysLimit,
                MonthlyDay = entity.MonthlyDay,
                UpperLimit = entity.UpperLimit,
                AdminID = entity.AdminID,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary
            };
        }
    }
}