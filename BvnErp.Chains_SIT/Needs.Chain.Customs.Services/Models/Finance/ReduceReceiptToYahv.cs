using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Payments;

namespace Needs.Ccs.Services.Models
{
    public class ReduceReceiptToYahv
    {
        private Admin Admin { get; set; }

        private string ReceivableID { get; set; } = string.Empty;

        private decimal ReduceNumber { get; set; }

        private string ReceivedID { get; set; }

        public ReduceReceiptToYahv(Admin admin, string receivableID, decimal reduceNumber)
        {
            this.Admin = admin;
            this.ReceivableID = receivableID;
            this.ReduceNumber = reduceNumber;
        }

        public ReduceReceiptToYahv(Admin admin, string receivedID)
        {
            this.Admin = admin;
            this.ReceivedID = receivedID;
        }

        /// <summary>
        /// 减免
        /// </summary>
        public void ExecuteReduce()
        {
            try
            {
                Yahv.Payments.PaymentManager.Erp(this.Admin.ErmAdminID).Received.
                                                            For(this.ReceivableID).
                                                            Reduction(Yahv.Underly.Currency.CNY, this.ReduceNumber);
            }
            catch (Exception ex)
            {
                ex.CcsLog("减免收款到Yahv发生异常(ReduceReceiptToYahv.ExecuteReduce)");
                throw ex;
            }
        }

        public void CancelReduce()
        {
            try
            {
                PaymentManager.Erp(this.Admin.ErmAdminID).Received.
                                ReductionCancel(this.ReceivedID);
            }
            catch (Exception ex)
            {
                ex.CcsLog("取消减免收款到Yahv发生异常(ReduceReceiptToYahv.CancelReduce)");
                throw ex;
            }
        }

    }
}
