using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class DurAccountFlow:IUnique
    {
        public string ID { get; set; }
        public string msgId { get; set; }
        public string accountNo { get; set; }
        public string accountCcy { get; set; }
        public string availableBal { get; set; }
        public string initiatingPartyName { get; set; }
        public string drCrInd { get; set; }
        public string txnCode { get; set; }
        public string txnDesc { get; set; }
        public DateTime txnDate { get; set; }
        public DateTime valueDate { get; set; }
        public string txnCcy { get; set; }
        public decimal txnAmount { get; set; }       
        public Needs.Ccs.Services.Enums.Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Summary { get; set; }

        public DurAccountFlow()
        {
            this.Status = Needs.Ccs.Services.Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }

        public void Enter()
        {
            using (var reponsitory = new Layer.Data.Sqls.ForicDBSReponsitory())
            {
                reponsitory.Insert<Layer.Data.Sqls.foricDBS.AccountFlow>(new Layer.Data.Sqls.foricDBS.AccountFlow
                {
                    ID = ChainsGuid.NewGuidUp(),
                    msgId = this.msgId,
                    accountNo = this.accountNo,
                    accountCcy = this.accountCcy,
                    availableBal = this.availableBal,
                    initiatingPartyName = this.initiatingPartyName,
                    drCrInd = this.drCrInd,
                    txnCode = this.txnCode,
                    txnDesc = this.txnDesc,
                    txnDate = this.txnDate,
                    valueDate = this.valueDate,
                    txnCcy = this.txnCcy,
                    txnAmount = this.txnAmount,                   
                    Status = (int)this.Status,
                    CreateDate = this.CreateDate,
                    UpdateDate = this.UpdateDate,
                    Summary = this.Summary
                });
            }
        }
    }
}
