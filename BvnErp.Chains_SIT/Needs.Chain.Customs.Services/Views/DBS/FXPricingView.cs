using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class FXPricingView : UniqueView<Models.DurFXResponse, ForicDBSReponsitory>
    {
        public FXPricingView()
        {
        }

        internal FXPricingView(ForicDBSReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<DurFXResponse> GetIQueryable()
        {

            var result = from pricing in this.Reponsitory.ReadTable<Layer.Data.Sqls.foricDBS.FXResponse>()
                         where pricing.validTill >= DateTime.Now && pricing.TransactionName== DBSConstConfig.DBSConstTransName.FXPricing
                         && pricing.isLocked == false 
                         select new Models.DurFXResponse
                         {
                             ID = pricing.ID,
                             uid = pricing.uid,
                             validTill = pricing.validTill,
                             txnAmount = pricing.txnAmount,
                             txnCcy = pricing.txnCcy,
                             rate = pricing.rate,
                             contraCcy = pricing.contraCcy,
                             contraAmount = pricing.contraAmount,
                             valueDate = pricing.valueDate,
                             CreateDate = pricing.CreateDate
                         };

            return result;
        }
    }
}
