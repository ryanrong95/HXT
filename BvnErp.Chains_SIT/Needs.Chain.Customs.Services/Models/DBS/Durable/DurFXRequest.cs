using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class DurFXRequest
    {
        public string ID { get; set; }
        public string TransactionName { get; set; }
        public string msgId { get; set; }
        public string orgId { get; set; }
        public string ccyPair { get; set; }
        public string dealtSide { get; set; }
        public decimal? txnAmount { get; set; }
        public string txnCcy { get; set; }
        public string tenor { get; set; }
        public string clientTxnsId { get; set; }
        public string uid { get; set; }
        public string txnPurpose { get; set; }
        public string underlyingCode { get; set; }
        public Needs.Ccs.Services.Enums.Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Summary { get; set; }

        public DurFXRequest()
        {
            this.Status = Needs.Ccs.Services.Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }

        public void Enter()
        {
            using (var reponsitory = new Layer.Data.Sqls.ForicDBSReponsitory())
            {
                reponsitory.Insert<Layer.Data.Sqls.foricDBS.FXRequest>(new Layer.Data.Sqls.foricDBS.FXRequest
                {
                    ID = ChainsGuid.NewGuidUp(),
                    TransactionName = this.TransactionName,
                    msgId = this.msgId,
                    orgId = this.orgId,
                    ccyPair = this.ccyPair,
                    dealtSide = this.dealtSide,
                    txnAmount = this.txnAmount,
                    txnCcy = this.txnCcy,
                    tenor = this.tenor,
                    clientTxnsId = this.clientTxnsId,
                    uid = this.uid,
                    txnPurpose = this.txnPurpose,
                    underlyingCode = this.underlyingCode,
                    Status = (int)this.Status,
                    CreateDate = this.CreateDate,
                    UpdateDate = this.UpdateDate,
                    Summary = this.Summary
                });
            }
        }
    }
}
