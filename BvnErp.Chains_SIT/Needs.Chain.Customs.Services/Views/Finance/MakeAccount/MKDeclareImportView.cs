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

namespace Needs.Ccs.Services.Views
{
    public class MKDeclareImportView : UniqueView<Models.SubjectReportItem, ScCustomsReponsitory>
    {

        public MKDeclareImportView()
        {
        }

        internal MKDeclareImportView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected MKDeclareImportView(ScCustomsReponsitory reponsitory, IQueryable<SubjectReportItem> iQueryable) : base(reponsitory, iQueryable)
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
                         where head.IsSuccess && head.MKImport == false
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
                else if (ienum_mySubs[i].Currency == "CNY")
                {
                    ienum_mySubs[i].RealExchangeRate = 1;
                }
            }

            for (int i = 0; i < ienum_mySubs.Count(); i++)
            {
                //匹配报关日期汇率
                var theExchangeRateDDate = ienums_exchangeRate
                        .Where(t => t.Code == ienum_mySubs[i].Currency
                                 && t.UpdateDate?.ToString("yyyy-MM-dd") == ienum_mySubs[i].DeclareDate.Value.ToString("yyyy-MM-dd"))
                        .OrderByDescending(t => t.UpdateDate).FirstOrDefault();

                if (theExchangeRateDDate != null)
                {
                    ienum_mySubs[i].RealExchangeRate = theExchangeRateDDate.Rate;
                }
            }

            #endregion

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

                //var temp = results.Select(convert).ToArray();
                var group_result = from sub in results
                                   group sub by new { sub.ClientName, DeclareDate = sub.DeclareDate?.ToString("yyyy-MM-dd"), InvoiceTypeName = sub.InvoiceType.GetDescription(), sub.ConsignorCode, sub.Currency, sub.RealExchangeRate } into grop
                                   let IDs = grop.Select(t => t.ID).ToArray()                                
                                   select new SubjectReportStatistics
                                   {
                                       DecHeadIDs = string.Join(",", IDs),
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

                return new
                {
                    total = total,
                    Size = pageSize ?? 20,
                    Index = pageIndex ?? 1,
                    rows = group_result.OrderBy(t => t.DeclareDate).ToArray()
                };
            
        }

        public object ForCredentialData()
        {

            IQueryable<Models.SubjectReportItem> iquery = this.IQueryable.Cast<Models.SubjectReportItem>().OrderByDescending(item => item.ID);
            int total = iquery.Count();

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
                else if (ienum_mySubs[i].Currency == "CNY")
                {
                    ienum_mySubs[i].RealExchangeRate = 1;
                }
            }

            for (int i = 0; i < ienum_mySubs.Count(); i++)
            {
                //匹配报关日期汇率
                var theExchangeRateDDate = ienums_exchangeRate
                        .Where(t => t.Code == ienum_mySubs[i].Currency
                                 && t.UpdateDate?.ToString("yyyy-MM-dd") == ienum_mySubs[i].DeclareDate.Value.ToString("yyyy-MM-dd"))
                        .OrderByDescending(t => t.UpdateDate).FirstOrDefault();

                if (theExchangeRateDDate != null)
                {
                    ienum_mySubs[i].RealExchangeRate = theExchangeRateDDate.Rate;
                }
            }

            #endregion

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

            //var temp = results.Select(convert).ToArray();
            var group_result = from sub in results
                               group sub by new { sub.ClientName, DeclareDate = sub.DeclareDate?.ToString("yyyy-MM-dd"), InvoiceTypeName = sub.InvoiceType.GetDescription(), sub.ConsignorCode, sub.Currency, sub.RealExchangeRate } into grop
                               let IDs = grop.Select(t => t.ID).ToArray()
                               select new SubjectReportStatistics
                               {
                                   DecHeadIDs = string.Join(",", IDs),
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

            return group_result;

        }

        /// <summary>
        /// 客户名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public MKDeclareImportView SearchByName(string name)
        {
            var linq = from query in this.IQueryable
                       where query.ClientName.Contains(name)
                       select query;

            var view = new MKDeclareImportView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 合同号
        /// </summary>
        /// <param name="contrNo"></param>
        /// <returns></returns>
        public MKDeclareImportView SearchByInvoiceType(InvoiceType type)
        {
            var linq = from query in this.IQueryable
                       where query.InvoiceType == type
                       select query;

            var view = new MKDeclareImportView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 报关开始日期
        /// </summary>
        /// <param name="fromtime"></param>
        /// <returns></returns>
        public MKDeclareImportView SearchByFrom(DateTime fromtime)
        {
            var linq = from query in this.IQueryable
                       where query.DeclareDate >= fromtime
                       select query;

            var view = new MKDeclareImportView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 报关结束日期
        /// </summary>
        /// <param name="totime"></param>
        /// <returns></returns>
        public MKDeclareImportView SearchByTo(DateTime totime)
        {
            var linq = from query in this.IQueryable
                       where query.DeclareDate < totime
                       select query;

            var view = new MKDeclareImportView(this.Reponsitory, linq);
            return view;
        }

    }
}
