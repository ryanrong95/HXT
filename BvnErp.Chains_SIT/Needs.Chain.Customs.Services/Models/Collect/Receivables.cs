using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class Receivables:IUnique
    {
        public string ID { get; set; }
        public string FinanceReceiptID { get; set; }
        public string ClientID { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public decimal CNYAmount { get; set; }
        /// <summary>
        /// 是否匹配
        /// </summary>
        public Enums.MatchStatusEnums MatchStatus { get; set; }
        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        public Receivables()
        {
            this.MatchStatus = Enums.MatchStatusEnums.UnMatched;
            this.Status = Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.ReceivableCode);                
                reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.Receivables
                {
                    ID = this.ID,
                    FinanceReceiptID = this.FinanceReceiptID,
                    ClientID = this.ClientID,
                    Amount = this.Amount,
                    Currency = this.Currency,
                    CNYAmount = this.CNYAmount,
                    MatchStatus = (int)this.MatchStatus,
                    Status = (int)this.Status,
                    CreateDate = this.CreateDate,
                    UpdateDate = this.UpdateDate,
                    Summary = this.Summary
                });
                
            }
        }
    }
}
