using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 金库账户的视图
    /// </summary>
    public class FinanceAccountFlowsView : UniqueView<Models.FinanceAccountFlow, ScCustomsReponsitory>
    {
        public FinanceAccountFlowsView()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        public FinanceAccountFlowsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<FinanceAccountFlow> GetIQueryable()
        {
            var adminView = new AdminsTopView(this.Reponsitory);
            var financeVaultsView = new FinanceVaultsView(this.Reponsitory);
            var financeAccountsView = new FinanceAccountsView(this.Reponsitory);
            var receiptView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceReceipts>().Where(t => t.Status == (int)Enums.Status.Normal);
            var paymentView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinancePayments>().Where(t => t.Status == (int)Enums.Status.Normal);

            var result = from flow in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceAccountFlows>()
                         join admin in adminView on flow.AdminID equals admin.ID
                         join financeVault in financeVaultsView on flow.FinanceVaultID equals financeVault.ID
                         join financeAccount in financeAccountsView on flow.FinanceAccountID equals financeAccount.ID
                         join receipt in receiptView on flow.SourceID equals receipt.ID into receipts
                         from receipt in receipts.DefaultIfEmpty()
                         join payment in paymentView on flow.SourceID equals payment.ID into payments
                         from payment in payments.DefaultIfEmpty()
                         where flow.Status == (int)Enums.Status.Normal
                         select new Models.FinanceAccountFlow
                         {
                             ID = flow.ID,
                             Admin = admin,
                             SeqNo = flow.SeqNo,
                             SourceID = flow.SourceID,
                             FinanceVault = financeVault,
                             FinanceAccount = financeAccount,
                             Type = (Enums.FinanceType)flow.Type,
                             FeeTypeInt = flow.FeeType,
                             FeeType = (Enums.FinanceFeeType)flow.FeeType,
                             PaymentType = (Enums.PaymentType)flow.PaymentType,
                             Amount = flow.Amount,
                             Currency = flow.Currency,
                             AccountBalance = flow.AccountBalance,
                             Status = (Enums.Status)flow.Status,
                             CreateDate = payment == null ? receipt.ReceiptDate : payment.PayDate,
                             UpdateDate = flow.UpdateDate,
                             Summary = flow.Summary,
                             //对方户名
                             OtherAccount = payment == null ? receipt.Payer : payment.PayeeName,
                         };
            return result;
        }
    }
}
