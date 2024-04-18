using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views.Origins
{
    internal class ClientAgreementsOrigin : UniqueView<Models.ClientAgreement, ScCustomsReponsitory>
    {
        internal ClientAgreementsOrigin()
        {
        }

        internal ClientAgreementsOrigin(ScCustomsReponsitory reponsitory) :base(reponsitory)
        {
        }

        protected override IQueryable<ClientAgreement> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>()
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
                       AdminID = entity.AdminID,
                       Status = (Enums.Status)entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Summary = entity.Summary
                   };
        }
    }
}
