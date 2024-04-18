using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views.Rolls
{
    internal class ClientAgreementsRoll : UniqueView<Models.ClientAgreement, ScCustomsReponsitory>
    {
        internal ClientAgreementsRoll()
        {
        }

        internal ClientAgreementsRoll(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<ClientAgreement> GetIQueryable()
        {
            var clientAgreementsView = new Origins.ClientAgreementsOrigin(this.Reponsitory);
            var feeSettlementsView = new Origins.ClientFeeSettlementsOrigin(this.Reponsitory);

            return from entity in clientAgreementsView
                   join feeSettlement in feeSettlementsView on entity.ID equals feeSettlement.AgreementID into feeSettlements
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
                       InvoiceType = entity.InvoiceType,
                       InvoiceTaxRate = entity.InvoiceTaxRate,
                       ClientFeeSettlementItems = feeSettlements,
                       AdminID = entity.AdminID,
                       Status = entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Summary = entity.Summary
                   };
        }
    }
}
