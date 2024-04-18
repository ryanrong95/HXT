using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 付款列表视图(查 FinancePayments 表)
    /// </summary>
    public class PaiedListView : QueryView<PaiedListViewModel, ScCustomsReponsitory>
    {
        public PaiedListView()
        {
        }

        protected PaiedListView(ScCustomsReponsitory reponsitory, IQueryable<PaiedListViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<PaiedListViewModel> GetIQueryable()
        {
            var financePayments = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinancePayments>();

            var iQuery = from financePayment in financePayments
                         where financePayment.Status == (int)Enums.Status.Normal
                         orderby financePayment.CreateDate descending
                         select new PaiedListViewModel
                         {
                             FinancePaymentID = financePayment.ID,
                             SeqNo = financePayment.SeqNo,
                             PayeeName = financePayment.PayeeName,
                             FinanceVaultID = financePayment.FinanceVaultID,
                             FinanceAccountID = financePayment.FinanceAccountID,
                             FeeTypeInt = financePayment.FeeType,
                             Amount = financePayment.Amount,
                             Currency = financePayment.Currency,
                             PayerID = financePayment.PayerID,
                             PayType = (Enums.PaymentType)financePayment.PayType,
                             PayDate = financePayment.PayDate,
                             IsPaperInvoiceUpload = (Enums.UploadStatus)financePayment.IsPaperInvoiceUpload,
                         };

            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<PaiedListViewModel> iquery = this.IQueryable.Cast<PaiedListViewModel>().OrderByDescending(item => item.PayDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_FinancePayments = iquery.ToArray();

            var financeVaultIDs = ienum_FinancePayments.Select(t => t.FinanceVaultID).ToArray();
            var financeAccountIDs = ienum_FinancePayments.Select(t => t.FinanceAccountID).ToArray();
            var payerIDs = ienum_FinancePayments.Select(t => t.PayerID).ToArray();

            #region 付款金库

            var financeVaults = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceVaults>();

            var linq_Vaults = from financeVault in financeVaults
                              where financeVault.Status == (int)Enums.Status.Normal
                                 && financeVaultIDs.Contains(financeVault.ID)
                              select new
                              {
                                  FinanceVaultID = financeVault.ID,
                                  FinanceVaultName = financeVault.Name,
                              };

            var ienums_Vaults = linq_Vaults.ToArray();

            #endregion

            #region 付款账户

            var financeAccounts = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceAccounts>();

            var linq_Accounts = from financeAccount in financeAccounts
                                where financeAccount.Status == (int)Enums.Status.Normal
                                   && financeAccountIDs.Contains(financeAccount.ID)
                                select new
                                {
                                    FinanceAccountID = financeAccount.ID,
                                    FinanceAccountName = financeAccount.AccountName,
                                };

            var ienums_Accounts = linq_Accounts.ToArray();

            #endregion

            #region 付款人

            var adminsTopView2 = new AdminsTopView2(this.Reponsitory);

            var linq_payer = from admin in adminsTopView2
                             where payerIDs.Contains(admin.OriginID)
                             group admin by new { admin.OriginID } into g
                             select new
                             {
                                 PayerID = g.Key.OriginID,
                                 PayerName = g.FirstOrDefault().RealName,
                             };

            var ienums_payer = linq_payer.ToArray();

            #endregion

            var ienums_linq = from financePayment in ienum_FinancePayments
                              join vault in ienums_Vaults on financePayment.FinanceVaultID equals vault.FinanceVaultID into ienums_Vaults2
                              from vault in ienums_Vaults2.DefaultIfEmpty()
                              join account in ienums_Accounts on financePayment.FinanceAccountID equals account.FinanceAccountID into ienums_Accounts2
                              from account in ienums_Accounts2.DefaultIfEmpty()
                              join payer in ienums_payer on financePayment.PayerID equals payer.PayerID into ienums_payer2
                              from payer in ienums_payer2.DefaultIfEmpty()
                              select new PaiedListViewModel
                              {
                                  FinancePaymentID = financePayment.FinancePaymentID,
                                  SeqNo = financePayment.SeqNo,
                                  PayeeName = financePayment.PayeeName,
                                  FinanceVaultID = financePayment.FinanceVaultID,
                                  FinanceAccountID = financePayment.FinanceAccountID,
                                  FeeTypeInt = financePayment.FeeTypeInt,
                                  Amount = financePayment.Amount,
                                  Currency = financePayment.Currency,
                                  PayerID = financePayment.PayerID,
                                  PayType = financePayment.PayType,
                                  PayDate = financePayment.PayDate,
                                  IsPaperInvoiceUpload = financePayment.IsPaperInvoiceUpload,

                                  FinanceVaultName = vault != null ? vault.FinanceVaultName : "",
                                  FinanceAccountName = account != null ? account.FinanceAccountName : "",
                                  PayerName = payer != null ? payer.PayerName : "",
                              };

            var results = ienums_linq;

            Func<PaiedListViewModel, object> convert = item => new
            {
                FinancePaymentID = item.FinancePaymentID,
                SeqNo = item.SeqNo,
                PayeeName = item.PayeeName,
                FinanceVaultID = item.FinanceVaultID,
                FinanceAccountID = item.FinanceAccountID,
                FeeTypeInt = item.FeeTypeInt,
                Amount = item.Amount,
                Currency = item.Currency,
                PayerID = item.PayerID,
                PayType = item.PayType.GetDescription(),
                PayDate = item.PayDate.ToString("yyyy-MM-dd"),
                IsPaperInvoiceUploadInt = (int)item.IsPaperInvoiceUpload,
                IsPaperInvoiceUploadDes = item.IsPaperInvoiceUpload.GetDescription(),

                FinanceVaultName = item.FinanceVaultName,
                FinanceAccountName = item.FinanceAccountName,
                PayerName = item.PayerName,

                FeeTypeName = item.FeeTypeInt > 10000 ? ((Needs.Ccs.Services.Enums.FeeTypeEnum)item.FeeTypeInt).ToString()
                                                      : ((Needs.Ccs.Services.Enums.FinanceFeeType)item.FeeTypeInt).GetDescription(),
            };

            if (!pageIndex.HasValue && !pageSize.HasValue)
            {
                Func<dynamic, object> convertAgain = item => new
                {
                    item.SeqNo,
                    item.PayeeName,
                    item.FinanceVaultName,
                    item.FinanceAccountName,
                    item.FeeTypeName,
                    item.Amount,
                    item.Currency,
                    item.PayType,
                    item.PayerName,
                    item.PayDate,
                };

                return results.Select(convert).Select(convertAgain).ToArray();
            }

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }

        /// <summary>
        /// 根据费用类型查询
        /// </summary>
        /// <param name="feeTypeInt"></param>
        /// <returns></returns>
        public PaiedListView SearchByFeeType(int feeTypeInt)
        {
            var linq = from query in this.IQueryable
                       where query.FeeTypeInt == feeTypeInt
                       select query;

            var view = new PaiedListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据付款类型查询
        /// </summary>
        /// <param name="payType"></param>
        /// <returns></returns>
        public PaiedListView SearchByPayType(Enums.PaymentType payType)
        {
            var linq = from query in this.IQueryable
                       where query.PayType == payType
                       select query;

            var view = new PaiedListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据付款日期开始时间查询
        /// </summary>
        /// <param name="begin"></param>
        /// <returns></returns>
        public PaiedListView SearchByPayDateStartDate(DateTime begin)
        {
            var linq = from query in this.IQueryable
                       where query.PayDate >= begin
                       select query;

            var view = new PaiedListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据付款日期结束时间查询
        /// </summary>
        /// <param name="end"></param>
        /// <returns></returns>
        public PaiedListView SearchByPayDateEndDate(DateTime end)
        {
            var linq = from query in this.IQueryable
                       where query.PayDate < end
                       select query;

            var view = new PaiedListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据FinanceVaultID查询
        /// </summary>
        /// <param name="financeVaultID"></param>
        /// <returns></returns>
        public PaiedListView SearchByFinanceVaultID(string financeVaultID)
        {
            var linq = from query in this.IQueryable
                       where query.FinanceVaultID == financeVaultID
                       select query;

            var view = new PaiedListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据FinanceAccountID查询
        /// </summary>
        /// <param name="financeAccountID"></param>
        /// <returns></returns>
        public PaiedListView SearchByFinanceAccountID(string financeAccountID)
        {
            var linq = from query in this.IQueryable
                       where query.FinanceAccountID == financeAccountID
                       select query;

            var view = new PaiedListView(this.Reponsitory, linq);
            return view;
        }
    }

    public class PaiedListViewModel
    {
        /// <summary>
        /// FinancePaymentID
        /// </summary>
        public string FinancePaymentID { get; set; }

        /// <summary>
        /// 流水号
        /// </summary>
        public string SeqNo { get; set; }

        /// <summary>
        /// 收款方
        /// </summary>
        public string PayeeName { get; set; }

        /// <summary>
        /// FinanceVaultID
        /// </summary>
        public string FinanceVaultID { get; set; }

        /// <summary>
        /// 付款金库
        /// </summary>
        public string FinanceVaultName { get; set; }

        /// <summary>
        /// FinanceAccountID
        /// </summary>
        public string FinanceAccountID { get; set; }

        /// <summary>
        /// 付款账户
        /// </summary>
        public string FinanceAccountName { get; set; }

        /// <summary>
        /// FeeTypeInt
        /// </summary>
        public int FeeTypeInt { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 付款人ID
        /// </summary>
        public string PayerID { get; set; }

        /// <summary>
        /// 付款人姓名
        /// </summary>
        public string PayerName { get; set; }

        /// <summary>
        /// 付款类型
        /// </summary>
        public Enums.PaymentType PayType { get; set; }

        /// <summary>
        /// 付款日期
        /// </summary>
        public DateTime PayDate { get; set; }

        /// <summary>
        /// 是否上传纸质单据
        /// </summary>
        public Enums.UploadStatus IsPaperInvoiceUpload { get; set; }
    }

}
