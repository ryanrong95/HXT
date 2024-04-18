using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ClientAgreementsView : UniqueView<Models.ClientAgreement, ScCustomsReponsitory>
    {
        public ClientAgreementsView()
        {
        }

        public ClientAgreementsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ClientAgreement> GetIQueryable()
        {
            var feeSettlementView = new ClientFeeSettlementView(this.Reponsitory);

            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>()
                   join feeSettlement in feeSettlementView on entity.ID equals feeSettlement.AgreementID into feeSettlements
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
                       IsTen = entity.IsTen,
                       InvoiceType = (Enums.InvoiceType)entity.InvoiceType,
                       InvoiceTaxRate = entity.InvoiceTaxRate,
                       ClientFeeSettlementItems = feeSettlements,
                       AdminID = entity.AdminID,
                       Status = (Enums.Status)entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Summary = entity.Summary
                   };
        }
    }
}
