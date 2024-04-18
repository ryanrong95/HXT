using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class GoodsImportView : UniqueView<Models.FinanceReceipt, ScCustomsReponsitory>
    {
        public GoodsImportView()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        public GoodsImportView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.FinanceReceipt> GetIQueryable()
        {          
          
            var result = from financeReceipt in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceReceipts>()
                         join vault in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceVaults>() on financeReceipt.FinanceVaultID equals vault.ID
                         join account in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceAccounts>() on financeReceipt.FinanceAccountID equals account.ID                        
                      
                         where financeReceipt.Status == (int)Enums.Status.Normal
                         select new Models.FinanceReceipt
                         {
                             ID = financeReceipt.ID,
                             SeqNo = financeReceipt.SeqNo,
                             Payer = financeReceipt.Payer,
                             FeeType = (FinanceFeeType)financeReceipt.FeeType,
                             ReceiptType = (PaymentType)financeReceipt.ReceiptType,
                             ReceiptDate = financeReceipt.ReceiptDate,
                             Currency = financeReceipt.Currency,
                             Rate = financeReceipt.Rate,
                             Amount = financeReceipt.Amount,                             
                             Vault = new Models.FinanceVault {
                                 ID = vault.ID,
                                 Name = vault.Name,
                             },
                             Account = new Models.FinanceAccount {
                                 ID = account.ID,
                                 AccountName = account.AccountName,
                                 BankName = account.BankName,
                             },
                          
                             Status = (Status)financeReceipt.Status,
                             CreateDate = financeReceipt.CreateDate,
                             UpdateDate = financeReceipt.UpdateDate,
                             Summary = financeReceipt.Summary,                                                 
                             AccountProperty = (AccountProperty)financeReceipt.AccountProperty,
                             DyjID = financeReceipt.DyjID,
                             GoodsCreStatus = financeReceipt.GoodsCreStatus,
                             GoodsCreWord = financeReceipt.GoodsCreWord,
                             GoodsCreNo = financeReceipt.GoodsCreNo,
                             RequestID = financeReceipt.RequestID
                         };
            return result;
        }
    }
}
