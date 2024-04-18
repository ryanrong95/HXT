using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    /// <summary>
    /// 客户的补充协议
    /// </summary>
    public class ClientAgreementsView : View<Models.ClientAgreement, ScCustomsReponsitory>
    {
        private string ClientID;

        public ClientAgreementsView(string clientID)
        {
            this.ClientID = clientID;
            this.AllowPaging = false;
        }

        protected override IQueryable<Models.ClientAgreement> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>()
                   join feeSettlement in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientFeeSettlements>()
                   on new { AgreementID = entity.ID, Status = (int)Needs.Wl.Models.Enums.Status.Normal } equals new { feeSettlement.AgreementID, feeSettlement.Status }
                   into feeSettlements
                   where entity.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                   && entity.ClientID == this.ClientID
                   select new Models.ClientAgreement
                   {
                       ID = entity.ID,
                       ClientID = entity.ClientID,
                       StartDate = entity.StartDate,
                       EndDate = entity.EndDate,
                       AgencyRate = entity.AgencyRate,
                       MinAgencyFee = entity.MinAgencyFee,
                       IsPrePayExchange = entity.IsPrePayExchange,
                       IsLimitNinetyDays = entity.IsLimitNinetyDays,
                       InvoiceType = (Enums.InvoiceType)entity.InvoiceType,
                       InvoiceTaxRate = entity.InvoiceTaxRate,
                       ClientFeeSettlementItems = new ClientFeeSettlementItems(feeSettlements.Select(item => new ClientFeeSettlement
                       {
                           ID = item.ID,
                           AgreementID = item.AgreementID,
                           FeeType = (Enums.FeeType)item.FeeType,
                           PeriodType = (Enums.PeriodType)item.PeriodType,
                           ExchangeRateType = (Enums.ExchangeRateType)item.ExchangeRateType,
                           ExchangeRateValue = item.ExchangeRateValue,
                           DaysLimit = item.DaysLimit,
                           MonthlyDay = item.MonthlyDay,
                           UpperLimit = item.UpperLimit,
                           Status = item.Status,
                           CreateDate = item.CreateDate,
                           UpdateDate = item.UpdateDate,
                           Summary = item.Summary
                       })),
                       AdminID = entity.AdminID,
                       Status = entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Summary = entity.Summary
                   };
        }
    }
}