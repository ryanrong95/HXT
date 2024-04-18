using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 应收账款双抬头货款列表视图
    /// </summary>
    public class DualProductListView : QueryView<DualProductListViewModel, ScCustomsReponsitory>
    {
        public DualProductListView()
        {
        }

        protected DualProductListView(ScCustomsReponsitory reponsitory, IQueryable<DualProductListViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<DualProductListViewModel> GetIQueryable()
        {
            var decHeads = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
            var decTaxs = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxs>();

            var iQuery = from decHead in decHeads
                         join decTax in decTaxs on decHead.ID equals decTax.ID
                         where decHead.IsSuccess == true
                            && decHead.CusDecStatus != "04"
                            && decTax.InvoiceType == (int)Enums.InvoiceType.Service
                         orderby decHead.DDate descending
                         select new DualProductListViewModel
                         {
                             DecHeadID = decHead.ID,
                             OrderID = decHead.OrderID,
                             OwnerName = decHead.OwnerName,
                             DDate = decHead.DDate,
                             ContractNo = decHead.ContrNo,
                             ConsignorCode = decHead.ConsignorCode
                         };

            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<DualProductListViewModel> iquery = this.IQueryable.Cast<DualProductListViewModel>().OrderByDescending(item => item.DDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_myDecHead = iquery.ToArray();

            //DecHeadID
            var decHeadIDs = ienum_myDecHead.Select(item => item.DecHeadID);

            //OrderID
            var orderIDs = ienum_myDecHead.Select(item => item.OrderID);

            #region 币种、委托金额、海关汇率

            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();

            var linq_order = from order in orders
                             where order.Status == (int)Enums.Status.Normal && orderIDs.Contains(order.ID)
                             select new
                             {
                                 OrderID = order.ID,
                                 Currency = order.Currency,
                                 DeclarePrice = order.DeclarePrice,
                                 CustomsExchangeRate = order.CustomsExchangeRate,
                             };

            var ienums_order = linq_order.ToArray();

            #endregion

            #region 报关金额

            var decLists = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>();

            var linq_declarationAmount = from decList in decLists
                                         where decHeadIDs.Contains(decList.DeclarationID)
                                         group decList by new { decList.DeclarationID, } into g
                                         select new
                                         {
                                             DecHeadID = g.Key.DeclarationID,
                                             DeclarationAmount = g.Sum(t => t.DeclTotal),
                                         };

            var ienums_declarationAmount = linq_declarationAmount.ToArray();

            #endregion

            #region 关税、增值税

            var orderItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();
            var orderItemTaxes = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>();

            var linq_orderItemTax = from orderItemTax in orderItemTaxes
                                    join orderItem in orderItems on orderItemTax.OrderItemID equals orderItem.ID
                                    where orderItemTax.Status == (int)Enums.Status.Normal
                                       && orderIDs.Contains(orderItem.OrderID)
                                    group orderItemTax by new { orderItem.OrderID, } into g
                                    select new
                                    {
                                        OrderID = g.Key.OrderID,
                                        ImportTaxValue = g.Where(t => t.Type == (int)Enums.CustomsRateType.ImportTax).Sum(t => t.Value),
                                        AddedValueTaxValue = g.Where(t => t.Type == (int)Enums.CustomsRateType.AddedValueTax).Sum(t => t.Value),
                                    };

            var ienums_orderItemTax = linq_orderItemTax.ToArray();

            #endregion

            var ienums_linq = from decHead in ienum_myDecHead
                              join order in ienums_order on decHead.OrderID equals order.OrderID
                              join declarationAmount in ienums_declarationAmount on decHead.DecHeadID equals declarationAmount.DecHeadID
                              join orderItemTax in ienums_orderItemTax on decHead.OrderID equals orderItemTax.OrderID into ienums_orderItemTax2
                              from orderItemTax in ienums_orderItemTax2.DefaultIfEmpty()
                              select new DualProductListViewModel
                              {
                                  DecHeadID = decHead.DecHeadID,
                                  OrderID = decHead.OrderID,
                                  OwnerName = decHead.OwnerName,
                                  DDate = decHead.DDate,

                                  Currency = order.Currency,
                                  AttorneyAmount = order.DeclarePrice,
                                  CustomsExchangeRate = order.CustomsExchangeRate,

                                  DeclarationAmount = declarationAmount.DeclarationAmount,

                                  ImportTaxValue = orderItemTax != null ? orderItemTax.ImportTaxValue : null,
                                  AddedValueTaxValue = orderItemTax != null ? orderItemTax.AddedValueTaxValue : null,
                                  ContractNo = decHead.ContractNo,
                                  ConsignorCode = decHead.ConsignorCode
                              };

            var results = ienums_linq.ToArray();

            #region 当天汇率

            var dDates = ienum_myDecHead.Select(t => t.DDate?.Date).Distinct().ToArray();

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
                                          && dDates.Contains(exchangeRateLog.CreateDate.Date)
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
                if (results[i].DDate != null)
                {
                    var theExchangeRateLog = ienums_exchangeRateLog
                        .Where(t => t.Code == results[i].Currency
                                 && t.CreateDate.ToString("yyyy-MM-dd") == results[i].DDate?.ToString("yyyy-MM-dd"))
                        .OrderByDescending(t => t.CreateDate).FirstOrDefault();

                    if (theExchangeRateLog != null)
                    {
                        results[i].ThatDayExchangeRate = theExchangeRateLog.Rate;
                    }
                }
            }

            for (int i = 0; i < results.Length; i++)
            {
                if (results[i].DDate != null)
                {
                    var theExchangeRate = ienums_exchangeRate
                        .Where(t => t.Code == results[i].Currency
                                 && t.UpdateDate?.ToString("yyyy-MM-dd") == results[i].DDate?.ToString("yyyy-MM-dd"))
                        .OrderByDescending(t => t.UpdateDate).FirstOrDefault();

                    if (theExchangeRate != null)
                    {
                        results[i].ThatDayExchangeRate = theExchangeRate.Rate;
                    }
                }
            }

            #endregion

            Func<DualProductListViewModel, object> convert = item => new
            {
                DecHeadID = item.DecHeadID,
                OwnerName = item.OwnerName,
                ContractNo = item.ContractNo,
                Currency = item.Currency,
                DDate = item.DDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                DeclarationAmount = item.DeclarationAmount,
                AttorneyAmount = item.AttorneyAmount,
                ConsignorCode = item.ConsignorCode,
                ImportTaxValue = item.ImportTaxValue != null ? Convert.ToString(item.ImportTaxValue) : "",
                AddedValueTaxValue = item.AddedValueTaxValue != null ? Convert.ToString(item.AddedValueTaxValue) : "",
                ThatDayExchangeRate = item.ThatDayExchangeRate != null ? Convert.ToString(item.ThatDayExchangeRate) : "",
                CustomsExchangeRate = item.CustomsExchangeRate != null ? Convert.ToString(item.CustomsExchangeRate) : "",
            };

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                Func<dynamic, object> convertAgain = item => new
                {
                    item.OwnerName,
                    item.ContractNo,
                    item.Currency,
                    item.DDate,
                    item.DeclarationAmount,
                    item.AttorneyAmount,
                    item.ConsignorCode,
                    item.ImportTaxValue,
                    item.AddedValueTaxValue,
                    item.ThatDayExchangeRate,
                    item.CustomsExchangeRate,
                    
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
        /// 查询报关日期开始时间查询
        /// </summary>
        /// <param name="begin"></param>
        /// <returns></returns>
        public DualProductListView SearchByDDateBegin(DateTime begin)
        {
            var linq = from query in this.IQueryable
                       where query.DDate >= begin
                       select query;

            var view = new DualProductListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 查询报关日期结束时间查询
        /// </summary>
        /// <param name="begin"></param>
        /// <returns></returns>
        public DualProductListView SearchByDDateEnd(DateTime end)
        {
            var linq = from query in this.IQueryable
                       where query.DDate < end
                       select query;

            var view = new DualProductListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 查询客户名称查询
        /// </summary>
        /// <param name="ownerName"></param>
        /// <returns></returns>
        public DualProductListView SearchByOwnerName(string ownerName)
        {
            var linq = from query in this.IQueryable
                       where query.OwnerName.Contains(ownerName)
                       select query;

            var view = new DualProductListView(this.Reponsitory, linq);
            return view;
        }

    }

    public class DualProductListViewModel
    {
        /// <summary>
        /// DecHeadID
        /// </summary>
        public string DecHeadID { get; set; }

        /// <summary>
        /// OrderID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string OwnerName { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 报关日期
        /// </summary>
        public DateTime? DDate { get; set; }

        /// <summary>
        /// 报关金额
        /// </summary>
        public decimal DeclarationAmount { get; set; }

        /// <summary>
        /// 委托金额
        /// </summary>
        public decimal AttorneyAmount { get; set; }

        /// <summary>
        /// 关税
        /// </summary>
        public decimal? ImportTaxValue { get; set; }

        /// <summary>
        /// 增值税
        /// </summary>
        public decimal? AddedValueTaxValue { get; set; }

        /// <summary>
        /// 当天汇率
        /// </summary>
        public decimal? ThatDayExchangeRate { get; set; }

        /// <summary>
        /// 海关汇率
        /// </summary>
        public decimal? CustomsExchangeRate { get; set; }

        /// <summary>
        /// 合同号
        /// </summary>
        public string ContractNo { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string ConsignorCode { get; set; }
    }

}
