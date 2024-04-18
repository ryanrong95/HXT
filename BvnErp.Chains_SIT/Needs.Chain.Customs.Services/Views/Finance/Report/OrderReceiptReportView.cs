using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class OrderReceiptReportView : UniqueView<Models.ReceiveDetailReport, ScCustomsReponsitory>
    {
        public OrderReceiptReportView()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        public OrderReceiptReportView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<ReceiveDetailReport> GetIQueryable()
        {

            #region 废弃

            //var payExItemView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>().Where(t => t.Status == (int)Enums.Status.Normal);
            ////var payExView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>().Where(t => t.Status == (int)Enums.Status.Normal);
            //var decheadView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>().Where(t => t.IsSuccess);
            //var ordersView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            ////银行收款记录
            //var financeReceiptsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceReceipts>().Where(t => t.FeeType == (int)Enums.FinanceFeeType.DepositReceived);

            ////订单货款收款
            ////var ProductReceiptsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>().
            ////    Where(t => t.FeeType == (int)Enums.FeeType.Product && t.Type == (int)OrderReceiptType.Received && t.Status == (int)Status.Normal);

            ////订单货款收款,根据 收款ID、付汇、订单汇总
            //var productReceipt = from receipt in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>()
            //                     where receipt.FeeType == (int)Enums.FeeType.Product
            //                     && receipt.Type == (int)OrderReceiptType.Received
            //                     && receipt.Status == (int)Status.Normal
            //                     group receipt by new { receipt.FeeSourceID, receipt.OrderID, receipt.FinanceReceiptID } into groupreceipt
            //                     select new
            //                     {
            //                         FeeSourceID = groupreceipt.Key.FeeSourceID,
            //                         OrderID = groupreceipt.Key.OrderID,
            //                         FinanceReceiptID = groupreceipt.Key.FinanceReceiptID,
            //                         ReceiptAmount = groupreceipt.Sum(t=> -t.Amount)
            //                     };

            ////付汇申请情况
            //var payexchange = from peapplyitem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>()
            //                  join peapply in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>() on peapplyitem.PayExchangeApplyID equals peapply.ID
            //                  where peapplyitem.Status == (int)Enums.Status.Normal
            //                  && peapply.PayExchangeApplyStatus != (int)PayExchangeApplyStatus.Cancled
            //                  && peapply.FatherID == null
            //                  select new
            //                  {
            //                      peapplyitem.OrderID,
            //                      peapplyitem.PayExchangeApplyID,
            //                      peapplyitem.Amount,
            //                      peapply.ExchangeRate
            //                  };


            ////货款收款与付汇申请精确匹配
            //var results = from payExItem in payexchange
            //              join order in ordersView on payExItem.OrderID equals order.ID
            //              join head in decheadView on payExItem.OrderID equals head.OrderID
            //              join productreceipt in productReceipt on new { FeeSourceID = payExItem.PayExchangeApplyID, OrderID = order.ID } equals new { FeeSourceID = productreceipt.FeeSourceID, OrderID = productreceipt.OrderID }
            //              join financereceipt in financeReceiptsView on productreceipt.FinanceReceiptID equals financereceipt.ID
            //              select new ReceiveDetailReport
            //              {
            //                  FinanceReceiptID = financereceipt.ID,
            //                  SeqNo = financereceipt.SeqNo,
            //                  ContractNo = head.ContrNo,
            //                  ReceiptDate = financereceipt.ReceiptDate,
            //                  ProductReceiptAmount = productreceipt.ReceiptAmount,
            //                  DeclPrice = payExItem.Amount,
            //                  DeclRate = payExItem.ExchangeRate,
            //                  DDate = head.DDate.Value,
            //                  DeclareCurrency = order.Currency,
            //              };

            //return results;

            #endregion

            return null;
        }

        /// <summary>
        /// 货款实收明细视图
        /// </summary>
        /// <returns></returns>
        public IQueryable<ReceiveDetailReport> GetProductReceiptDetail(string StartDate, string EndDate, string ReceiptStartDate, string ReceiptEndDate)
        {          
            var decheadView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>().Where(t => t.IsSuccess);
            var ordersView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clientsView = this.Reponsitory.ReadTable < Layer.Data.Sqls.ScCustoms.Clients>();
            var companyView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();
            var agreementView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>();

            var financeReceiptsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceReceipts>().Where(t => t.FeeType == (int)Enums.FinanceFeeType.DepositReceived);
            var productReceiptView = from receipt in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>()
                                     where receipt.FeeType == (int)Enums.FeeType.Product
                                     && receipt.Type == (int)OrderReceiptType.Received
                                     && receipt.Status == (int)Status.Normal
                                     select receipt;

            //银行收款的时间过滤
            if (string.IsNullOrEmpty(StartDate) == false)
            {
                DateTime start = Convert.ToDateTime(StartDate);
                financeReceiptsView = financeReceiptsView.Where(item => item.ReceiptDate >= start);
            }
            if (string.IsNullOrEmpty(EndDate) == false)
            {
                DateTime end = Convert.ToDateTime(EndDate).AddDays(1);
                financeReceiptsView = financeReceiptsView.Where(item => item.ReceiptDate < end);
            }

            //实收款的时间过滤
            if (string.IsNullOrEmpty(ReceiptStartDate) == false)
            {
                DateTime start = Convert.ToDateTime(ReceiptStartDate);
                productReceiptView = productReceiptView.Where(item => item.UpdateDate >= start);               
            }
            if (string.IsNullOrEmpty(ReceiptEndDate) == false)
            {
                DateTime end = Convert.ToDateTime(ReceiptEndDate).AddDays(1);
                productReceiptView = productReceiptView.Where(item => item.UpdateDate < end);
                decheadView = decheadView.Where(item => item.DDate < end);
            }


            //订单货款收款,根据 收款ID、付汇、订单汇总
            var productReceipt = from receipt in productReceiptView
                                 group receipt by new { receipt.FeeSourceID, receipt.OrderID, receipt.FinanceReceiptID } into groupreceipt
                                 select new
                                 {
                                     FeeSourceID = groupreceipt.Key.FeeSourceID,
                                     OrderID = groupreceipt.Key.OrderID,
                                     FinanceReceiptID = groupreceipt.Key.FinanceReceiptID,
                                     ReceiptAmount = groupreceipt.Sum(t => -t.Amount)
                                 };

            //付汇申请情况
            var payexchange = from peapplyitem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>()
                              join peapply in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>() on peapplyitem.PayExchangeApplyID equals peapply.ID
                              where peapplyitem.Status == (int)Enums.Status.Normal
                              && peapply.PayExchangeApplyStatus != (int)PayExchangeApplyStatus.Cancled
                              && peapply.FatherID == null
                              select new
                              {
                                  peapplyitem.OrderID,
                                  peapplyitem.PayExchangeApplyID,
                                  peapplyitem.Amount,
                                  peapply.ExchangeRate
                              };


            //货款收款与付汇申请精确匹配
            var results = from payExItem in payexchange
                          join order in ordersView on payExItem.OrderID equals order.ID
                          join client in clientsView on order.ClientID equals client.ID
                          join company in companyView on client.CompanyID equals company.ID
                          join head in decheadView on payExItem.OrderID equals head.OrderID
                          join agreement in agreementView on order.ClientAgreementID equals agreement.ID
                          join productreceipt in productReceipt on new { FeeSourceID = payExItem.PayExchangeApplyID, OrderID = order.ID } equals new { FeeSourceID = productreceipt.FeeSourceID, OrderID = productreceipt.OrderID }
                          join financereceipt in financeReceiptsView on productreceipt.FinanceReceiptID equals financereceipt.ID
                          select new ReceiveDetailReport
                          {
                              FinanceReceiptID = financereceipt.ID,
                              SeqNo = financereceipt.SeqNo,
                              ContractNo = head.ContrNo,
                              PayExchangeApplyID = payExItem.PayExchangeApplyID,
                              ReceiptDate = financereceipt.ReceiptDate,
                              ProductReceiptAmount = productreceipt.ReceiptAmount,
                              DeclPrice = payExItem.Amount,
                              DeclRate = payExItem.ExchangeRate,
                              DDate = head.DDate.Value,
                              DeclareCurrency = order.Currency,

                              InvoiceType = agreement.InvoiceType,
                              ClientName = company.Name,
                              OrderRealRate = order.RealExchangeRate.Value,
                              Currency = order.Currency
                          };

            return results;
        }

        public IQueryable<ReceiveDetailReport> GetProductReceiptDetailForCredential(string StartDate, string EndDate, string ReceiptStartDate, string ReceiptEndDate)
        {
           
            var decheadView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>().Where(t => t.IsSuccess);
            var ordersView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clientsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companyView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();
            var agreementView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>();

            var financeReceiptsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceReceipts>().Where(t => t.FeeType == (int)Enums.FinanceFeeType.DepositReceived);
            var productReceiptView = from receipt in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>()
                                     where receipt.FeeType == (int)Enums.FeeType.Product
                                     && receipt.Type == (int)OrderReceiptType.Received
                                     && receipt.Status == (int)Status.Normal
                                     select receipt;

            //银行收款的时间过滤
            if (string.IsNullOrEmpty(StartDate) == false)
            {
                DateTime start = Convert.ToDateTime(StartDate);
                financeReceiptsView = financeReceiptsView.Where(item => item.ReceiptDate >= start);
            }
            if (string.IsNullOrEmpty(EndDate) == false)
            {
                DateTime end = Convert.ToDateTime(EndDate).AddDays(1);
                financeReceiptsView = financeReceiptsView.Where(item => item.ReceiptDate < end);
            }

            //实收款的时间过滤
            if (string.IsNullOrEmpty(ReceiptStartDate) == false)
            {
                DateTime start = Convert.ToDateTime(ReceiptStartDate);
                productReceiptView = productReceiptView.Where(item => item.UpdateDate >= start);
            }
            if (string.IsNullOrEmpty(ReceiptEndDate) == false)
            {
                DateTime end = Convert.ToDateTime(ReceiptEndDate).AddDays(1);
                productReceiptView = productReceiptView.Where(item => item.UpdateDate < end);
                decheadView = decheadView.Where(item => item.DDate < end);
            }


            //订单货款收款,根据 收款ID、付汇、订单汇总
            var productReceipt = from receipt in productReceiptView
                                 group receipt by new { receipt.FeeSourceID, receipt.OrderID, receipt.FinanceReceiptID } into groupreceipt
                                 select new
                                 {
                                     FeeSourceID = groupreceipt.Key.FeeSourceID,
                                     OrderID = groupreceipt.Key.OrderID,
                                     FinanceReceiptID = groupreceipt.Key.FinanceReceiptID,
                                     ReceiptAmount = groupreceipt.Sum(t => -t.Amount)
                                 };

            //付汇申请情况
            var payexchange = from peapplyitem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>()
                              join peapply in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>() on peapplyitem.PayExchangeApplyID equals peapply.ID
                              where peapplyitem.Status == (int)Enums.Status.Normal
                              && peapply.PayExchangeApplyStatus != (int)PayExchangeApplyStatus.Cancled
                              && peapply.FatherID == null
                              select new
                              {
                                  peapplyitem.OrderID,
                                  peapplyitem.PayExchangeApplyID,
                                  peapplyitem.Amount,
                                  peapply.ExchangeRate
                              };


            //货款收款与付汇申请精确匹配
            var results = from payExItem in payexchange
                          join order in ordersView on payExItem.OrderID equals order.ID
                          join client in clientsView on order.ClientID equals client.ID
                          join company in companyView on client.CompanyID equals company.ID
                          join head in decheadView on payExItem.OrderID equals head.OrderID
                          join agreement in agreementView on order.ClientAgreementID equals agreement.ID
                          join productreceipt in productReceipt on new { FeeSourceID = payExItem.PayExchangeApplyID, OrderID = order.ID } equals new { FeeSourceID = productreceipt.FeeSourceID, OrderID = productreceipt.OrderID }
                          join financereceipt in financeReceiptsView on productreceipt.FinanceReceiptID equals financereceipt.ID
                          select new ReceiveDetailReport
                          {
                              FinanceReceiptID = financereceipt.ID,
                              SeqNo = financereceipt.SeqNo,
                              ContractNo = head.ContrNo,
                              PayExchangeApplyID = payExItem.PayExchangeApplyID,
                              ReceiptDate = financereceipt.ReceiptDate,
                              ProductReceiptAmount = productreceipt.ReceiptAmount,
                              DeclPrice = payExItem.Amount,
                              DeclRate = payExItem.ExchangeRate,
                              DDate = head.DDate.Value,
                              DeclareCurrency = order.Currency,

                              InvoiceType = agreement.InvoiceType,
                              ClientName = company.Name,
                              OrderRealRate = order.RealExchangeRate.Value,
                              Currency = order.Currency
                          };

            return results;
        }


        /// <summary>
        /// 主表视图
        /// </summary>
        /// <returns></returns>
        public List<ReceiveDetailReportMain> GetFinanceReceiptMain(string StartDate, string EndDate, string ReceiptStartDate, string ReceiptEndDate)
        {

            var receiptView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>();
            var receiptEndView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>();
            var financeRecView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceReceipts>();
            var decHeaaView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>().Where(t => t.IsSuccess == true);

            //银行收款的时间过滤
            if (string.IsNullOrEmpty(StartDate) == false)
            {
                DateTime start = Convert.ToDateTime(StartDate);
                financeRecView = financeRecView.Where(item => item.ReceiptDate >= start);
            }
            if (string.IsNullOrEmpty(EndDate) == false)
            {
                DateTime end = Convert.ToDateTime(EndDate).AddDays(1);
                financeRecView = financeRecView.Where(item => item.ReceiptDate < end);
            }

            //实收款的时间过滤
            if (string.IsNullOrEmpty(ReceiptStartDate) == false)
            {
                DateTime start = Convert.ToDateTime(ReceiptStartDate);
                receiptView = receiptView.Where(item => item.UpdateDate >= start);            
            }
            if (string.IsNullOrEmpty(ReceiptEndDate) == false)
            {
                DateTime end = Convert.ToDateTime(ReceiptEndDate).AddDays(1);
                receiptView = receiptView.Where(item => item.UpdateDate < end);
                receiptEndView = receiptEndView.Where(item => item.UpdateDate < end);
                decHeaaView = decHeaaView.Where(item => item.DDate < end);
            }

            var OrderReceiptsView = (from receipt in receiptView
                                    join dechead in decHeaaView on receipt.OrderID equals dechead.OrderID
                                    join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on receipt.OrderID equals order.ID
                                    join agreement in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>() on order.ClientAgreementID equals agreement.ID
                                    where receipt.Type == (int)Enums.OrderReceiptType.Received 
                                    && receipt.Status == (int)Enums.Status.Normal 
                                    select new
                                    {
                                        ID = receipt.ID,
                                        FinanceReceiptID = receipt.FinanceReceiptID,
                                        FeeType = receipt.FeeType,
                                        Amount = receipt.Amount,
                                        InvoiceType = agreement.InvoiceType
                                    }).ToList();
       
            //分类汇总个费用类型
            var OrderReceipt_Group = (from rec in OrderReceiptsView                                    
                                     group rec by new { rec.FinanceReceiptID, rec.InvoiceType } into rec_g
                                     let IDs = rec_g.Select(t=>t.ID).ToArray()
                                     select new ReceiveDetailReportMain
                                     {
                                         OrderRecepitID = string.Join(",",IDs),
                                         ID = rec_g.Key.FinanceReceiptID,
                                         InvoiceType = rec_g.Key.InvoiceType,
                                         TotalProduct = -rec_g.Where(t => t.FeeType == (int)Enums.OrderFeeType.Product).Sum(t => t.Amount),
                                         TotalAddTax = -rec_g.Where(t => t.FeeType == (int)Enums.OrderFeeType.AddedValueTax).Sum(t => t.Amount),
                                         TotalTariffTax = -rec_g.Where(t => t.FeeType == (int)Enums.OrderFeeType.Tariff).Sum(t => t.Amount),
                                         TotalExciseTax = -rec_g.Where(t => t.FeeType == (int)Enums.OrderFeeType.ExciseTax).Sum(t => t.Amount),
                                         TotalAgency = -rec_g.Where(t => t.FeeType == (int)Enums.OrderFeeType.AgencyFee || t.FeeType == (int)Enums.OrderFeeType.Incidental).Sum(t => t.Amount),

                                     }).ToList();

            //根据收款截止时间统计已收款金额
            var OrderReceiptEndView = (from receipt in receiptEndView
                                      where receipt.Type == (int)Enums.OrderReceiptType.Received
                                      && receipt.Status == (int)Enums.Status.Normal
                                      group receipt by new { receipt.FinanceReceiptID } into grouprec
                                      select new
                                      {
                                          FinanceReceiptID = grouprec.Key.FinanceReceiptID,
                                          ClearAmount = -grouprec.Sum(t => t.Amount)
                                      }).ToList();

            //银行收款记录
            var financeReceiptsView = (from finance in financeRecView
                                      join account in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceAccounts>() on finance.FinanceAccountID equals account.ID
                                      join notice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ReceiptNotices>() on finance.ID equals notice.ID
                                      where finance.FeeType == (int)Enums.FinanceFeeType.DepositReceived
                                      select new
                                      {
                                          ID = finance.ID,
                                          SeqNo = finance.SeqNo,
                                          AccountName = account.AccountName,
                                          ReceiptDate = finance.ReceiptDate,
                                          ClientName = finance.Payer,
                                          Amount = finance.Amount,
                                          ClearAmount = notice.ClearAmount
                                      }).ToList();


            var financeView = from finance in financeReceiptsView
                              join receipt in OrderReceipt_Group on finance.ID equals receipt.ID
                              //没有核销记录的收款财务一律按照预收处理  luyahui 20220526
                              //into receipt_finance
                              //from receipt in receipt_finance.DefaultIfEmpty()
                              //已确认金额 = 累积到ReceiptEndDate的收款金额，不用当前实际的 luyahui 20220610
                              join receiptEnd in OrderReceiptEndView on finance.ID equals receiptEnd.FinanceReceiptID into receipt_end
                              from receiptEnd in receipt_end.DefaultIfEmpty()
                              select new ReceiveDetailReportMain
                              {
                                  OrderRecepitID = receipt.OrderRecepitID,
                                  ID = finance.ID,
                                  SeqNo = finance.SeqNo,
                                  AccountName = finance.AccountName,
                                  ReceiptDate = finance.ReceiptDate,
                                  ClientName = finance.ClientName,
                                  InvoiceType = receipt == null ? 0 : receipt.InvoiceType,
                                  Amount = finance.Amount,
                                  ClearAmount = receiptEnd == null ? 0 : receiptEnd.ClearAmount,
                                  TotalProduct = receipt == null ? 0 : receipt.TotalProduct,
                                  TotalAddTax = receipt == null ? 0 : receipt.TotalAddTax,
                                  TotalTariffTax = receipt == null ? 0 : receipt.TotalTariffTax,
                                  TotalExciseTax = receipt == null ? 0 : receipt.TotalExciseTax,
                                  TotalAgency = receipt == null ? 0 : receipt.TotalAgency,
                              };

            return financeView.ToList();
        }
      

        public List<ReceiveDetailReportMain> GetFinanceReceiptMainForCredential(string StartDate, string EndDate, string ReceiptStartDate, string ReceiptEndDate)
        {

            var receiptView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>();
            var receiptEndView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>();
            var financeRecView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceReceipts>();
            var decHeaaView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>().Where(t => t.IsSuccess == true);

            //银行收款的时间过滤
            if (string.IsNullOrEmpty(StartDate) == false)
            {
                DateTime start = Convert.ToDateTime(StartDate);
                financeRecView = financeRecView.Where(item => item.ReceiptDate >= start);
            }
            if (string.IsNullOrEmpty(EndDate) == false)
            {
                DateTime end = Convert.ToDateTime(EndDate).AddDays(1);
                financeRecView = financeRecView.Where(item => item.ReceiptDate < end);
            }
          
            //实收款的时间过滤
            if (string.IsNullOrEmpty(ReceiptStartDate) == false)
            {
                DateTime start = Convert.ToDateTime(ReceiptStartDate);
                receiptView = receiptView.Where(item => item.UpdateDate >= start);               
            }
            if (string.IsNullOrEmpty(ReceiptEndDate) == false)
            {
                DateTime end = Convert.ToDateTime(ReceiptEndDate).AddDays(1);
                receiptView = receiptView.Where(item => item.UpdateDate < end);
                receiptEndView = receiptEndView.Where(item => item.UpdateDate < end);
                decHeaaView = decHeaaView.Where(item => item.DDate < end);
            }

            var OrderReceiptsView = (from receipt in receiptView
                                    join dechead in decHeaaView on receipt.OrderID equals dechead.OrderID
                                    join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on receipt.OrderID equals order.ID
                                    join agreement in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>() on order.ClientAgreementID equals agreement.ID                                  
                                    where receipt.Type == (int)Enums.OrderReceiptType.Received && receipt.ReImport == false
                                    && receipt.Status == (int)Enums.Status.Normal                                 
                                    select new
                                    {
                                        ID = receipt.ID,
                                        FinanceReceiptID = receipt.FinanceReceiptID,
                                        FeeType = receipt.FeeType,
                                        Amount = receipt.Amount,
                                        InvoiceType = agreement.InvoiceType
                                    }).ToList();
            //分类汇总个费用类型
            var OrderReceipt_Group = (from rec in OrderReceiptsView
                                     group rec by new { rec.FinanceReceiptID, rec.InvoiceType } into rec_g
                                     let IDs = rec_g.Select(t => t.ID).ToArray()
                                      select new ReceiveDetailReportMain
                                     {
                                         OrderRecepitID = string.Join(",", IDs),
                                         ID = rec_g.Key.FinanceReceiptID,
                                         InvoiceType = rec_g.Key.InvoiceType,
                                         TotalProduct = -rec_g.Where(t => t.FeeType == (int)Enums.OrderFeeType.Product).Sum(t => t.Amount),
                                         TotalAddTax = -rec_g.Where(t => t.FeeType == (int)Enums.OrderFeeType.AddedValueTax).Sum(t => t.Amount),
                                         TotalTariffTax = -rec_g.Where(t => t.FeeType == (int)Enums.OrderFeeType.Tariff).Sum(t => t.Amount),
                                         TotalExciseTax = -rec_g.Where(t => t.FeeType == (int)Enums.OrderFeeType.ExciseTax).Sum(t => t.Amount),
                                         TotalAgency = -rec_g.Where(t => t.FeeType == (int)Enums.OrderFeeType.AgencyFee || t.FeeType == (int)Enums.OrderFeeType.Incidental).Sum(t => t.Amount),

                                     }).ToList();

            //根据收款截止时间统计已收款金额
            var OrderReceiptEndView = (from receipt in receiptEndView
                                      where receipt.Type == (int)Enums.OrderReceiptType.Received
                                      && receipt.Status == (int)Enums.Status.Normal
                                      group receipt by new { receipt.FinanceReceiptID } into grouprec
                                      select new
                                      {
                                          FinanceReceiptID = grouprec.Key.FinanceReceiptID,
                                          ClearAmount = -grouprec.Sum(t => t.Amount)
                                      }).ToList();

            //银行收款记录
            var financeReceiptsView = (from finance in financeRecView
                                      join account in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceAccounts>() on finance.FinanceAccountID equals account.ID
                                      join notice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ReceiptNotices>() on finance.ID equals notice.ID
                                      where finance.FeeType == (int)Enums.FinanceFeeType.DepositReceived
                                      select new
                                      {
                                          ID = finance.ID,
                                          SeqNo = finance.SeqNo,
                                          AccountName = account.AccountName,
                                          ReceiptDate = finance.ReceiptDate,
                                          ClientName = finance.Payer,
                                          Amount = finance.Amount,
                                          ClearAmount = notice.ClearAmount
                                      }).ToList();


            var financeView = from finance in financeReceiptsView
                              join receipt in OrderReceipt_Group on finance.ID equals receipt.ID
                              //没有核销记录的收款财务一律按照预收处理  luyahui 20220526
                              //into receipt_finance
                              //from receipt in receipt_finance.DefaultIfEmpty()
                              //已确认金额 = 累积到ReceiptEndDate的收款金额，不用当前实际的 luyahui 20220610
                              join receiptEnd in OrderReceiptEndView on finance.ID equals receiptEnd.FinanceReceiptID into receipt_end
                              from receiptEnd in receipt_end.DefaultIfEmpty()
                              select new ReceiveDetailReportMain
                              {
                                  OrderRecepitID = receipt.OrderRecepitID,
                                  ID = finance.ID,
                                  SeqNo = finance.SeqNo,
                                  AccountName = finance.AccountName,
                                  ReceiptDate = finance.ReceiptDate,
                                  ClientName = finance.ClientName,
                                  InvoiceType = receipt == null ? 0 : receipt.InvoiceType,
                                  Amount = finance.Amount,
                                  ClearAmount = receiptEnd == null ? 0 : receiptEnd.ClearAmount,
                                  TotalProduct = receipt == null ? 0 : receipt.TotalProduct,
                                  TotalAddTax = receipt == null ? 0 : receipt.TotalAddTax,
                                  TotalTariffTax = receipt == null ? 0 : receipt.TotalTariffTax,
                                  TotalExciseTax = receipt == null ? 0 : receipt.TotalExciseTax,
                                  TotalAgency = receipt == null ? 0 : receipt.TotalAgency,
                              };

            return financeView.ToList();
        }


        /// <summary>
        /// 获取货款汇差
        /// </summary>
        /// <returns></returns>
        public List<ReceiveDetailReport> GetProductReceipt(ReceiveDetailReport[] results)
        {
            //var results = this.GetIQueryable().ToArray();

            #region 当天汇率
            //所有需要取汇率的日期
            var date = new List<DateTime>();
            date.AddRange(results.Select(t => t.ReceiptDate.AddDays(1).Date).Distinct().ToArray());
            date.AddRange(results.Select(t => t.DDate.AddDays(1).Date).Distinct().ToArray());

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

            //对每一条数据，先用日志表的值，再用当前实时汇率表中的值
            for (int i = 0; i < results.Length; i++)
            {
                //匹配收款日期汇率
                var theExchangeRateLog = ienums_exchangeRateLog
                        .Where(t => t.Code == results[i].DeclareCurrency
                                 && t.CreateDate.ToString("yyyy-MM-dd") == results[i].ReceiptDate.AddDays(1).ToString("yyyy-MM-dd"))
                        .OrderByDescending(t => t.CreateDate).FirstOrDefault();

                if (theExchangeRateLog != null)
                {
                    results[i].ReceiptRate = theExchangeRateLog.Rate;
                }

                //匹配报关日期汇率
                var theExchangeRateLogDDate = ienums_exchangeRateLog
                        .Where(t => t.Code == results[i].DeclareCurrency
                                 && t.CreateDate.ToString("yyyy-MM-dd") == results[i].DDate.AddDays(1).ToString("yyyy-MM-dd"))
                        .OrderByDescending(t => t.CreateDate).FirstOrDefault();

                if (theExchangeRateLogDDate != null)
                {
                    results[i].DDateRate = theExchangeRateLogDDate.Rate;
                }
            }

            for (int i = 0; i < results.Length; i++)
            {
                //匹配收款日期汇率
                var theExchangeRate = ienums_exchangeRate
                        .Where(t => t.Code == results[i].DeclareCurrency
                                 && t.UpdateDate?.ToString("yyyy-MM-dd") == results[i].ReceiptDate.AddDays(1).ToString("yyyy-MM-dd"))
                        .OrderByDescending(t => t.UpdateDate).FirstOrDefault();

                if (theExchangeRate != null)
                {
                    results[i].ReceiptRate = theExchangeRate.Rate;
                }

                //匹配报关日期汇率
                var theExchangeRateDDate = ienums_exchangeRate
                        .Where(t => t.Code == results[i].DeclareCurrency
                                 && t.UpdateDate?.ToString("yyyy-MM-dd") == results[i].DDate.AddDays(1).ToString("yyyy-MM-dd"))
                        .OrderByDescending(t => t.UpdateDate).FirstOrDefault();

                if (theExchangeRateDDate != null)
                {
                    results[i].DDateRate = theExchangeRateDDate.Rate;
                }


                //处理单双抬头
                if (results[i].InvoiceType == (int)Enums.InvoiceType.Full)
                {
                    results[i].DDateRate = results[i].OrderRealRate;
                    results[i].DeclarePrice = (results[i].DeclPrice * results[i].OrderRealRate).ToRound(2);
                }
                else
                {
                    results[i].DeclarePrice = (results[i].DeclPrice * results[i].DDateRate).ToRound(2);
                }
            }

            //一个付汇多比收款的情况
            var groupTimes = (from result in results
                              group result by new { result.ContractNo, result.PayExchangeApplyID } into groupResult
                              select new
                              {
                                  groupResult.Key.ContractNo,
                                  groupResult.Key.PayExchangeApplyID,
                                  Times = groupResult.Count(),
                              }).Where(t => t.Times > 1).ToArray();

            foreach (var grop in groupTimes)
            {
                var repeat = results.Where(t => t.ContractNo == grop.ContractNo && t.PayExchangeApplyID == grop.PayExchangeApplyID).ToArray();
                var receiptP = 0M;
                for (int i = 0; i < repeat.Length; i++)
                {
                    receiptP += repeat[i].ProductReceiptAmount;
                    repeat[i].ExchangeSpread = 0;
                }
                repeat[repeat.Length - 1].ExchangeSpread = repeat[repeat.Length - 1].DeclarePrice - receiptP;
            }

            //合同号重复，最后一次倒挤
            foreach(var grop in groupTimes)
            {
                var repeat = results.Where(t => t.ContractNo == grop.ContractNo && t.PayExchangeApplyID == grop.PayExchangeApplyID).ToArray();
                decimal totalUSD = repeat[0].DeclPrice;
                decimal totalRmb = repeat[0].DeclarePrice;

                decimal exTotalUSD = 0m;
                decimal exTotalRmb = 0m;
                for(int i = 0; i < repeat.Length - 1; i++)
                {
                    exTotalUSD += repeat[i].USD;
                    exTotalRmb += repeat[i].RMB;
                }

                repeat[repeat.Length - 1].uSD = totalUSD - exTotalUSD;
                repeat[repeat.Length - 1].rMB = totalRmb - exTotalRmb;
            }
           

            #endregion

            return results.ToList();

        }

        public List<string>  GetDeclaredOrders(IQueryable<ReceiveDetailReportMain> results ,string ReceiptStartDate, string ReceiptEndDate) 
        {
            DateTime start = Convert.ToDateTime(ReceiptStartDate);
            DateTime end = Convert.ToDateTime(ReceiptEndDate).AddDays(1);
            var receiptIDs = results.Select(t => t.OrderRecepitID).Distinct().ToArray();
            var decHeadsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>().Where(t=>t.DDate> start && t.DDate< end).ToArray();
            var receiptView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>().Where(t => receiptIDs.Contains(t.ID)).ToArray();
            var declared = (from dechead in decHeadsView
                           join receipt in receiptView on dechead.OrderID equals receipt.OrderID
                           select receipt.ID).ToList();
            return declared;
        }
    }
}
