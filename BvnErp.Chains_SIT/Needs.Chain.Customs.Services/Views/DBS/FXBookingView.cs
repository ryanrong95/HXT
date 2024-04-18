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
    public class FXBookingView : UniqueView<Models.DurFXResponse, ForicDBSReponsitory>
    {
        public FXBookingView()
        {
        }

        internal FXBookingView(ForicDBSReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<DurFXResponse> GetIQueryable()
        {

            var result = from pricing in this.Reponsitory.ReadTable<Layer.Data.Sqls.foricDBS.FXResponse>()
                         where  pricing.TransactionName== DBSConstConfig.DBSConstTransName.FXBooking  
                         && pricing.txnStatus == DBSConstConfig.DBSConstError.OKStatus
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
                             txnRefId = pricing.txnRefId,
                             IsACT = pricing.isACT,
                             IsTT = pricing.isTT,
                             CreateDate = pricing.CreateDate
                         };

            return result;
        }
    }
}
