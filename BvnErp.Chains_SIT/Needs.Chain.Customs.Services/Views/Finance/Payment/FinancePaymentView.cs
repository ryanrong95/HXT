using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
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
    /// 财务付款视图
    /// </summary>
    public class FinancePaymentView : UniqueView<Models.FinancePayment, ScCustomsReponsitory>
    {
        public FinancePaymentView()
        {
        }

        internal FinancePaymentView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<FinancePayment> GetIQueryable()
        {
            var adminView = new AdminsTopView(this.Reponsitory);
            var financeVaultsView = new FinanceVaultsView(this.Reponsitory);
            var financeAccountsView = new FinanceAccountsView(this.Reponsitory);
            var flowsView = new FinanceAccountFlowsView(this.Reponsitory);

            var result = from financePayment in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinancePayments>()
                         join payer in adminView on financePayment.PayerID equals payer.ID
                         join financeVault in financeVaultsView on financePayment.FinanceVaultID equals financeVault.ID
                         join financeAccount in financeAccountsView on financePayment.FinanceAccountID equals financeAccount.ID
                         join flow in flowsView on financePayment.ID equals flow.SourceID
                         where financePayment.Status == (int)Enums.Status.Normal
                         select new Models.FinancePayment
                         {
                             ID = financePayment.ID,
                             SeqNo = financePayment.SeqNo,
                             PayeeName = financePayment.PayeeName,
                             BankName = financePayment.BankName,
                             BankAccount = financePayment.BankAccount,
                             Payer = payer,
                             FinanceVault = financeVault,
                             FinanceAccount = financeAccount,
                             Amount = financePayment.Amount,
                             Currency = financePayment.Currency,
                             ExchangeRate = financePayment.ExchangeRate,
                             PayFeeType = (Enums.FinanceFeeType)financePayment.FeeType,
                             PayType = (Enums.PaymentType)financePayment.PayType,
                             PayDate = financePayment.PayDate,
                             Status = (Enums.Status)financePayment.Status,
                             CreateDate = financePayment.CreateDate,
                             UpdateDate = financePayment.UpdateDate,
                             Summary = financePayment.Summary,
                             //对应账户流水
                             FinanceAccountFlow = flow,
                             FeeTypeInt = financePayment.FeeType,
                         };
            return result;
        }
    }


    /// <summary>
    /// 付款列表速度优化修改
    /// </summary>
    public class FinancePaymentViewRJ : QueryView<FiancePaymentOrigin, ScCustomsReponsitory>
    {
        public FinancePaymentViewRJ()
        {
        }

        internal FinancePaymentViewRJ(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected FinancePaymentViewRJ(ScCustomsReponsitory reponsitory, IQueryable<FiancePaymentOrigin> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<FiancePaymentOrigin> GetIQueryable()
        {
            var financePaymentView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinancePayments>();
            var vaultView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceVaults>();
            var accountView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceAccounts>();
            var adminView = new AdminsTopView(this.Reponsitory);

            var linq = from finance in financePaymentView
                       join vault in vaultView on finance.FinanceVaultID equals vault.ID
                       join account in accountView on finance.FinanceAccountID equals account.ID
                       join admin in adminView on finance.PayerID equals admin.ID
                       select new FiancePaymentOrigin
                       {
                           ID = finance.ID,
                           SeqNo = finance.SeqNo,
                           PayeeName = finance.PayeeName,
                           FinanceVaultName = vault.Name,
                           FinanceAccountName = account.AccountName,
                           FeeTypeInt = finance.FeeType,
                           PayerName = admin.ByName,
                           Amount = finance.Amount,
                           Currency = finance.Currency,
                           PayDate = finance.PayDate,
                           FinPCreSta = finance.FinPCreSta,
                           FinPCreNo = finance.FinPCreNo,
                           FinPCreWord = finance.FinPCreWord,
                           BankName = account.BankName,
                           RequestID = finance.RequestID
                       };

            return linq;
        }

        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<FiancePaymentOrigin> iquery = this.IQueryable.Cast<FiancePaymentOrigin>().OrderByDescending(item => item.PayDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            var results = iquery.ToArray();


            Func<FiancePaymentOrigin, object> convert = item => new
            {
                ID = item.ID,
                SeqNo = item.SeqNo,
                FinanceVault = item.FinanceVaultName,
                FinanceAccount = item.FinanceAccountName,
                FeeTypeInt = item.FeeTypeInt,
                FeeType = item.FeeTypeInt > 10000 ? ((Needs.Ccs.Services.Enums.FeeTypeEnum)item.FeeTypeInt).ToString()
                                                  : ((Needs.Ccs.Services.Enums.FinanceFeeType)item.FeeTypeInt).GetDescription(),    //item.PayFeeType.GetDescription(),
                PayeeName = item.PayeeName,
                Amount = item.Amount,
                Currency = item.Currency,
                PayDate = item.PayDate.ToShortDateString(),
                Payer = item.PayerName,
                item.FinPCreSta,
                item.FinPCreNo,
                item.FinPCreWord,
                item.BankName,
                item.RequestID

            };


            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }



        /// <summary>
        /// 过滤状态--费用类型
        /// </summary>
        /// <returns>视图</returns>
        public FinancePaymentViewRJ SearchByFeeType(int feeType)
        {
            var linq = from query in this.IQueryable
                       where query.FeeTypeInt == feeType
                       select query;

            var view = new FinancePaymentViewRJ(this.Reponsitory, linq);
            return view;
        }

        public FinancePaymentViewRJ SearchByPoundge()
        {
            var linq = from query in this.IQueryable
                       where query.FeeTypeInt == (int)Needs.Ccs.Services.Enums.FinanceFeeType.Poundage ||
                             query.FeeTypeInt == (int)Needs.Ccs.Services.Enums.FinanceFeeType.PayBankPaypal
                       select query;

            var view = new FinancePaymentViewRJ(this.Reponsitory, linq);
            return view;
        }

        public FinancePaymentViewRJ SearchByFrom(DateTime fromtime)
        {
            var linq = from query in this.IQueryable
                       where query.PayDate >= fromtime
                       select query;

            var view = new FinancePaymentViewRJ(this.Reponsitory, linq);
            return view;
        }

        public FinancePaymentViewRJ SearchByTo(DateTime totime)
        {
            var to = totime.AddDays(1);
            var linq = from query in this.IQueryable
                       where query.PayDate < to
                       select query;

            var view = new FinancePaymentViewRJ(this.Reponsitory, linq);
            return view;
        }

        public FinancePaymentViewRJ SearchByHKCW()
        {
            var linq = from query in this.IQueryable
                       where query.FinanceVaultName.Contains("香港")
                       select query;

            var view = new FinancePaymentViewRJ(this.Reponsitory, linq);
            return view;
        }

        public FinancePaymentViewRJ SearchByPayeeName(string payeeName)
        {
            var linq = from query in this.IQueryable
                       where query.PayeeName.Contains(payeeName)
                       select query;

            var view = new FinancePaymentViewRJ(this.Reponsitory, linq);
            return view;
        }

        public FinancePaymentViewRJ SearchByCreSta(bool sta)
        {
            var linq = from query in this.IQueryable
                       where query.FinPCreSta == sta
                       select query;

            var view = new FinancePaymentViewRJ(this.Reponsitory, linq);
            return view;
        }

        public FinancePaymentViewRJ SearchByVaultName(string vaultName)
        {
            var linq = from query in this.IQueryable
                       where query.FinanceVaultName == vaultName
                       select query;

            var view = new FinancePaymentViewRJ(this.Reponsitory, linq);
            return view;
        }

        public FinancePaymentViewRJ SearchByOther()
        {
            var linq = from query in this.IQueryable
                       where query.FeeTypeInt != (int)Needs.Ccs.Services.Enums.FinanceFeeType.Poundage &&
                             query.FeeTypeInt != (int)Needs.Ccs.Services.Enums.FinanceFeeType.PayBankPaypal &&
                             query.FeeTypeInt != (int)Needs.Ccs.Services.Enums.FinanceFeeType.FundTransfer &&
                             query.FinanceVaultName != "香港金库" &&
                             !query.PayeeName.Contains("暂收款")
                       select query;

            var view = new FinancePaymentViewRJ(this.Reponsitory, linq);
            return view;
        }
    }

    /// <summary>
    /// 付款列表使用的简洁视图结构
    /// </summary>
    public class FiancePaymentOrigin : IUnique
    {
        public string ID { get; set; }

        public string SeqNo { get; set; }

        public string PayeeName { get; set; }

        public string FinanceVaultName { get; set; }

        public string FinanceAccountName { get; set; }

        public int FeeTypeInt { get; set; }

        public string FeeType { get; set; }

        public string PayerName { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public DateTime PayDate { get; set; }
        public bool? FinPCreSta { get; set; }
        public string FinPCreWord { get; set; }
        public string FinPCreNo { get; set; }
        public string BankName { get; set; }
        public string RequestID { get; set; }
    }
}
