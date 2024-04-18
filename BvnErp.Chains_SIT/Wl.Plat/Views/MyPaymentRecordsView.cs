using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Wl.User.Plat.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Needs.Wl.User.Plat.Views
{
    public class MyPaymentRecordsView : View<Needs.Wl.Client.Services.Models.ClientPaymentRecord, ScCustomsReponsitory>
    {
        IPlatUser User;

        public MyPaymentRecordsView(IPlatUser user)
        {
            this.User = user;
        }

        protected override IQueryable<Needs.Wl.Client.Services.Models.ClientPaymentRecord> GetIQueryable()
        {
            var query = from receiptNotice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ReceiptNotices>()
                        join financeReceipt in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceReceipts>() on receiptNotice.ID equals financeReceipt.ID
                        join financeAccounts in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceAccounts>() on financeReceipt.FinanceAccountID equals financeAccounts.ID
                        where receiptNotice.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                               && receiptNotice.ClientID == this.User.ClientID
                        orderby financeReceipt.ReceiptDate descending
                        select new Needs.Wl.Client.Services.Models.ClientPaymentRecord
                        {
                            ID = receiptNotice.ID,
                            SeqNo = financeReceipt.SeqNo,
                            ClearAmount = receiptNotice.ClearAmount,
                            Amount = financeReceipt.Amount,
                            ClientID = receiptNotice.ClientID,
                            Currency = financeReceipt.Currency,
                            ReceiptDate = financeReceipt.ReceiptDate,
                            AccountName = financeAccounts.AccountName,
                            AccountBankName = financeAccounts.BankName,
                            AccountBankAccount = financeAccounts.BankAccount
                        };

            return query;
        }

        /// <summary>
        /// 已付款总金额
        /// </summary>
        /// <returns></returns>
        public async Task<decimal> PaidTotalAmount()
        {
            var query = from receiptNotice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ReceiptNotices>()
                        join financeReceipt in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceReceipts>() on receiptNotice.ID equals financeReceipt.ID
                        where receiptNotice.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                               && receiptNotice.ClientID == this.User.ClientID
                        select new
                        {
                            financeReceipt.Amount
                        };

            return await Task.Run(() => query.Sum(s => s.Amount));
        }

        /// <summary>
        /// 已经入账总金额
        /// </summary>
        /// <returns></returns>
        public async Task<decimal> RecordedTotalAmount()
        {
            var query = from receiptNotice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ReceiptNotices>()
                        where receiptNotice.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                               && receiptNotice.ClientID == this.User.ClientID
                        select new
                        {
                            receiptNotice.ClearAmount
                        };

            return await Task.Run(() => query.Sum(s => s.ClearAmount));
        }
    }
}