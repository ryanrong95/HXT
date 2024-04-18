using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class StoreReceiptHandler
    {
        private string FinanceReceiptId { get; set; } = string.Empty;

        private decimal IncreaceAmount { get; set; }

        public decimal NewClearAmount { get; set; }

        public StoreReceiptHandler(string financeReceiptId, decimal increaceAmount)
        {
            this.FinanceReceiptId = financeReceiptId;
            this.IncreaceAmount = increaceAmount;
        }

        public void Execute()
        {
            var receiptNotice = new Needs.Ccs.Services.Views.ReceiptNoticesView()[this.FinanceReceiptId];

            decimal oldClearAmount = receiptNotice.ClearAmount;

            this.NewClearAmount = oldClearAmount + this.IncreaceAmount;

            //更新收款通知的已明确金额
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ReceiptNotices>(new
                {
                    ClearAmount = this.NewClearAmount,
                }, item => item.ID == receiptNotice.ID);
            }
        }


    }
}
