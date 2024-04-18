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
    /// 财务收款(查 FinanceReceipts 表)视图
    /// </summary>
    public class ReceiptedListView : QueryView<ReceiptedListViewModel, ScCustomsReponsitory>
    {
        public ReceiptedListView()
        {
        }

        protected ReceiptedListView(ScCustomsReponsitory reponsitory, IQueryable<ReceiptedListViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<ReceiptedListViewModel> GetIQueryable()
        {
            var financeReceipts = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceReceipts>();

            var iQuery = from financeReceipt in financeReceipts
                         where financeReceipt.Status == (int)Enums.Status.Normal
                         orderby financeReceipt.CreateDate descending
                         select new ReceiptedListViewModel
                         {
                             FinanceReceiptID = financeReceipt.ID,
                             SeqNo = financeReceipt.SeqNo,
                             FinanceVaultID = financeReceipt.FinanceVaultID,
                             FinanceAccountID = financeReceipt.FinanceAccountID,
                             Payer = financeReceipt.Payer,
                             FeeType = (Enums.FinanceFeeType)financeReceipt.FeeType,
                             Amount = financeReceipt.Amount,
                             Currency = financeReceipt.Currency,
                             ReceiptDate = financeReceipt.ReceiptDate,
                         };

            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<ReceiptedListViewModel> iquery = this.IQueryable.Cast<ReceiptedListViewModel>().OrderByDescending(item => item.ReceiptDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_financeReceipts = iquery.ToArray();

            var financeVaultIDs = ienum_financeReceipts.Select(t => t.FinanceVaultID);
            var financeAccountIDs = ienum_financeReceipts.Select(t => t.FinanceAccountID);
            var financeReceiptIDs = ienum_financeReceipts.Select(t => t.FinanceReceiptID);

            var orderReceipts = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>();
            var swapNotices = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNotices>();
            var swapNoticeReceiptUses = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticeReceiptUses>();

            #region 金库名称、账户名称

            var financeVaults = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceVaults>();

            var linq_financeVaults = from financeVault in financeVaults
                                     where financeVault.Status == (int)Enums.Status.Normal
                                        && financeVaultIDs.Contains(financeVault.ID)
                                     select new
                                     {
                                         FinanceVaultID = financeVault.ID,
                                         FinanceVaultName = financeVault.Name,
                                     };

            var ienums_financeVaults = linq_financeVaults.ToArray();


            var financeAccounts = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceAccounts>();

            var linq_financeAccounts = from financeAccount in financeAccounts
                                       where financeAccount.Status == (int)Enums.Status.Normal
                                          && financeAccountIDs.Contains(financeAccount.ID)
                                       select new
                                       {
                                           FinanceAccountID = financeAccount.ID,
                                           FinanceAccountName = financeAccount.AccountName,
                                       };

            var ienums_financeAccounts = linq_financeAccounts.ToArray();

            #endregion

            #region 关税、增值税、消费税、代理费、杂费

            int[] taxFeeTypes = new int[]
            {
                (int)Enums.OrderFeeType.Tariff,
                (int)Enums.OrderFeeType.AddedValueTax,
                (int)Enums.OrderFeeType.AgencyFee,
                (int)Enums.OrderFeeType.Incidental,
                (int)Enums.OrderFeeType.ExciseTax,
            };

            var linq_taxAmounts = from orderReceipt in orderReceipts
                                  where financeReceiptIDs.Contains(orderReceipt.FinanceReceiptID)
                                     && orderReceipt.Status == (int)Enums.Status.Normal
                                     && orderReceipt.Type == (int)Enums.OrderReceiptType.Received
                                     && taxFeeTypes.Contains(orderReceipt.FeeType)
                                  group orderReceipt by new { orderReceipt.FinanceReceiptID, orderReceipt.FeeType, } into g
                                  select new
                                  {
                                      FinanceReceiptID = g.Key.FinanceReceiptID,
                                      FeeType = (Enums.OrderFeeType)g.Key.FeeType,
                                      Amount = g.Sum(t => 0 - t.Amount),
                                  };

            var ienums_taxAmounts = linq_taxAmounts.ToArray();

            var tariff_ienums_taxAmounts = ienums_taxAmounts.Where(t => t.FeeType == Enums.OrderFeeType.Tariff).ToArray();
            var addedValueTax_ienums_taxAmounts = ienums_taxAmounts.Where(t => t.FeeType == Enums.OrderFeeType.AddedValueTax).ToArray();
            var agencyFee_ienums_taxAmounts = ienums_taxAmounts.Where(t => t.FeeType == Enums.OrderFeeType.AgencyFee).ToArray();
            var incidental_ienums_taxAmounts = ienums_taxAmounts.Where(t => t.FeeType == Enums.OrderFeeType.Incidental).ToArray();
            var exciseTax_ienums_taxAmounts = ienums_taxAmounts.Where(t => t.FeeType == Enums.OrderFeeType.ExciseTax).ToArray();

            #endregion

            #region 货款、换汇汇率

            //总共的货款
            var linq_productFees_all = from orderReceipt in orderReceipts
                                       where financeReceiptIDs.Contains(orderReceipt.FinanceReceiptID)
                                          && orderReceipt.Status == (int)Enums.Status.Normal
                                          && orderReceipt.Type == (int)Enums.OrderReceiptType.Received
                                          && orderReceipt.FeeType == (int)Enums.OrderFeeType.Product
                                       group orderReceipt by new { orderReceipt.FinanceReceiptID, } into g
                                       select new
                                       {
                                           FinanceReceiptID = g.Key.FinanceReceiptID,
                                           Amount = g.Sum(t => 0 - t.Amount),
                                       };

            var ienums_productFees_all = linq_productFees_all.ToArray();

            //使用了 SwapNotice 的货款
            var linq_productFees_useSwap = from orderReceipt in orderReceipts
                                           join swapNoticeReceiptUse in swapNoticeReceiptUses on orderReceipt.ID equals swapNoticeReceiptUse.OrderReceiptID
                                           join swapNotice in swapNotices on swapNoticeReceiptUse.SwapNoticeID equals swapNotice.ID

                                           where financeReceiptIDs.Contains(orderReceipt.FinanceReceiptID)
                                             && orderReceipt.Status == (int)Enums.Status.Normal
                                             && orderReceipt.Type == (int)Enums.OrderReceiptType.Received
                                             && orderReceipt.FeeType == (int)Enums.OrderFeeType.Product

                                             && swapNoticeReceiptUse.Status == (int)Enums.Status.Normal
                                           group new { orderReceipt, swapNotice, } by new { orderReceipt.FinanceReceiptID, swapNotice.ExchangeRate, } into g
                                           select new
                                           {
                                               FinanceReceiptID = g.Key.FinanceReceiptID,
                                               ExchangeRate = g.Key.ExchangeRate,
                                               Amount = g.Sum(t => 0 - t.orderReceipt.Amount),
                                           };

            var ienums_productFees_useSwap = linq_productFees_useSwap.ToList();

            var ienums_productFees_useSwapByFinanceReceiptID = (from useSwap in ienums_productFees_useSwap
                                                                group useSwap by new { useSwap.FinanceReceiptID, } into g
                                                                select new
                                                                {
                                                                    FinanceReceiptID = g.Key.FinanceReceiptID,
                                                                    Amount = g.Sum(t => t.Amount),
                                                                }).ToArray();

            //未使用 SwapNotice 的货款
            var ienums_productFees_NoUseSwap = (from all in ienums_productFees_all
                                                join useByFinanceReceiptID in ienums_productFees_useSwapByFinanceReceiptID on all.FinanceReceiptID equals useByFinanceReceiptID.FinanceReceiptID
                                                into ienums_productFees_useSwapByFinanceReceiptID222
                                                from useByFinanceReceiptID in ienums_productFees_useSwapByFinanceReceiptID222.DefaultIfEmpty()
                                                select new
                                                {
                                                    FinanceReceiptID = all.FinanceReceiptID,
                                                    NoUseAmount = all.Amount - (useByFinanceReceiptID != null ? useByFinanceReceiptID.Amount : 0),
                                                }).ToList();



            var results_productFees = ienums_productFees_useSwap.Select(t1 => new
            {
                FinanceReceiptID = t1.FinanceReceiptID,
                ExchangeRate = Convert.ToString(t1.ExchangeRate),
                Amount = t1.Amount,
            }).Union(
                ienums_productFees_NoUseSwap.Select(t2 => new
                {
                    FinanceReceiptID = t2.FinanceReceiptID,
                    ExchangeRate = "",
                    Amount = t2.NoUseAmount,
                })
            ).ToArray();


            #endregion

            var ienums_linq = from financeReceipt in ienum_financeReceipts
                              join financeVault in ienums_financeVaults on financeReceipt.FinanceVaultID equals financeVault.FinanceVaultID into ienums_financeVaults2
                              from financeVault in ienums_financeVaults2.DefaultIfEmpty()
                              join financeAccount in ienums_financeAccounts on financeReceipt.FinanceAccountID equals financeAccount.FinanceAccountID into ienums_financeAccounts2
                              from financeAccount in ienums_financeAccounts2.DefaultIfEmpty()

                              join tariff in tariff_ienums_taxAmounts on financeReceipt.FinanceReceiptID equals tariff.FinanceReceiptID into tariff_ienums_taxAmounts2
                              from tariff in tariff_ienums_taxAmounts2.DefaultIfEmpty()
                              join addedValueTax in addedValueTax_ienums_taxAmounts on financeReceipt.FinanceReceiptID equals addedValueTax.FinanceReceiptID into addedValueTax_ienums_taxAmounts2
                              from addedValueTax in addedValueTax_ienums_taxAmounts2.DefaultIfEmpty()
                              join agencyFee in agencyFee_ienums_taxAmounts on financeReceipt.FinanceReceiptID equals agencyFee.FinanceReceiptID into agencyFee_ienums_taxAmounts2
                              from agencyFee in agencyFee_ienums_taxAmounts2.DefaultIfEmpty()
                              join incidental in incidental_ienums_taxAmounts on financeReceipt.FinanceReceiptID equals incidental.FinanceReceiptID into incidental_ienums_taxAmounts2
                              from incidental in incidental_ienums_taxAmounts2.DefaultIfEmpty()
                              join exciseTax in exciseTax_ienums_taxAmounts on financeReceipt.FinanceReceiptID equals exciseTax.FinanceReceiptID into exciseTax_ienums_taxAmounts2
                              from exciseTax in exciseTax_ienums_taxAmounts2.DefaultIfEmpty()

                                  //关联 ienums_productFees 会导致每行数据变为多行, 页面上需要上下合并单元格
                              join productFee in results_productFees on financeReceipt.FinanceReceiptID equals productFee.FinanceReceiptID into results_productFees222
                              from productFee in results_productFees222.DefaultIfEmpty()

                              select new ReceiptedListViewModel
                              {
                                  FinanceReceiptID = financeReceipt.FinanceReceiptID,
                                  SeqNo = financeReceipt.SeqNo,
                                  FinanceVaultID = financeReceipt.FinanceVaultID,
                                  FinanceAccountID = financeReceipt.FinanceAccountID,
                                  Payer = financeReceipt.Payer,
                                  FeeType = (Enums.FinanceFeeType)financeReceipt.FeeType,
                                  Amount = financeReceipt.Amount,
                                  Currency = financeReceipt.Currency,
                                  ReceiptDate = financeReceipt.ReceiptDate,

                                  FinanceVaultName = financeVault != null ? financeVault.FinanceVaultName : "",
                                  FinanceAccountName = financeAccount != null ? financeAccount.FinanceAccountName : "",

                                  TariffAmount = tariff != null ? tariff.Amount : 0,
                                  AddedValueTaxAmount = addedValueTax != null ? addedValueTax.Amount : 0,
                                  AgencyFeeAmount = agencyFee != null ? agencyFee.Amount : 0,
                                  IncidentalAmount = incidental != null ? incidental.Amount : 0,
                                  ExciseTaxAmount = exciseTax != null ? exciseTax.Amount : 0,

                                  ProductAmount = productFee != null ? productFee.Amount : 0,
                                  SwapExchangeRate = productFee != null ? productFee.ExchangeRate : "",
                              };

            var results = ienums_linq.ToArray();

            #region 当天汇率

            var receiptDates = ienum_financeReceipts.Select(t => t.ReceiptDate).Distinct().ToArray();

            var exchangeRates = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExchangeRates>();
            var exchangeRateLogs = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExchangeRateLogs>();

            //查当前实时汇率表中的所有值
            var linq_exchangeRate = from exchangeRate in exchangeRates
                                    where exchangeRate.Type == (int)Enums.ExchangeRateType.RealTime
                                    select new
                                    {
                                        ExchangeRateID = exchangeRate.ID,
                                        Code = exchangeRate.Code,
                                        Rate = exchangeRate.Rate,
                                        UpdateDate = exchangeRate.UpdateDate,
                                    };

            var ienums_exchangeRate = linq_exchangeRate.ToArray();

            var exchangeRateIDs = ienums_exchangeRate.Select(t => t.ExchangeRateID);

            //查实时汇率日志表中的可能值
            var linq_exchangeRateLog = from exchangeRateLog in exchangeRateLogs
                                       join exchangeRate in exchangeRates on exchangeRateLog.ExchangeRateID equals exchangeRate.ID
                                       where exchangeRateIDs.Contains(exchangeRateLog.ExchangeRateID)
                                          && receiptDates.Contains(exchangeRateLog.CreateDate.Date)
                                       select new
                                       {
                                           Code = exchangeRate.Code,
                                           Rate = exchangeRateLog.Rate,
                                           CreateDate = exchangeRateLog.CreateDate,
                                       };

            var ienums_exchangeRateLog = linq_exchangeRateLog.ToArray();

            //对每一条数据，先用日志表的值，再用当前实时汇率表中的值
            for (int i = 0; i < results.Length; i++)
            {
                var theExchangeRateLog = ienums_exchangeRateLog
                        .Where(t => t.Code == results[i].Currency
                                 && t.CreateDate.ToString("yyyy-MM-dd") == results[i].ReceiptDate.ToString("yyyy-MM-dd"))
                        .OrderByDescending(t => t.CreateDate).FirstOrDefault();

                if (theExchangeRateLog != null)
                {
                    results[i].ThatDayExchangeRate = theExchangeRateLog.Rate;
                }
            }

            for (int i = 0; i < results.Length; i++)
            {
                var theExchangeRate = ienums_exchangeRate
                        .Where(t => t.Code == results[i].Currency
                                 && t.UpdateDate?.ToString("yyyy-MM-dd") == results[i].ReceiptDate.ToString("yyyy-MM-dd"))
                        .OrderByDescending(t => t.UpdateDate).FirstOrDefault();

                if (theExchangeRate != null)
                {
                    results[i].ThatDayExchangeRate = theExchangeRate.Rate;
                }
            }

            #endregion


            Func<ReceiptedListViewModel, object> convert = item => new
            {
                FinanceReceiptID = item.FinanceReceiptID,
                SeqNo = item.SeqNo,
                FinanceAccountID = item.FinanceAccountID,
                Payer = item.Payer,
                FeeType = item.FeeType.GetDescription(),
                Amount = item.Amount,
                Currency = item.Currency,
                ReceiptDate = item.ReceiptDate.ToString("yyyy-MM-dd"),

                FinanceVaultName = item.FinanceVaultName,
                FinanceAccountName = item.FinanceAccountName,
                ThatDayExchangeRate = (item.Currency == "CNY" || item.Currency == "RMB") ? 
                                        "1" : (item.ThatDayExchangeRate != null ? Convert.ToString(item.ThatDayExchangeRate) : ""),

                TariffAmount = item.FeeType == Enums.FinanceFeeType.DepositReceived ? Convert.ToString(item.TariffAmount) : "---",
                AddedValueTaxAmount = item.FeeType == Enums.FinanceFeeType.DepositReceived ? Convert.ToString(item.AddedValueTaxAmount) : "---",
                AgencyFeeAmount = item.FeeType == Enums.FinanceFeeType.DepositReceived ? Convert.ToString(item.AgencyFeeAmount) : "---",
                IncidentalAmount = item.FeeType == Enums.FinanceFeeType.DepositReceived ? Convert.ToString(item.IncidentalAmount) : "---",
                ExciseTaxAmount = item.FeeType == Enums.FinanceFeeType.DepositReceived ? Convert.ToString(item.ExciseTaxAmount) : "---",
                ServiceAmount = item.FeeType == Enums.FinanceFeeType.DepositReceived ? Convert.ToString(item.AgencyFeeAmount + item.IncidentalAmount) : "---",  //服务费

                ProductAmount = item.ProductAmount,
                SwapExchangeRate = item.SwapExchangeRate,
            };

            if (!pageIndex.HasValue && !pageSize.HasValue)
            {
                Func<dynamic, object> convertAgain = item => new
                {
                    item.SeqNo,
                    item.FinanceVaultName,
                    item.FinanceAccountName,
                    item.Payer,
                    item.FeeType,
                    item.Amount,
                    item.Currency,
                    item.ReceiptDate,
                    item.ThatDayExchangeRate,
                    item.ProductAmount,
                    item.SwapExchangeRate,
                    item.TariffAmount,
                    item.AddedValueTaxAmount,
                    item.ExciseTaxAmount,
                    item.ServiceAmount,
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
        /// 根据收款类型查询
        /// </summary>
        /// <param name="feeType"></param>
        /// <returns></returns>
        public ReceiptedListView SearchByFeeType(Enums.FinanceFeeType feeType)
        {
            var linq = from query in this.IQueryable
                       where query.FeeType == feeType
                       select query;

            var view = new ReceiptedListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据付款人查询
        /// </summary>
        /// <param name="payer"></param>
        /// <returns></returns>
        public ReceiptedListView SearchByPayer(string payer)
        {
            var linq = from query in this.IQueryable
                       where query.Payer.Contains(payer)
                       select query;

            var view = new ReceiptedListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据收款日期开始时间查询
        /// </summary>
        /// <param name="begin"></param>
        /// <returns></returns>
        public ReceiptedListView SearchByReceiptDateStartDate(DateTime begin)
        {
            var linq = from query in this.IQueryable
                       where query.ReceiptDate >= begin
                       select query;

            var view = new ReceiptedListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据收款日期结束时间查询
        /// </summary>
        /// <param name="end"></param>
        /// <returns></returns>
        public ReceiptedListView SearchByReceiptDateEndDate(DateTime end)
        {
            var linq = from query in this.IQueryable
                       where query.ReceiptDate < end
                       select query;

            var view = new ReceiptedListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据银行流水号查询
        /// </summary>
        /// <param name="seqNo"></param>
        /// <returns></returns>
        public ReceiptedListView SearchBySeqNo(string seqNo)
        {
            var linq = from query in this.IQueryable
                       where query.SeqNo.Contains(seqNo)
                       select query;

            var view = new ReceiptedListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据FinanceVaultID查询
        /// </summary>
        /// <param name="financeVaultID"></param>
        /// <returns></returns>
        public ReceiptedListView SearchByFinanceVaultID(string financeVaultID)
        {
            var linq = from query in this.IQueryable
                       where query.FinanceVaultID == financeVaultID
                       select query;

            var view = new ReceiptedListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据FinanceAccountID查询
        /// </summary>
        /// <param name="financeAccountID"></param>
        /// <returns></returns>
        public ReceiptedListView SearchByFinanceAccountID(string financeAccountID)
        {
            var linq = from query in this.IQueryable
                       where query.FinanceAccountID == financeAccountID
                       select query;

            var view = new ReceiptedListView(this.Reponsitory, linq);
            return view;
        }

    }

    public class ReceiptedListViewModel
    {
        /// <summary>
        /// FinanceReceiptID
        /// </summary>
        public string FinanceReceiptID { get; set; }

        /// <summary>
        /// 银行流水号
        /// </summary>
        public string SeqNo { get; set; }

        /// <summary>
        /// FinanceVaultID
        /// </summary>
        public string FinanceVaultID { get; set; }

        /// <summary>
        /// 金库名称
        /// </summary>
        public string FinanceVaultName { get; set; }

        /// <summary>
        /// FinanceAccountID
        /// </summary>
        public string FinanceAccountID { get; set; }

        /// <summary>
        /// 账户名称
        /// </summary>
        public string FinanceAccountName { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string Payer { get; set; }

        /// <summary>
        /// 收款类型
        /// </summary>
        public Enums.FinanceFeeType FeeType { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 收款日期
        /// </summary>
        public DateTime ReceiptDate { get; set; }


        /// <summary>
        /// 当天汇率
        /// </summary>
        public decimal? ThatDayExchangeRate { get; set; }

        /// <summary>
        /// 货款
        /// </summary>
        public decimal ProductAmount { get; set; }

        /// <summary>
        /// 换汇汇率
        /// </summary>
        public string SwapExchangeRate { get; set; }

        /// <summary>
        /// 关税
        /// </summary>
        public decimal TariffAmount { get; set; }

        /// <summary>
        /// 增值税
        /// </summary>
        public decimal AddedValueTaxAmount { get; set; }

        /// <summary>
        /// 消费税
        /// </summary>
        public decimal ExciseTaxAmount { get; set; }

        ///// <summary>
        ///// 服务费
        ///// </summary>
        //public decimal ServiceAmount { get; set; }

        /// <summary>
        /// 代理费
        /// </summary>
        public decimal AgencyFeeAmount { get; set; }

        /// <summary>
        /// 杂费
        /// </summary>
        public decimal IncidentalAmount { get; set; }

    }

}
