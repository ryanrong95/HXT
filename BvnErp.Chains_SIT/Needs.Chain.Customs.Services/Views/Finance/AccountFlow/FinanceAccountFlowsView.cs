using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Services.Models;

namespace Needs.Ccs.Services.Views.Finance.AccountFlow
{
    public class FinanceAccountFlowsView : QueryView<Models.FinanceAccountFlow, ScCustomsReponsitory>
    {
        public FinanceAccountFlowsView()
        {
        }

        protected FinanceAccountFlowsView(ScCustomsReponsitory reponsitory, IQueryable<FinanceAccountFlow> iQueryable) : base(reponsitory, iQueryable)
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
        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Models.FinanceAccountFlow> iquery = this.IQueryable.Cast<Models.FinanceAccountFlow>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue) //如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_my = iquery.ToArray();

            var ienums_linq = from financeAccountFlowsNew in ienum_my
                              select new FinanceAccountFlow
                              {
                                  SeqNo = financeAccountFlowsNew.SeqNo,
                                  FinanceVault = financeAccountFlowsNew.FinanceVault,
                                  FinanceAccount = financeAccountFlowsNew.FinanceAccount,
                                  OtherAccount = financeAccountFlowsNew.OtherAccount,
                                  Type = financeAccountFlowsNew.Type,
                                  FeeTypeInt = financeAccountFlowsNew.FeeTypeInt,
                                  FeeType = (FinanceFeeType)financeAccountFlowsNew.FeeTypeInt,
                                  PaymentType = (PaymentType)financeAccountFlowsNew.PaymentType,
                                  Currency = financeAccountFlowsNew.Currency,
                                  Amount = financeAccountFlowsNew.Amount,
                                  AccountBalance = financeAccountFlowsNew.AccountBalance,
                                  CreateDate = financeAccountFlowsNew.CreateDate,
                              };

            var results = ienums_linq;

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return results.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            Func<FinanceAccountFlow, object> convert = accountFlow => new
            {
                SeqNo = accountFlow.SeqNo,
                Vault = accountFlow.FinanceVault.Name,
                Account = accountFlow.FinanceAccount.AccountName,
                OtherAccount = accountFlow.OtherAccount,
                Type = accountFlow.Type.GetDescription(),
                FeeType = accountFlow.FeeTypeInt > 10000 ? ((FeeTypeEnum)accountFlow.FeeTypeInt).ToString()
                                                         : ((FinanceFeeType)accountFlow.FeeTypeInt).GetDescription(),    //accountFlow.FeeType.GetDescription(),
                PayType = accountFlow.PaymentType.GetDescription(),
                Currency = accountFlow.Currency,
                Amount = accountFlow.Amount,
                Balance = accountFlow.AccountBalance,
                Date = accountFlow.CreateDate.ToShortDateString(),
                CreateDate = accountFlow.CreateDate,
            };

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.OrderByDescending(item => item.CreateDate).Select(convert).ToArray(),
            };
        }

        public FinanceAccountFlowsView SearchByFeeTypeInt(FinanceFeeType feeType)
        {
            var linq = from query in this.IQueryable
                       where query.FeeType == feeType
                       select query;

            var view = new FinanceAccountFlowsView(this.Reponsitory, linq);
            return view;
        }
        public FinanceAccountFlowsView SearchByPayType(PaymentType payType)
        {
            var linq = from query in this.IQueryable
                       where query.PaymentType == payType
                       select query;

            var view = new FinanceAccountFlowsView(this.Reponsitory, linq);
            return view;
        }
        public FinanceAccountFlowsView SearchByCurrency(string Currency)
        {
            var linq = from query in this.IQueryable
                       where query.Currency == Currency
                       select query;

            var view = new FinanceAccountFlowsView(this.Reponsitory, linq);
            return view;
        }
        public FinanceAccountFlowsView SearchByType(FinanceType type)
        {
            var linq = from query in this.IQueryable
                       where query.Type == type
                       select query;

            var view = new FinanceAccountFlowsView(this.Reponsitory, linq);
            return view;
        }
        public FinanceAccountFlowsView SearchByStartDate(DateTime startTime)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate >= startTime
                       select query;

            var view = new FinanceAccountFlowsView(this.Reponsitory, linq);
            return view;
        }

        public FinanceAccountFlowsView SearchByEndDate(DateTime endTime)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate < endTime
                       select query;

            var view = new FinanceAccountFlowsView(this.Reponsitory, linq);
            return view;
        }

        public FinanceAccountFlowsView SearchByVault(string Vault)
        {
                var linq = from query in this.IQueryable
                           where query.FinanceVault.ID == Vault
                           select query;

                var view = new FinanceAccountFlowsView(this.Reponsitory, linq);
                return view;
        }
        public FinanceAccountFlowsView SearchByAccount(string Account)
        {
            var linq = from query in this.IQueryable
                       where query.FinanceAccount.ID == Account
                       select query;

            var view = new FinanceAccountFlowsView(this.Reponsitory, linq);
            return view;
        }

        public FinanceAccountFlowsView SearchByHKCW()
        {
            var linq = from query in this.IQueryable
                       where query.FinanceVault.Name.Contains("香港")
                       select query;

            var view = new FinanceAccountFlowsView(this.Reponsitory, linq);
            return view;
        }
    }
}
