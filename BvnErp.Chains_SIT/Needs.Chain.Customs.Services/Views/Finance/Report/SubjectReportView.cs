using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class SubjectReportView : UniqueView<Models.SubjectReportItem, ScCustomsReponsitory>
    {
        public SubjectReportView()
        {
        }

        internal SubjectReportView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.SubjectReportItem> GetIQueryable()
        {
            throw new NotImplementedException();
        }



        public IQueryable<Models.SubjectReportItem> GetIQueryableResult(Expression<Func<SubjectReportItem, bool>> expression)
        {
            return null;
        }

        public IEnumerable<Models.SubjectReportItem> GetResult(out int totalCount, int pageIndex, int pageSize, params LambdaExpression[] expressions)
        {
            var decNoticeList = GetList(pageIndex, pageSize, expressions);
            var count = GetCount(expressions);

            #region 客户类型(单抬头、双抬头)

            var clientAgreements = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>();

            var clientAgreementIDs = decNoticeList.Select(t => t.ClientAgreementID).ToArray();

            var linq_clientAgreement = from clientAgreement in clientAgreements
                                       where clientAgreement.Status == (int)Enums.Status.Normal
                                          && clientAgreementIDs.Contains(clientAgreement.ID)
                                       select new
                                       {
                                           ClientAgreementID = clientAgreement.ID,
                                           InvoiceType = (Enums.InvoiceType)clientAgreement.InvoiceType,
                                       };

            var ienums_clientAgreement = linq_clientAgreement.ToArray();

            #endregion

            for (int i = 0; i < decNoticeList.Count; i++)
            {
                decNoticeList[i].InvoiceTypeName = "";

                switch (decNoticeList[i].InvoiceType)
                {
                    case InvoiceType.Full:
                        decNoticeList[i].InvoiceTypeName = "单抬头";
                        break;
                    case InvoiceType.Service:
                        decNoticeList[i].InvoiceTypeName = "双抬头";
                        break;
                    default:
                        break;
                }
            }

            totalCount = count;

            return decNoticeList;
        }

        private List<Models.SubjectReportItem> GetList(int pageIndex, int pageSize, params LambdaExpression[] expressions)
        {
            var decNoticeLists = GetCommon(expressions);
            decNoticeLists = decNoticeLists.Skip(pageSize * (pageIndex - 1)).Take(pageSize);

            var resultArr = decNoticeLists.ToList();

            #region 报关当天汇率池
            //所有需要取汇率的日期
            var date = new List<DateTime>();
            date.AddRange(resultArr.Select(t => t.DeclareDate.Value.AddDays(1).Date).Distinct().ToArray());

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
            for (int i = 0; i < resultArr.Count; i++)
            {
                //匹配报关日期汇率
                var theExchangeRateLogDDate = ienums_exchangeRateLog
                        .Where(t => t.Code == resultArr[i].Currency
                                 && t.CreateDate.ToString("yyyy-MM-dd") == resultArr[i].DeclareDate.Value.AddDays(1).ToString("yyyy-MM-dd"))
                        .OrderByDescending(t => t.CreateDate).FirstOrDefault();

                if (theExchangeRateLogDDate != null)
                {
                    resultArr[i].RealExchangeRate = theExchangeRateLogDDate.Rate;
                }
            }

            for (int i = 0; i < resultArr.Count; i++)
            {
                //匹配报关日期汇率
                var theExchangeRateDDate = ienums_exchangeRate
                        .Where(t => t.Code == resultArr[i].Currency
                                 && t.UpdateDate?.ToString("yyyy-MM-dd") == resultArr[i].DeclareDate.Value.AddDays(1).ToString("yyyy-MM-dd"))
                        .OrderByDescending(t => t.UpdateDate).FirstOrDefault();

                if (theExchangeRateDDate != null)
                {
                    resultArr[i].RealExchangeRate = theExchangeRateDDate.Rate;
                }
            }

            #endregion

            #region 增加开票信息

            var orderids = resultArr.Select(t => t.OrderID).ToArray();

            var invoiceInfo = (from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>()
                               join invoice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNotices>() on item.InvoiceNoticeID equals invoice.ID
                               where invoice.Status == (int)InvoiceNoticeStatus.Confirmed
                               select new
                               {
                                   InvoiceType = (Enums.InvoiceType)invoice.InvoiceType,
                                   InvoiceTaxRate = invoice.InvoiceTaxRate,
                                   InvoiceDate = invoice.UpdateDate,
                                   InvoiceNo = item.InvoiceNo,
                                   OrderID = item.OrderID
                               }).ToList();

            foreach (var dec in resultArr)
            {
                //补充发票信息
                var inv = invoiceInfo.Where(t => t.OrderID.Contains(dec.OrderID))?.FirstOrDefault();
                if (inv != null)
                {
                    dec.InvoiceNo = inv.InvoiceNo;
                    dec.InvoiceDate = inv.InvoiceDate;
                }
            }

            #endregion

            #region 从订单中取客户信息
            var clientIDs = resultArr.Select(t => t.ClientID).ToArray();
            var companyView = new ClientCompanyView(this.Reponsitory).Where(t => clientIDs.Contains(t.ID)).ToArray();
            foreach (var dec in resultArr)
            {
                //补充发票信息
                var inv = companyView.Where(t => t.ID == dec.ClientID)?.FirstOrDefault();
                if (inv != null)
                {
                    dec.ClientName = inv.Company.Name;
                }
            }
            #endregion

            return resultArr;
        }

        private IQueryable<Models.SubjectReportItem> GetCommon(params LambdaExpression[] expressions)
        {
            var heads = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>()
                        join d in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on c.OrderID equals d.ID
                        join e in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>() on new { decID = c.ID, taxType = (int)DecTaxType.Tariff } equals new { decID = e.DecTaxID, taxType = e.TaxType }
                        into f
                        from tariff in f.DefaultIfEmpty()
                        join exciseTax in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>() on new { decID = c.ID, taxType = (int)DecTaxType.ExciseTax } equals new { decID = exciseTax.DecTaxID, taxType = exciseTax.TaxType }
                        into exciseTaxes
                        from exciseTax in exciseTaxes.DefaultIfEmpty()
                        join h in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>() on new { decID = c.ID, taxType = (int)DecTaxType.AddedValueTax } equals new { decID = h.DecTaxID, taxType = h.TaxType }
                        into j
                        from addedValue in j.DefaultIfEmpty()
                        select new SubjectReportItem
                        {
                            ID = c.ID,
                            OrderID = d.ID,
                            DeclareDate = c.DDate,
                            ContrNo = c.ContrNo,
                            ConsignorCode = c.ConsignorCode,
                            RealExchangeRate = d.RealExchangeRate,
                            CustomsExchangeRate = d.CustomsExchangeRate,
                            DecTotalPrice = d.DeclarePrice,
                            ActualTariff = tariff.Amount,
                            ActualExciseTax = exciseTax.Amount,
                            ActualAddedValueTax = addedValue.Amount,
                            ClientName = c.OwnerName,
                            ClientAgreementID = d.ClientAgreementID,
                            Currency = d.Currency,
                            ClientID = d.ClientID,
                        };

            foreach (var expression in expressions)
            {
                heads = heads.Where(expression as Expression<Func<Needs.Ccs.Services.Models.SubjectReportItem, bool>>);
            }



            var items = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>()
                        join d in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>() on c.OrderItemID equals d.ID
                        join tariff in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>() on c.OrderItemID equals tariff.OrderItemID
                        where tariff.Type == (int)CustomsRateType.ImportTax
                        group new { c, d, tariff } by new { c.DeclarationID } into g
                        select new
                        {
                            ID = g.Key.DeclarationID,
                            Tariff = g.Sum(t => t.tariff.Value),
                            DecForeignTotal = g.Sum(t => t.c.DeclTotal)
                        };

            var data = from c in heads
                       join d in items on c.ID equals d.ID
                       orderby c.DeclareDate descending
                       select new SubjectReportItem
                       {
                           ID = c.ID,
                           OrderID = c.OrderID,
                           DeclareDate = c.DeclareDate,
                           ContrNo = c.ContrNo,
                           ConsignorCode = c.ConsignorCode,
                           RealExchangeRate = c.RealExchangeRate,
                           CustomsExchangeRate = c.CustomsExchangeRate,
                           DecTotalPrice = c.DecTotalPrice,
                           DecForeignTotal = d.DecForeignTotal.ToRound(2),
                           ActualTariff = c.ActualTariff,
                           ActualExciseTax = c.ActualExciseTax,
                           ActualAddedValueTax = c.ActualAddedValueTax,
                           Tariff = d.Tariff,
                           ClientName = c.ClientName,
                           Currency = c.Currency,
                           ClientID = c.ClientID,
                       };

            return data;
        }

        private int GetCount(params LambdaExpression[] expressions)
        {
            return GetCommon(expressions).Count();
        }

        public List<Models.SubjectReportItem> GetDownload(params LambdaExpression[] expressions)
        {
            var heads = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>()
                        join d in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on c.OrderID equals d.ID
                        join e in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>() on new { decID = c.ID, taxType = (int)DecTaxType.Tariff } equals new { decID = e.DecTaxID, taxType = e.TaxType }
                        into f
                        from tariff in f.DefaultIfEmpty()
                        join exciseTax in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>() on new { decID = c.ID, taxType = (int)DecTaxType.ExciseTax } equals new { decID = exciseTax.DecTaxID, taxType = exciseTax.TaxType }
                        into exciseTaxes
                        from exciseTax in exciseTaxes.DefaultIfEmpty()
                        join h in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>() on new { decID = c.ID, taxType = (int)DecTaxType.AddedValueTax } equals new { decID = h.DecTaxID, taxType = h.TaxType }
                        into j
                        from addedValue in j.DefaultIfEmpty()

                        join clientAgreement in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>()
                        on d.ClientAgreementID equals clientAgreement.ID
                        select new SubjectReportItem
                        {
                            ID = c.ID,
                            OrderID = c.OrderID,
                            DeclareDate = c.DDate,
                            ContrNo = c.ContrNo,
                            ConsignorCode = c.ConsignorCode,
                            RealExchangeRate = d.RealExchangeRate,
                            CustomsExchangeRate = d.CustomsExchangeRate,
                            DecTotalPrice = d.DeclarePrice,
                            ActualTariff = tariff.Amount,
                            ActualExciseTax = exciseTax.Amount,
                            ActualAddedValueTax = addedValue.Amount,
                            ClientName = c.OwnerName,
                            InvoiceType = (Enums.InvoiceType)clientAgreement.InvoiceType,
                            Currency = d.Currency,
                            ClientID = d.ClientID,
                        };

            foreach (var expression in expressions)
            {
                heads = heads.Where(expression as Expression<Func<Needs.Ccs.Services.Models.SubjectReportItem, bool>>);
            }


            var items = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>()
                        join d in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>() on c.OrderItemID equals d.ID
                        join tariff in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>() on c.OrderItemID equals tariff.OrderItemID
                        where tariff.Type == (int)CustomsRateType.ImportTax
                        group new { c, d, tariff } by new { c.DeclarationID } into g
                        select new
                        {
                            ID = g.Key.DeclarationID,
                            Tariff = g.Sum(t => t.tariff.Value),
                            DecForeignTotal = g.Sum(t => t.c.DeclTotal)
                        };

            var data = (from c in heads
                        join d in items on c.ID equals d.ID
                        select new SubjectReportItem
                        {
                            ID = c.ID,
                            OrderID = c.OrderID,
                            DeclareDate = c.DeclareDate,
                            ContrNo = c.ContrNo,
                            DecForeignTotal = d.DecForeignTotal,
                            ConsignorCode = c.ConsignorCode,
                            RealExchangeRate = c.RealExchangeRate,
                            CustomsExchangeRate = c.CustomsExchangeRate,
                            DecTotalPrice = c.DecTotalPrice,
                            ActualTariff = c.ActualTariff,
                            ActualExciseTax = c.ActualExciseTax,
                            ActualAddedValueTax = c.ActualAddedValueTax,
                            Tariff = d.Tariff,
                            ClientName = c.ClientName,
                            ClientAgreementID = c.ClientAgreementID,
                            InvoiceType = c.InvoiceType,
                            Currency = c.Currency,
                            ClientID = c.ClientID,
                        }).ToList();


            //增加开票信息
            var orderids = data.Select(t => t.OrderID).ToArray();

            #region 报关当天汇率池
            //所有需要取汇率的日期
            var date = new List<DateTime>();
            date.AddRange(data.Select(t => t.DeclareDate.Value.AddDays(1).Date).Distinct().ToArray());

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
            for (int i = 0; i < data.Count; i++)
            {
                //匹配报关日期汇率
                var theExchangeRateLogDDate = ienums_exchangeRateLog
                        .Where(t => t.Code == data[i].Currency
                                 && t.CreateDate.ToString("yyyy-MM-dd") == data[i].DeclareDate.Value.AddDays(1).ToString("yyyy-MM-dd"))
                        .OrderByDescending(t => t.CreateDate).FirstOrDefault();

                if (theExchangeRateLogDDate != null)
                {
                    data[i].RealExchangeRate = theExchangeRateLogDDate.Rate;
                }
            }

            for (int i = 0; i < data.Count; i++)
            {
                //匹配报关日期汇率
                var theExchangeRateDDate = ienums_exchangeRate
                        .Where(t => t.Code == data[i].Currency
                                 && t.UpdateDate?.ToString("yyyy-MM-dd") == data[i].DeclareDate.Value.AddDays(1).ToString("yyyy-MM-dd"))
                        .OrderByDescending(t => t.UpdateDate).FirstOrDefault();

                if (theExchangeRateDDate != null)
                {
                    data[i].RealExchangeRate = theExchangeRateDDate.Rate;
                }
            }

            #endregion




            var invoiceInfo = (from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>()
                               join invoice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNotices>() on item.InvoiceNoticeID equals invoice.ID
                               where invoice.Status == (int)InvoiceNoticeStatus.Confirmed
                               select new
                               {
                                   InvoiceType = (Enums.InvoiceType)invoice.InvoiceType,
                                   InvoiceTaxRate = invoice.InvoiceTaxRate,
                                   InvoiceDate = invoice.UpdateDate,
                                   InvoiceNo = item.InvoiceNo,
                                   OrderID = item.OrderID
                               }).ToList();

            foreach (var dec in data)
            {
                var inv = invoiceInfo.Where(t => t.OrderID.Contains(dec.OrderID))?.FirstOrDefault();
                if (inv != null)
                {
                    dec.InvoiceNo = inv.InvoiceNo;
                    dec.InvoiceDate = inv.InvoiceDate;
                }
            }

            var companyView = new ClientCompanyView(this.Reponsitory).ToArray();
            foreach (var dec in data)
            {
                //补充发票信息
                var inv = companyView.Where(t => t.ID == dec.ClientID)?.FirstOrDefault();
                if (inv != null)
                {
                    dec.ClientName = inv.Company.Name;
                }
            }

            return data;
        }

    }



    public class SubjectReportViewNew : UniqueView<Models.SubjectReportItem, ScCustomsReponsitory>
    {
        public SubjectReportViewNew()
        {
        }

        internal SubjectReportViewNew(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected SubjectReportViewNew(ScCustomsReponsitory reponsitory, IQueryable<SubjectReportItem> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<SubjectReportItem> GetIQueryable()
        {

            var decHeadsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
            var ordersView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clientsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companyView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();

            var iQuery = from head in decHeadsView
                         join order in ordersView on head.OrderID equals order.ID
                         join client in clientsView on order.ClientID equals client.ID
                         join company in companyView on client.CompanyID equals company.ID
                         where head.IsSuccess
                         select new SubjectReportItem
                         {
                             ID = head.ID,
                             OrderID = head.OrderID,
                             DeclareDate = head.DDate,
                             ContrNo = head.ContrNo,
                             ConsignorCode = head.ConsignorCode,
                             //RealExchangeRate = d.RealExchangeRate,
                             CustomsExchangeRate = head.CustomsExchangeRate,
                             DecTotalPrice = order.DeclarePrice,
                             //ActualTariff = tariff.Amount,
                             //ActualExciseTax = exciseTax.Amount,
                             //ActualAddedValueTax = addedValue.Amount,
                             ClientName = company.Name,
                             //InvoiceType = (Enums.InvoiceType)clientAgreement.InvoiceType,
                             Currency = order.Currency,
                             ClientID = order.ClientID,
                             ClientAgreementID = order.ClientAgreementID
                         };

            return iQuery;
        }


        public object ToMyPage(int? pageIndex = null, int? pageSize = null, bool IsStatistic = false)
        {

            IQueryable<Models.SubjectReportItem> iquery = this.IQueryable.Cast<Models.SubjectReportItem>().OrderByDescending(item => item.ID);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_mySubs = iquery.ToArray();

            var orderIDs = ienum_mySubs.Select(item => item.OrderID);
            var decheadIDs = ienum_mySubs.Select(item => item.ID);
            var agreeIDs = ienum_mySubs.Select(item => item.ClientAgreementID);


            //表体
            var declistView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>();
            var traffView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>();

            var linq_declist = from list in declistView
                               join traff in traffView on list.OrderItemID equals traff.OrderItemID
                               where traff.Type == (int)CustomsRateType.ImportTax
                               && orderIDs.Contains(list.OrderID)
                               select new
                               {
                                   OrderID = list.OrderID,
                                   list.DeclTotal,
                                   DecHeadID = list.DeclarationID,
                                   traff.Rate
                               };
            var arry_declist = linq_declist.ToArray();

            //需要通关型号计算的一些金额
            var linq_models = from head in ienum_mySubs
                              join list in arry_declist on head.ID equals list.DecHeadID
                              group new { head, list } by new { head.ID } into grop
                              select new
                              {
                                  ID = grop.Key.ID,
                                  DecTotalPriceRMB = grop.Sum(t => Math.Round(t.list.DeclTotal * t.head.CustomsExchangeRate.Value, 2, MidpointRounding.AwayFromZero)),
                                  DecForeignTotal = grop.Sum(t => t.list.DeclTotal),
                                  DutiablePrice = grop.Sum(t => Math.Round(t.list.DeclTotal * t.head.CustomsExchangeRate.Value, 2, MidpointRounding.AwayFromZero) + Math.Round(Math.Round(t.list.DeclTotal * t.head.CustomsExchangeRate.Value, 2, MidpointRounding.AwayFromZero) * t.list.Rate, 2, MidpointRounding.AwayFromZero)),
                                  Tariff = grop.Sum(t => Math.Round(Math.Round(t.list.DeclTotal * t.head.CustomsExchangeRate.Value, 2, MidpointRounding.AwayFromZero) * t.list.Rate, 2, MidpointRounding.AwayFromZero))
                              };
            var arry_models = linq_models.ToArray();

            //缴税流水
            var linq_taxflow = from taxflow in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>()
                               where decheadIDs.Contains(taxflow.DecTaxID)
                               select taxflow;
            var arry_taxflow = linq_taxflow.ToArray();
            var arry_traffflow = arry_taxflow.Where(t => t.TaxType == (int)DecTaxType.Tariff).ToArray();
            var arry_exciseflow = arry_taxflow.Where(t => t.TaxType == (int)DecTaxType.ExciseTax).ToArray();
            var arry_addedflow = arry_taxflow.Where(t => t.TaxType == (int)DecTaxType.AddedValueTax).ToArray();


            //协议
            var linq_agreement = from agree in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>()
                                 where agreeIDs.Contains(agree.ID)
                                 select agree;
            var arry_agreement = linq_agreement.ToArray();


            //

            #region 报关当天汇率池
            //所有需要取汇率的日期
            var date = new List<DateTime>();
            date.AddRange(ienum_mySubs.Select(t => t.DeclareDate.Value.AddDays(1).Date).Distinct().ToArray());

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
            for (int i = 0; i < ienum_mySubs.Count(); i++)
            {
                //匹配报关日期汇率
                var theExchangeRateLogDDate = ienums_exchangeRateLog
                        .Where(t => t.Code == ienum_mySubs[i].Currency
                                 && t.CreateDate.ToString("yyyy-MM-dd") == ienum_mySubs[i].DeclareDate.Value.AddDays(1).ToString("yyyy-MM-dd"))
                        .OrderByDescending(t => t.CreateDate).FirstOrDefault();

                if (theExchangeRateLogDDate != null)
                {
                    ienum_mySubs[i].RealExchangeRate = theExchangeRateLogDDate.Rate;
                }
                else if(ienum_mySubs[i].Currency == "CNY")
                {
                    ienum_mySubs[i].RealExchangeRate = 1;
                }
            }

            for (int i = 0; i < ienum_mySubs.Count(); i++)
            {
                //匹配报关日期汇率
                var theExchangeRateDDate = ienums_exchangeRate
                        .Where(t => t.Code == ienum_mySubs[i].Currency
                                 && t.UpdateDate?.ToString("yyyy-MM-dd") == ienum_mySubs[i].DeclareDate.Value.AddDays(1).ToString("yyyy-MM-dd"))
                        .OrderByDescending(t => t.UpdateDate).FirstOrDefault();

                if (theExchangeRateDDate != null)
                {
                    ienum_mySubs[i].RealExchangeRate = theExchangeRateDDate.Rate;
                }
            }

            #endregion

            //发票
            //var invoiceInfo = (from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>()
            //                   join invoice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNotices>() on item.InvoiceNoticeID equals invoice.ID
            //                   where invoice.Status == (int)InvoiceNoticeStatus.Confirmed
            //                   select new
            //                   {
            //                       InvoiceType = (Enums.InvoiceType)invoice.InvoiceType,
            //                       InvoiceTaxRate = invoice.InvoiceTaxRate,
            //                       InvoiceDate = invoice.UpdateDate,
            //                       InvoiceNo = item.InvoiceNo,
            //                       OrderID = item.OrderID
            //                   }).ToList();

            //foreach (var dec in ienum_mySubs)
            //{
            //    var inv = invoiceInfo.Where(t => t.OrderID.Contains(dec.OrderID));
            //    if (inv != null && inv.Count() > 0)
            //    {
            //        dec.InvoiceNo = string.Join(",", inv.Select(t=>t.InvoiceNo).Distinct().ToArray());
            //        dec.InvoiceDate = inv.FirstOrDefault().InvoiceDate;
            //    }
            //}


            //
            var results = from head in ienum_mySubs
                          join model in arry_models on head.ID equals model.ID
                          join agree in arry_agreement on head.ClientAgreementID equals agree.ID
                          join taxflow in arry_traffflow on head.ID equals taxflow.DecTaxID into tax_temp
                          from taxflow in tax_temp.DefaultIfEmpty()
                          join excise in arry_exciseflow on head.ID equals excise.DecTaxID into excise_temp
                          from excise in excise_temp.DefaultIfEmpty()
                          join addedflow in arry_addedflow on head.ID equals addedflow.DecTaxID into added_temp
                          from addedflow in added_temp.DefaultIfEmpty()
                          select new SubjectReportItem
                          {
                              ID = head.ID,
                              OrderID = head.OrderID,
                              DeclareDate = head.DeclareDate,
                              ContrNo = head.ContrNo,
                              DecForeignTotal = model.DecForeignTotal,
                              DecTotalPriceRMB = model.DecTotalPriceRMB,
                              ConsignorCode = head.ConsignorCode,
                              RealExchangeRate = head.RealExchangeRate,
                              CustomsExchangeRate = head.CustomsExchangeRate,
                              DecTotalPrice = Math.Round(head.DecTotalPrice, 2, MidpointRounding.AwayFromZero),
                              DutiablePrice = model.DutiablePrice,
                              ActualTariff = taxflow == null ? 0 : taxflow.Amount,
                              ActualExciseTax = excise == null ? 0 : excise.Amount,
                              ActualAddedValueTax = addedflow == null ? 0 : addedflow.Amount,
                              Tariff = model.Tariff,
                              ClientName = head.ClientName,
                              //ClientAgreementID = c.ClientAgreementID,
                              InvoiceType = (InvoiceType)agree.InvoiceType,
                              Currency = head.Currency,
                              ClientID = head.ClientID,
                          };

            Func<Needs.Ccs.Services.Models.SubjectReportItem, object> convert = t => new
            {
                DeclareDate = t.DeclareDate?.ToString("yyyy-MM-dd"),
                ContrNo = t.ContrNo,
                DecForeignTotal = t.DecForeignTotal,
                DecAgentTotal = t.DecAgentTotal,
                DecYunBaoZaTotal = t.DecYunBaoZaTotal,
                DecTotalPriceRMB = t.DecTotalPriceRMB,
                ImportPrice = t.ImportPrice,
                SalePrice = t.SalePrice,
                Tariff = t.Tariff,
                ActualExciseTax = t.ActualExciseTax == null ? 0 : t.ActualExciseTax.Value,
                ActualAddedValueTax = t.ActualAddedValueTax == null ? 0 : t.ActualAddedValueTax.Value,
                ExchangeCustomer = t.ExchangeCustomer,
                ExchangeXDT = t.ExchangeXDT,
                RealExchangeRate = t.RealExchangeRate,
                DueCustomerFC = t.DueCustomerFC,
                DueCustomerRMB = t.DueCustomerRMB,
                DueXDTFC = t.DueXDTFC,
                DueXDTRMB = t.DueXDTRMB,
                ActualTariff = t.ActualTariff == null ? 0 : t.ActualTariff.Value,
                DutiablePrice = t.DutiablePrice,
                ClientName = t.ClientName,
                InvoiceTypeName = t.InvoiceType.GetDescription(),
                ConsignorCode = t.ConsignorCode,
                //InvoiceNo = string.IsNullOrEmpty(t.InvoiceNo) ? "-" : t.InvoiceNo,
                //InvoiceDate = t.InvoiceDate == null ? "-" : t.InvoiceDate?.ToString("yyyy-MM-dd")

            };


            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                //return results.Select(item =>
                //{
                //    object o = item;
                //    return o;
                //}).ToArray();
                if (!IsStatistic)
                {
                    return results.Select(convert).ToArray();
                }
                else
                {
                    //var temp = results.Select(convert).ToArray();
                    var group_result = from sub in results
                                       group sub by new { sub.ClientName, DeclareDate = sub.DeclareDate?.ToString("yyyy-MM-dd"), InvoiceTypeName = sub.InvoiceType.GetDescription(), sub.ConsignorCode, sub.Currency, sub.RealExchangeRate } into grop
                                       let IDs = grop.Select(t => t.ID).ToArray()
                                       select new SubjectReportStatistics
                                       {
                                           DecHeadIDs = string.Join(",",IDs),
                                           DeclareDate = grop.Key.DeclareDate,//报关日期
                                           DecTotalPriceRMB = grop.Sum(t => t.DecTotalPriceRMB),//报关总价
                                           ImportPrice = grop.Sum(t => t.ImportPrice),//委托报关RMB ->进口
                                           //运保杂 = 报关总价 - 进口
                                           Tariff = grop.Sum(t => t.Tariff.Value),
                                           ActualTariff = grop.Sum(t => t.ActualTariff.Value),
                                           //消费税 空白
                                           ActualExciseTax = grop.Sum(t => t.ActualExciseTax.Value),//实缴消费税
                                           ActualAddedValueTax = grop.Sum(t => t.ActualAddedValueTax.Value),//实缴增值税

                                           //合计委托外币 for 委托金额-汇
                                           TotalAgentAmount = grop.Sum(t => t.DecTotalPrice),
                                           //Round（客户当天的外币合计  * 实时汇率，2） +  报关单实交关税（合计）  -  库存进口（合计） - 库存应交关税（合计）
                                           //ExchangeCustomer = grop.Sum(t => t.ExchangeCustomer),//委托金额-汇兑 -> 汇兑客户

                                           ClientName = grop.Key.ClientName,//公司 -> 客户名称
                                           //ExchangeXDT = grop.Sum(t => t.ExchangeXDT),//运保杂-汇兑 -> 汇兑-芯达通
                                           Currency = grop.Key.Currency,
                                           RealExchangeRate = grop.Key.RealExchangeRate.Value,//实时汇率
                                           DecAgentTotal = grop.Sum(t => t.DecAgentTotal),//委托金额
                                           ConsignorCode = grop.Key.ConsignorCode,//物流放公司
                                           DecForeignTotal = grop.Sum(t => t.DecForeignTotal),//报关金额
                                           DecYunBaoZaTotal = grop.Sum(t => t.DecYunBaoZaTotal),//运保杂-usd = 报关金额 - 委托金额
                                           InvoiceTypeName = grop.Key.InvoiceTypeName
                                       };
                    return group_result.OrderBy(t=>t.DeclareDate).OrderByDescending(t=>t.InvoiceTypeName).OrderBy(t=>t.ClientName).ToArray();
                }

            }
            else
            {
                return new
                {
                    total = total,
                    Size = pageSize ?? 20,
                    Index = pageIndex ?? 1,
                    rows = results.Select(convert).ToArray(),
                };
            }
        }

        /// <summary>
        /// 客户名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SubjectReportViewNew SearchByName(string name)
        {
            var linq = from query in this.IQueryable
                       where query.ClientName.Contains(name)
                       select query;

            var view = new SubjectReportViewNew(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 合同号
        /// </summary>
        /// <param name="contrNo"></param>
        /// <returns></returns>
        public SubjectReportViewNew SearchByContrNo(string contrNo)
        {
            var linq = from query in this.IQueryable
                       where query.ContrNo.Contains(contrNo)
                       select query;

            var view = new SubjectReportViewNew(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 报关开始日期
        /// </summary>
        /// <param name="fromtime"></param>
        /// <returns></returns>
        public SubjectReportViewNew SearchByFrom(DateTime fromtime)
        {
            var linq = from query in this.IQueryable
                       where query.DeclareDate >= fromtime
                       select query;

            var view = new SubjectReportViewNew(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 报关结束日期
        /// </summary>
        /// <param name="totime"></param>
        /// <returns></returns>
        public SubjectReportViewNew SearchByTo(DateTime totime)
        {
            var linq = from query in this.IQueryable
                       where query.DeclareDate < totime
                       select query;

            var view = new SubjectReportViewNew(this.Reponsitory, linq);
            return view;
        }


    }
}
