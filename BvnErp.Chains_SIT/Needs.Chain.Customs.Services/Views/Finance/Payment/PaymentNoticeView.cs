using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Enums;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 付款通知的视图
    /// </summary>
    public class PaymentNoticesView : UniqueView<Models.PaymentNotice, ScCustomsReponsitory>
    {
        public PaymentNoticesView()
        {
        }

        internal PaymentNoticesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<PaymentNotice> GetIQueryable()
        {
            var adminView = new AdminsTopView(this.Reponsitory);
            var paymentApplyView = new PaymentApplyView(this.Reponsitory);
            var payExchangeApplyView = new AdminPayExchangeApplyView(this.Reponsitory);
            var financeVaultsView = new FinanceVaultsView(this.Reponsitory);
            var financeAccountsView = new FinanceAccountsView(this.Reponsitory);

            var result = from notice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PaymentNotices>()
                         join admin in adminView on notice.AdminID equals admin.ID
                         join payer in adminView on notice.PayerID equals payer.ID into payers
                         from payer in payers.DefaultIfEmpty()
                         join payment in paymentApplyView on notice.PaymentApplyID equals payment.ID into payments
                         from payment in payments.DefaultIfEmpty()
                         join payexchange in payExchangeApplyView on notice.PayExchangeApplyID equals payexchange.ID into payexchanges
                         from payexchange in payexchanges.DefaultIfEmpty()
                         join financeVault in financeVaultsView on notice.FinanceVaultID equals financeVault.ID into financeVaults
                         from financeVault in financeVaults.DefaultIfEmpty()
                         join financeAccount in financeAccountsView on notice.FinanceAccountID equals financeAccount.ID into financeAccounts
                         from financeAccount in financeAccounts.DefaultIfEmpty()
                         select new Models.PaymentNotice
                         {
                             ID = notice.ID,
                             SeqNo = notice.SeqNo,
                             Admin = admin,
                             Payer = payer,
                             PaymentApply = payment,
                             PayExchangeApply = payexchange,
                             FinanceVault = financeVault,
                             FinanceAccount = financeAccount,
                             PayFeeType = (Enums.FinanceFeeType)notice.FeeType,
                             FeeDesc = notice.FeeDesc,
                             PayeeName = notice.PayeeName,
                             BankName = notice.BankName,
                             BankAccount = notice.BankAccount,
                             Amount = notice.Amount,
                             Currency = notice.Currency,
                             ExchangeRate = notice.ExchangeRate,
                             PayDate = notice.PayDate,
                             PayType = (Enums.PaymentType)notice.PayType,
                             CreateDate = notice.CreateDate,
                             UpdateDate = notice.UpdateDate,
                             Status = (Enums.PaymentNoticeStatus)notice.Status,
                             Summary = notice.Summary,
                             FeeTypeInt = notice.FeeType,
                             CostApplyID = notice.CostApplyID,
                             RefundApplyID = notice.RefundApplyID,
                             //增加手续费及流水号 2020-09-29 by yeshuangshuang
                             Poundage = notice.Poundage==null?0:notice.Poundage.Value,
                             SeqNoPoundage = notice.SeqNoPoundage,
                             USDAmount = notice.USDAmount
                         };
            return result;
        }
    }

    /// <summary>
    /// 订单付款通知的视图
    /// </summary>
    public class OrderPaymentNoticesView : UniqueView<Models.PaymentNotice, ScCustomsReponsitory>
    {
        public OrderPaymentNoticesView()
        {
        }

        internal OrderPaymentNoticesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<PaymentNotice> GetIQueryable()
        {
            var adminView = new AdminsTopView(this.Reponsitory);

            var result = from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PaymentNoticeItems>()
                         join notice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PaymentNotices>() on item.PaymentNoticeID equals notice.ID
                         join payer in adminView on notice.PayerID equals payer.ID into payers
                         from payer in payers.DefaultIfEmpty()
                         where item.Status == (int)Enums.PaymentNoticeStatus.Paid
                         select new Models.PaymentNotice
                         {
                             ID = item.ID,
                             Payer = payer,
                             PayFeeType = (Enums.FinanceFeeType)notice.FeeType,
                             PayeeName = notice.PayeeName,
                             Amount = item.Amount,
                             ExchangeRate = notice.ExchangeRate,
                             PayDate = notice.PayDate,
                             //增加手续费及流水号 2020-09-29 by yeshuangshuang
                             Poundage = notice.Poundage.Value,
                             SeqNoPoundage = notice.SeqNoPoundage
                         };
            return result;
        }
    }


}