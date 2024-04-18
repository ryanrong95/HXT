using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class DurFXResponse : IUnique
    {
        public string ID { get; set; }
        public string TransactionName { get; set; }
        public string msgId { get; set; }
        public string orgId { get; set; }
        public string bookingStatus { get; set; }
        public string message { get; set; }
        public string txnRefId { get; set; }
        public DateTime? txnDateTime { get; set; }
        public string uid { get; set; }
        public string custProfile { get; set; }
        public DateTime? validTill { get; set; }
        public string ccyPair { get; set; }
        public decimal? txnAmount { get; set; }
        public string txnCcy { get; set; }
        public string contraCcy { get; set; }
        public decimal? contraAmount { get; set; }
        public string dealtSide { get; set; }
        public decimal? rate { get; set; }
        public string tenor { get; set; }
        public string valueDate { get; set; }
        public string dealType { get; set; }
        public string clientTxnsId { get; set; }
        public string txnStatus { get; set; }
        public string traceId { get; set; }
        public string txnRejectCode { get; set; }
        public string txnStatusDescription { get; set; }
        public bool? IsLocked { get; set; }
        public bool? IsACT { get; set; }
        public bool? IsTT { get; set; }
        public Needs.Ccs.Services.Enums.Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Summary { get; set; }

        public DurFXResponse()
        {
            this.Status = Needs.Ccs.Services.Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }

        public void Enter()
        {
            using (var reponsitory = new Layer.Data.Sqls.ForicDBSReponsitory())
            {
                reponsitory.Insert<Layer.Data.Sqls.foricDBS.FXResponse>(new Layer.Data.Sqls.foricDBS.FXResponse
                {
                    ID = ChainsGuid.NewGuidUp(),
                    TransactionName = this.TransactionName,
                    msgId = this.msgId,
                    orgId = this.orgId,
                    bookingStatus = this.bookingStatus,
                    message = this.message,
                    txnRefId = this.txnRefId,
                    txnDateTime = this.txnDateTime,
                    uid = this.uid,
                    custProfile = this.custProfile,
                    validTill = this.validTill,
                    ccyPair = this.ccyPair,
                    txnCcy = this.txnCcy,
                    txnAmount = this.txnAmount,
                    contraCcy = this.contraCcy,
                    contraAmount = this.contraAmount,
                    dealtSide = this.dealtSide,
                    rate = this.rate,
                    tenor = this.tenor,
                    valueDate = this.valueDate,
                    dealType = this.dealType,
                    clientTxnsId = this.clientTxnsId,
                    txnStatus = this.txnStatus,
                    traceId = this.traceId,
                    txnRejectCode = this.txnRejectCode,
                    txnStatusDescription = this.txnStatusDescription,                    
                    isLocked = this.IsLocked,
                    Status = (int)this.Status,
                    CreateDate = this.CreateDate,
                    UpdateDate = this.UpdateDate,
                    Summary = this.Summary
                });
            }
        }
    }
}
