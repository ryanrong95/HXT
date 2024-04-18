using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class TTRequestView : UniqueView<Models.DurTSTRequest, ForicDBSReponsitory>
    {
        public TTRequestView()
        {
        }

        internal TTRequestView(ForicDBSReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Models.DurTSTRequest> GetIQueryable()
        {

            var result = from tt in this.Reponsitory.ReadTable<Layer.Data.Sqls.foricDBS.TSTRequest>()   
                         where tt.TransactionName == Models.DBSConstConfig.DBSConstTransName.TT
                         select new Models.DurTSTRequest
                         {
                             ID = tt.ID,
                             TransactionName = tt.TransactionName,
                             msgId = tt.msgId,
                             totalTxnAmount = tt.totalTxnAmount,
                             txnDate = tt.txnDate,
                             txnCcy = tt.txnCcy,
                             txnAmount = tt.txnAmount,
                             debitAccountCcy = tt.debitAccountCcy,
                             fxContractRef1 = tt.fxContractRef1,
                             fxAmountUtilized1 = tt.fxAmountUtilized1==null?0:tt.fxAmountUtilized1.Value,
                             senderName = tt.senderName,
                             senderAccountNo = tt.senderAccountNo,
                             senderBankCtryCode = tt.senderBankCtryCode,
                             senderSwiftBic = tt.senderSwiftBic,
                             receivingName = tt.receivingName,
                             receivingAccountNo = tt.receivingAccountNo,
                             receivingSwiftBic = tt.receivingSwiftBic,
                             receivingBankCtryCode = tt.receivingBankCtryCode,
                             chargeBearer = tt.chargeBearer,
                             CreateDate = tt.CreateDate
                         };

            return result;
        }
    }
}
