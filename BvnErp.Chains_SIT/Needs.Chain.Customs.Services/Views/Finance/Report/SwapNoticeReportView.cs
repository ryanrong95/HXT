using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class SwapNoticeReportView : UniqueView<Models.SwapNoticeDetail, ScCustomsReponsitory>
    {
        public SwapNoticeReportView()
        {

        }

        public SwapNoticeReportView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.SwapNoticeDetail> GetIQueryable()
        {
            var decheadsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>().Where(t => t.IsSuccess);
            var ordersView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clientsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companyView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();
            var swapNoticesItemsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticeItems>().Where(t => t.Status == (int)Enums.Status.Normal);
            var swapNoticesView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNotices>();
            var accountsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceAccounts>();

            var linq = from swapItem in swapNoticesItemsView
                       join dechead in decheadsView on swapItem.DecHeadID equals dechead.ID
                       join order in ordersView on dechead.OrderID equals order.ID
                       join client in clientsView on order.ClientID equals client.ID
                       join company in companyView on client.CompanyID equals company.ID
                       join swap in swapNoticesView on swapItem.SwapNoticeID equals swap.ID
                       join account in accountsView on swap.MidFinanceAccountID equals account.ID into swap_Acc
                       from midAcc in swap_Acc.DefaultIfEmpty()
                       where swap.Status == (int)Enums.SwapStatus.Audited || swap.Status == (int)Enums.SwapStatus.PartAudit
                       orderby swap.UpdateDate descending
                       select new Models.SwapNoticeDetail
                       {
                           ID = swapItem.ID,
                           DecHeadID = dechead.ID,
                           SwapNoticeID = swapItem.SwapNoticeID,
                           DDate = dechead.DDate.Value,
                           ClientName = company.Name,
                           SupplierName = dechead.ConsignorCode,
                           ContractNo = dechead.ContrNo,
                           OrderID = dechead.OrderID,
                           EntryID = dechead.EntryId,
                           Currency = order.Currency,
                           DeclPrice = order.DeclarePrice,
                           //DeclarePrice = 
                           SwapPrice = swapItem.Amount.Value,
                           SwapDate = swap.UpdateDate,
                           SwapBank = swap.BankName,
                           SwapBankMiddle = midAcc.AccountName,
                           SwapRate = swap.ExchangeRate.Value
                       };
            return linq;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public List<Models.SwapNoticeDetail> GetNoticeDetailWithDecPrice(Models.SwapNoticeDetail[] results)
        {

            var decheadIDs = results.Select(t => t.DecHeadID).Distinct().ToArray();

            var item_gruop = from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>()
                             where decheadIDs.Contains(item.DeclarationID)
                             group item by item.DeclarationID into item_g
                             select new
                             {
                                 DecHeadID = item_g.Key,
                                 Amount = item_g.Sum(t => t.DeclTotal)
                             };

            var item_arr = item_gruop.ToArray();


            #region 所有需要取汇率的日期
            var date = results.Select(t => t.DDate.AddDays(1).Date).Distinct().ToArray();

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
                                          && date.Contains(exchangeRateLog.CreateDate.Date)
                                       select new
                                       {
                                           Code = exchangeRate.Code,
                                           Rate = exchangeRateLog.Rate,
                                           CreateDate = exchangeRateLog.CreateDate,
                                       };

            var ienums_exchangeRateLog = linq_exchangeRateLog.ToArray();

            #endregion

            //对每一条数据，先用日志表的值，再用当前实时汇率表中的值
            for (int i = 0; i < results.Length; i++)
            {
                #region 匹配报关日期汇率
                var theExchangeRateLogDDate = ienums_exchangeRateLog
                        .Where(t => t.Code == results[i].Currency
                                 && t.CreateDate.ToString("yyyy-MM-dd") == results[i].DDate.AddDays(1).ToString("yyyy-MM-dd"))
                        .OrderByDescending(t => t.CreateDate).FirstOrDefault();

                if (theExchangeRateLogDDate != null)
                {
                    results[i].DDateRate = theExchangeRateLogDDate.Rate;
                }
                else
                {
                    var theExchangeRateDDate = ienums_exchangeRate
                        .Where(t => t.Code == results[i].Currency
                                 && t.UpdateDate?.ToString("yyyy-MM-dd") == results[i].DDate.AddDays(1).ToString("yyyy-MM-dd"))
                        .OrderByDescending(t => t.UpdateDate).FirstOrDefault();

                    if (theExchangeRateDDate != null)
                    {
                        results[i].DDateRate = theExchangeRateDDate.Rate;
                    }
                }
                #endregion

                //顺便给报关金额赋值
                results[i].DeclarePrice = item_arr.Where(t => t.DecHeadID == results[i].DecHeadID).FirstOrDefault().Amount.ToRound(2);
                results[i].DeclPrice = results[i].DeclPrice.ToRound(2);
                results[i].DDate = results[i].DDate.Date;

                //K3账户名词修改
                switch (results[i].SwapBank)
                {
                    case ("星展银行"):
                        results[i].SwapBank = "星展银行（中国）有限公司深圳分行";
                        break;
                    case ("农业银行"):
                        results[i].SwapBank = "中国农业银行股份有限公司深圳免税大厦支行";
                        results[i].SwapBankMiddle = "中国农业银行深圳免税大厦支行美金账户";
                        break;
                    case ("宁波银行"):
                        results[i].SwapBank = "宁波银行深圳分行";
                        results[i].SwapBankMiddle = "宁波银行深圳分行美金账户";
                        break;
                    default:
                        break;
                }
            }

            return results.ToList();
        }

        /// <summary>
        /// 换汇主表
        /// </summary>
        /// <returns></returns>
        public IQueryable<Models.SwapNoticeMain> GetSwapNoticeMain()
        {
            var swapNoticesView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNotices>();
            var accountsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceAccounts>();

            var linq = from swap in swapNoticesView
                       join account in accountsView on swap.MidFinanceAccountID equals account.ID into swap_Acc
                       from midAcc in swap_Acc.DefaultIfEmpty()
                       where swap.Status == (int)Enums.SwapStatus.Audited || swap.Status == (int)Enums.SwapStatus.PartAudit
                       orderby swap.UpdateDate descending
                       select new Models.SwapNoticeMain
                       {
                           SwapNoticeID = swap.ID,
                           ConsignorCode = swap.ConsignorCode,
                           SwapAmount = swap.TotalAmount,
                           SwapRate = swap.ExchangeRate.Value,
                           SwapAmountRMB = swap.TotalAmountCNY.Value,
                           BankName = swap.BankName,
                           SwapBankMiddle = midAcc.AccountName,
                           SwapDate = swap.UpdateDate
                       };

            return linq;
        }



        /// <summary>
        /// 报关单所有ID
        /// </summary>
        /// <param name="DecHeadID"></param>
        /// <returns></returns>
        public List<Models.SwapNoticeItemOrigin> GetLastSwap(string DecHeadID)
        {


            var list = from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticeItems>()
                       where entity.DecHeadID == DecHeadID && entity.Status == (int)Enums.Status.Normal
                       orderby entity.CreateDate descending
                       select new Models.SwapNoticeItemOrigin
                       {
                           ID = entity.ID,
                           SwapNoticeID = entity.SwapNoticeID,
                           DecHeadID = entity.DecHeadID,
                           CreateDate = entity.CreateDate,
                           Amount = entity.Amount ?? 0,
                           //Status = (Enums.Status)entity.Status,
                       };

            return list.ToList();
        }

    }








}
