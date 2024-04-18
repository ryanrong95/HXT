using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Origins
{
    public class FinanceReceiptsOrigin : UniqueView<Models.FinanceReceipt, ScCustomsReponsitory>
    {
        internal FinanceReceiptsOrigin()
        {
        }

        internal FinanceReceiptsOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<FinanceReceipt> GetIQueryable()
        {
            return from financeReceipt in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceReceipts>()
                   select new Models.FinanceReceipt
                   {
                       ID = financeReceipt.ID,
                       SeqNo = financeReceipt.SeqNo,
                       Payer = financeReceipt.Payer,
                       FeeType = (Enums.FinanceFeeType)financeReceipt.FeeType,
                       ReceiptType = (Enums.PaymentType)financeReceipt.ReceiptType,
                       ReceiptDate = financeReceipt.ReceiptDate,
                       Currency = financeReceipt.Currency,
                       Rate = financeReceipt.Rate,
                       Amount = financeReceipt.Amount,
                       FinanceVaultID = financeReceipt.FinanceVaultID,
                       FinanceAccountID = financeReceipt.FinanceAccountID,
                       AdminID = financeReceipt.AdminID,
                       Status = (Enums.Status)financeReceipt.Status,
                       CreateDate = financeReceipt.CreateDate,
                       UpdateDate = financeReceipt.UpdateDate,
                       Summary = financeReceipt.Summary,
                   };
        }
    }
}
