using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;

namespace Needs.Ccs.Services.Views
{

    public class FinanceReceiptsView : UniqueView<Models.FinanceReceipt, ScCustomsReponsitory>
    {
        public FinanceReceiptsView()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        public FinanceReceiptsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<FinanceReceipt> GetIQueryable()
        {
            var adminsView = new Views.AdminsTopView(this.Reponsitory);
            var vaultsView = new Views.FinanceVaultsView(this.Reponsitory);
            var accountsView = new Views.FinanceAccountsView(this.Reponsitory);
            var accountFlowsView = new Views.FinanceAccountFlowsView(this.Reponsitory);

            var result = from financeReceipt in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceReceipts>()
                         join vault in vaultsView on financeReceipt.FinanceVaultID equals vault.ID
                         join account in accountsView on financeReceipt.FinanceAccountID equals account.ID
                         join accountFlow in accountFlowsView on financeReceipt.ID equals accountFlow.SourceID
                         join admin in adminsView on financeReceipt.AdminID equals admin.ID
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
                             Vault = vault,
                             Account = account,
                             Admin = admin,
                             Status = (Status)financeReceipt.Status,
                             CreateDate = financeReceipt.CreateDate,
                             UpdateDate = financeReceipt.UpdateDate,
                             Summary = financeReceipt.Summary,
                             //对应的账户流水
                             AccountFlow = accountFlow,
                             AccountProperty = (AccountProperty)financeReceipt.AccountProperty,
                             DyjID = financeReceipt.DyjID
                         };
            return result;
        }
    }
}