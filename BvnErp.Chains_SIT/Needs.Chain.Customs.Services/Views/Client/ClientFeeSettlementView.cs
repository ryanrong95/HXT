using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ClientFeeSettlementView : UniqueView<Models.ClientFeeSettlement, ScCustomsReponsitory>
    {
        public ClientFeeSettlementView()
        {
        }

        internal ClientFeeSettlementView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ClientFeeSettlement> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientFeeSettlements>()
                   select new Models.ClientFeeSettlement
                   {
                       ID = entity.ID,
                       AgreementID = entity.AgreementID,
                       FeeType = (Enums.FeeType)entity.FeeType,
                       PeriodType = (Enums.PeriodType)entity.PeriodType,
                       ExchangeRateType = (Enums.ExchangeRateType)entity.ExchangeRateType,
                       ExchangeRateValue = entity.ExchangeRateValue,
                       DaysLimit = entity.DaysLimit,
                       MonthlyDay = entity.MonthlyDay,
                       UpperLimit = entity.UpperLimit,
                       Status = (Enums.Status)entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Summary = entity.Summary,
                       AdminID = entity.AdminID
                   };
        }
    }
}