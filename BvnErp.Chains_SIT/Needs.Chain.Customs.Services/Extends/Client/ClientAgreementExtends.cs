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
    public static class ClientAgreementExtends
    {
        public static Layer.Data.Sqls.ScCustoms.ClientAgreements ToLinq(this Models.ClientAgreement entity)
        {
            return new Layer.Data.Sqls.ScCustoms.ClientAgreements
            {
                ID = entity.ID,
                ClientID = entity.ClientID,
                AgreementCode = entity.AgreementCode,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                PreAgency = entity.PreAgency,
                AgencyRate = entity.AgencyRate,
                MinAgencyFee = entity.MinAgencyFee,
                IsPrePayExchange = entity.IsPrePayExchange,
                IsLimitNinetyDays = entity.IsLimitNinetyDays,
                IsTen = (int)entity.IsTen,
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