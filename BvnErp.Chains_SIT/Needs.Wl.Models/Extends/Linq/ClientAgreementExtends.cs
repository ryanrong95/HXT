using System;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 
    /// </summary>
    public static class ClientAgreementExtends
    {
        public static Layer.Data.Sqls.ScCustoms.ClientAgreements ToLinq(this ClientAgreement entity)
        {
            return new Layer.Data.Sqls.ScCustoms.ClientAgreements
            {
                ID = entity.ID,
                ClientID = entity.ClientID,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                AgencyRate = entity.AgencyRate,
                MinAgencyFee = entity.MinAgencyFee,
                IsPrePayExchange = entity.IsPrePayExchange,
                IsLimitNinetyDays = entity.IsLimitNinetyDays,
                InvoiceType = (int)entity.InvoiceType,
                InvoiceTaxRate = entity.InvoiceTaxRate,
                AdminID = entity.AdminID,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                Summary = entity.Summary
            };
        }
    }
}