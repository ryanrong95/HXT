using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class DurTSTRequest:IUnique
    {
        #region
        public string ID { get; set; }
        public string TransactionName { get; set; }
        public string msgId { get; set; }
        public string orgId { get; set; }
        public DateTime timeStamp { get; set; }

        public string ctry { get; set; }
        public int noOfTxs { get; set; }
        public decimal totalTxnAmount { get; set; }
        public string customerReference { get; set; }
        public string txnType { get; set; }

        public string txnDate { get; set; }
        public string txnCcy { get; set; }
        public decimal txnAmount { get; set; }
        public string debitAccountCcy { get; set; }
        public decimal debitAccountAmount { get; set; }

        public string fxContractRef1 { get; set; }
        public decimal fxAmountUtilized1 { get; set; }
        public string fxContractRef2 { get; set; }
        public decimal fxAmountUtilized2 { get; set; }
        public string senderName { get; set; }

        public string senderAccountNo { get; set; }
        public string senderBankCtryCode { get; set; }
        public string senderSwiftBic { get; set; }
        public string receivingName { get; set; }
        public string receivingAccountNo { get; set; }

        public string receivingBankCtryCode { get; set; }
        public string receivingClearingCode { get; set; }
        public string receivingSwiftBic { get; set; }
        public string receivingRoutingCode { get; set; }
        public string receivingBankName { get; set; }

        public string receivingBankAddress { get; set; }
        public string addresses { get; set; }
        public string countrySpecific { get; set; }
        public string intermediarySwiftBic { get; set; }
        public string mode { get; set; }

        public string emails { get; set; }
        public string phoneNumbers { get; set; }
        public string paymentDetails { get; set; }
        public string clientReferences { get; set; }
        public string invoices { get; set; }

        public string specificPaymentPurpose { get; set; }
        public string taxFreeGoodsRelated { get; set; }
        public string paymentNature { get; set; }
        public string referenceNo { get; set; }
        public string bOPCode1PaymentCategory { get; set; }

        public string bOPCode1SeriesCode { get; set; }
        public Needs.Ccs.Services.Enums.Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Summary { get; set; }

        public string chargeBearer { get; set; }
        #endregion

        public DurTSTRequest()
        {
            this.Status = Needs.Ccs.Services.Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }

        public void Enter()
        {
            using (var reponsitory = new Layer.Data.Sqls.ForicDBSReponsitory())
            {
                reponsitory.Insert<Layer.Data.Sqls.foricDBS.TSTRequest>(new Layer.Data.Sqls.foricDBS.TSTRequest
                {
                    ID = ChainsGuid.NewGuidUp(),
                    TransactionName = this.TransactionName,
                    msgId = this.msgId,
                    orgId = this.orgId,
                    timeStamp = this.timeStamp,

                    ctry = this.ctry,
                    noOfTxs = this.noOfTxs,
                    totalTxnAmount = this.totalTxnAmount,
                    customerReference = this.customerReference,
                    txnType = this.txnType,

                    txnDate = this.txnDate,
                    txnCcy = this.txnCcy,
                    txnAmount = this.txnAmount,
                    debitAccountCcy = this.debitAccountCcy,
                    debitAccountAmount = this.debitAccountAmount,

                    fxContractRef1 = this.fxContractRef1,
                    fxAmountUtilized1 = this.fxAmountUtilized1,
                    fxContractRef2 = this.fxContractRef2,
                    fxAmountUtilized2 = this.fxAmountUtilized2,

                    senderName = this.senderName,
                    senderAccountNo = this.senderAccountNo,
                    senderBankCtryCode = this.senderBankCtryCode,
                    senderSwiftBic = this.senderSwiftBic,

                    receivingName = this.receivingName,
                    receivingAccountNo = this.receivingAccountNo,
                    receivingBankName = this.receivingBankName,
                    receivingBankAddress = this.receivingBankAddress,
                    receivingBankCtryCode = this.receivingBankCtryCode,
                    receivingClearingCode = this.receivingClearingCode,
                    receivingSwiftBic = this.receivingSwiftBic,
                    receivingRoutingCode = this.receivingRoutingCode,

                    addresses = this.addresses,
                    countrySpecific = this.countrySpecific,
                    intermediarySwiftBic = this.intermediarySwiftBic,
                    mode = this.mode,
                    emails = this.emails,

                    phoneNumbers = this.phoneNumbers,
                    paymentDetails = this.paymentDetails,
                    clientReferences = this.clientReferences,
                    invoices = this.invoices,
                    specificPaymentPurpose = this.specificPaymentPurpose,

                    taxFreeGoodsRelated = this.taxFreeGoodsRelated,
                    paymentNature = this.paymentNature,
                    referenceNo = this.referenceNo,
                    bOPCode1SeriesCode = this.bOPCode1SeriesCode,
                    bOPCode1PaymentCategory = this.bOPCode1PaymentCategory,

                   
                    Status = (int)this.Status,
                    CreateDate = this.CreateDate,
                    UpdateDate = this.UpdateDate,
                    Summary = this.Summary,

                    chargeBearer = this.chargeBearer

                });
            }
        }
    }
}
