using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class FundTransferAppliesView : UniqueView<Models.FundTransferApplies, ScCustomsReponsitory>
    {
        public FundTransferAppliesView()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        public FundTransferAppliesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.FundTransferApplies> GetIQueryable()
        {
            var adminsView = new Views.AdminsTopView2(this.Reponsitory);
            var result = from apply in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FundTransferApplies>()                        
                         join admin in adminsView on apply.AdminID equals admin.OriginID
                         join outaccount in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceAccounts>() on apply.OutAccountID equals outaccount.ID
                         join inAccount in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceAccounts>() on apply.InAccountID equals inAccount.ID
                         where apply.Status == (int)Enums.Status.Normal
                         select new Models.FundTransferApplies
                         {
                             ID = apply.ID,
                             FromSeqNo = apply.FromSeqNo,
                             OutAccount = new Models.FinanceAccount { ID = outaccount.ID, AccountName = outaccount.AccountName,BankAccount = outaccount.BankAccount,FinanceVaultID=outaccount.FinanceVaultID,Balance=outaccount.Balance,BankName = outaccount.BankName },
                             OutAmount = apply.OutAmount,
                             OutSeqNo = apply.OutSeqNo,
                             InAccount = new Models.FinanceAccount { ID = inAccount.ID, AccountName = inAccount.AccountName, BankAccount = inAccount.BankAccount,FinanceVaultID=inAccount.FinanceVaultID, Balance = inAccount.Balance,BankName = inAccount.BankName },
                             InAmount = apply.InAmount,
                             InSeqNo = apply.InSeqNo,
                             DiscountInterest = apply.DiscountInterest,
                             Poundage = apply.Poundage,
                             PaymentType = (Enums.PaymentType)apply.PaymentType,
                             PaymentDate = apply.PaymentDate,
                             OrderID = apply.OrderID,
                             ApplyStatus = (Enums.FundTransferApplyStatus)apply.ApplyStatus,
                             Admin = admin,
                             Status = (Enums.Status)apply.Status,
                             CreateDate = apply.CreateDate,
                             UpdateDate = apply.UpdateDate,
                             Summary = apply.Summary,
                             FeeType = (Enums.FundTransferType)apply.FeeType,
                             InCurrency = apply.InCurrency,
                             OutCurrency = apply.OutCurrency,
                             Rate = apply.Rate==null?1:apply.Rate.Value,
                             QRCodeFee = apply.QRCodeFee,
                             FundTranCreSta = apply.FundTranCreSta,
                             FundTranWrod = apply.FundTranCreWord,
                             FundTranNo = apply.FundTranCreNo
                         };
            return result;
        }
    }

    /// <summary>
    /// 承兑视图
    /// </summary>
    public class ReceiptNoticesViewForAcceptanceBill : UniqueView<Models.FundTransferApplies, ScCustomsReponsitory>
    {
        public ReceiptNoticesViewForAcceptanceBill()
        {

        }

        public ReceiptNoticesViewForAcceptanceBill(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.FundTransferApplies> GetIQueryable()
        {
            var clientsView = new ClientsView(this.Reponsitory);
            var financeReceiptsView = new FinanceReceiptsView(this.Reponsitory);

            var result = from receiptNotice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ReceiptNotices>()
                         join financeReceipt in financeReceiptsView on receiptNotice.ID equals financeReceipt.ID
                         join fundTransferApply in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FundTransferApplies>() on financeReceipt.SeqNo equals fundTransferApply.FromSeqNo
                         join client in clientsView on receiptNotice.ClientID equals client.ID into clients
                         from client in clients.DefaultIfEmpty()
                         where receiptNotice.Status == (int)Enums.Status.Normal &&
                         fundTransferApply.Status== (int)Enums.Status.Normal&&
                         fundTransferApply.ApplyStatus==(int)Enums.FundTransferApplyStatus.Done
                         select new Models.FundTransferApplies()
                         {
                             ID = fundTransferApply.ID,
                             FromSeqNo = financeReceipt.SeqNo,
                             OutAccount = financeReceipt.Account,
                             DiscountInterest = fundTransferApply.DiscountInterest,
                             CreateDate = financeReceipt.ReceiptDate,
                             OrderID = fundTransferApply.OrderID,
                             Payer = financeReceipt.Payer,
                             OutSeqNo = fundTransferApply.OutSeqNo,
                             Client = client
                         };

            return result;
        }
    }
}
