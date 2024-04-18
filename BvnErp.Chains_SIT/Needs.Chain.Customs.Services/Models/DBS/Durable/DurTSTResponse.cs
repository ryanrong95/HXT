using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class DurTSTResponse
    {
        #region 属性
        public string ID { get; set; }
        public string TransactionName { get; set; }
        public string msgId { get; set; }
        public string orgId { get; set; }
        public int noOfTxs { get; set; }
       

        public decimal? totalTxnAmount { get; set; }
        public string ctry { get; set; }
        public string responseType { get; set; }
        public string customerReference { get; set; }
        public string msgRefId { get; set; }

        public string txnRefId { get; set; }
        public string bankReference { get; set; }
        public string txnType { get; set; }
        public string txnStatus { get; set; }
        public string txnRejectCode { get; set; }

        public string txnStatusDescription { get; set; }
        public string txnCcy { get; set; }
        public decimal? txnAmount { get; set; }
        public string debitAccountCcy { get; set; }
        public decimal? tranSettlementAmt { get; set; }

        public DateTime? tranSettlementDt { get; set; }
        public string fxContractRef1 { get; set; }
        public decimal? fxAmountUtilized1 { get; set; }
        public string fxContractRef2 { get; set; }
        public decimal? fxAmountUtilized2 { get; set; }

        public string senderName { get; set; }
        public string senderSwiftBic { get; set; }
        public string receivingName { get; set; }
        public string receivingAccountNo { get; set; }
        public string receivingBankCtryCode { get; set; }

        public string receivingSwiftBic { get; set; }
        public string receivingBankName { get; set; }
        public string addresses { get; set; }
        public string transactionReference { get; set; }
        public bool isAllMatched { get; set; }

        public Needs.Ccs.Services.Enums.Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Summary { get; set; }

        #endregion

        public DurTSTResponse()
        {
            this.Status = Needs.Ccs.Services.Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }

        public void Enter()
        {
            using (var reponsitory = new Layer.Data.Sqls.ForicDBSReponsitory())
            {
                reponsitory.Insert<Layer.Data.Sqls.foricDBS.TSTResponse>(new Layer.Data.Sqls.foricDBS.TSTResponse
                {
                    ID = ChainsGuid.NewGuidUp(),
                    TransactionName = this.TransactionName,
                    msgId = this.msgId,
                    orgId = this.orgId,
                    noOfTxs = this.noOfTxs,

                    totalTxnAmount = this.totalTxnAmount,
                    ctry = this.ctry,
                    responseType = this.responseType,
                    customerReference = this.customerReference,
                    msgRefId = this.msgRefId,

                    txnRefId = this.txnRefId,
                    bankReference = this.bankReference,
                    txnType = this.txnType,
                    txnStatus = this.txnStatus,
                    txnRejectCode = this.txnRejectCode,
                    txnStatusDescription = this.txnStatusDescription,

                    txnCcy = this.txnCcy,
                    txnAmount = this.txnAmount,
                    debitAccountCcy = this.debitAccountCcy,
                    tranSettlementAmt = this.tranSettlementAmt,
                    tranSettlementDt = this.tranSettlementDt,

                    fxContractRef1 = this.fxContractRef1,
                    fxAmountUtilized1 = this.fxAmountUtilized1,
                    fxContractRef2 = this.fxContractRef2,
                    fxAmountUtilized2 = this.fxAmountUtilized2,

                    senderName = this.senderName,
                    senderSwiftBic = this.senderSwiftBic,
                    receivingName = this.receivingName,
                    receivingAccountNo = this.receivingAccountNo,
                    receivingBankName = this.receivingBankName,
                    receivingSwiftBic = this.receivingSwiftBic,
                    receivingBankCtryCode = this.receivingBankCtryCode,

                    addresses = this.addresses,
                    isAllMatched = this.isAllMatched,
                    transactionReference = this.transactionReference,

                    Status = (int)this.Status,
                    CreateDate = this.CreateDate,
                    UpdateDate = this.UpdateDate,
                    Summary = this.Summary
                });
            }
        }
    }
}
