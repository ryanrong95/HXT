using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ReceiptToYahvView
    {
        ScCustomsReponsitory Reponsitory { get; set; }

        public ReceiptToYahvView()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        public ReceiptToYahvViewModel GetData(string financeReceiptID)
        {
            var financeReceipts = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceReceipts>();
            var financeAccounts = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceAccounts>();

            var linq = from financeReceipt in financeReceipts
                       join financeAccount in financeAccounts
                            on new
                            {
                                FinanceAccountID = financeReceipt.FinanceAccountID,
                                FinanceReceiptDataStatus = financeReceipt.Status,
                                FinanceAccountDataStatus = (int)Enums.Status.Normal,
                            }
                            equals new
                            {
                                FinanceAccountID = financeAccount.ID,
                                FinanceReceiptDataStatus = (int)Enums.Status.Normal,
                                FinanceAccountDataStatus = financeAccount.Status,
                            }
                       where financeReceipt.ID == financeReceiptID
                       select new ReceiptToYahvViewModel
                       {
                           SeqNo = financeReceipt.SeqNo,
                           BankName = financeAccount.BankName,
                           BankAccount = financeAccount.BankAccount,
                       };

            return linq.FirstOrDefault();
        }
    }

    public class ReceiptToYahvViewModel
    {
        /// <summary>
        /// 流水号
        /// </summary>
        public string SeqNo { get; set; } = string.Empty;

        /// <summary>
        /// 银行
        /// </summary>
        public string BankName { get; set; } = string.Empty;

        /// <summary>
        /// 银行卡号
        /// </summary>
        public string BankAccount { get; set; } = string.Empty;

    }
}
