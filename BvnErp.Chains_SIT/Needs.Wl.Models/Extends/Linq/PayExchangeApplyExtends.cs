using System;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 付汇申请扩展
    /// </summary>
    public static partial class PayExchangeApplyExtends
    {
        public static Layer.Data.Sqls.ScCustoms.PayExchangeApplies ToLinq(this Models.PayExchangeApply entity)
        {
            return new Layer.Data.Sqls.ScCustoms.PayExchangeApplies
            {
                ID = entity.ID,
                AdminID = entity.Admin?.ID,
                UserID = entity.User?.ID,
                ClientID = entity.ClientID,
                SupplierName = entity.SupplierName,
                SupplierEnglishName = entity.SupplierEnglishName,
                SupplierAddress = entity.SupplierAddress,
                BankAccount = entity.BankAccount,
                BankAddress = entity.BankAddress,
                BankName = entity.BankName,
                SwiftCode = entity.SwiftCode,
                ExchangeRateType = (int)entity.ExchangeRateType,
                Currency = entity.Currency,
                ExchangeRate = entity.ExchangeRate,
                PaymentType = (int)entity.PaymentType,
                ExpectPayDate = entity.ExpectPayDate,
                SettlemenDate = entity.SettlemenDate,
                OtherInfo = entity.OtherInfo,
                PayExchangeApplyStatus = (int)entity.PayExchangeApplyStatus,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                Summary = entity.Summary
            };
        }
    }
}