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
    /// 已换汇中, 换汇详情中的列表视图
    /// </summary>
    public class SwapedDetailListView : QueryView<SwapedDetailListViewModel, ScCustomsReponsitory>
    {
        public SwapedDetailListView()
        {
        }

        protected SwapedDetailListView(ScCustomsReponsitory reponsitory, IQueryable<SwapedDetailListViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<SwapedDetailListViewModel> GetIQueryable()
        {
            var swapNoticeItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticeItems>();

            var iQuery = from swapNoticeItem in swapNoticeItems
                         where swapNoticeItem.Status == (int)Enums.Status.Normal
                         select new SwapedDetailListViewModel
                         {
                             SwapNoticeItemID = swapNoticeItem.ID,
                             SwapNoticeID = swapNoticeItem.SwapNoticeID,
                             DecHeadID = swapNoticeItem.DecHeadID,
                             Amount = swapNoticeItem.Amount ?? 0,
                         };

            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<SwapedDetailListViewModel> iquery = this.IQueryable.Cast<SwapedDetailListViewModel>();
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_swapNoticeItems = iquery.ToArray();

            var decHeadIDs = ienum_swapNoticeItems.Select(t => t.DecHeadID).Distinct().ToArray();
            var swapNoticeIDs = ienum_swapNoticeItems.Select(t => t.SwapNoticeID).Distinct().ToArray();

            #region 合同协议号、订单编号、报关日期、订单中报关总金额

            var decHeads = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();

            var linq_decHead = from decHead in decHeads
                               join order in orders on decHead.OrderID equals order.ID
                               where decHeadIDs.Contains(decHead.ID)
                                  && order.Status == (int)Enums.Status.Normal
                               select new
                               {
                                   DecHeadID = decHead.ID,
                                   ContrNo = decHead.ContrNo,
                                   OrderID = decHead.OrderID,
                                   DDate = decHead.DDate,
                                   OrderDeclarePrice = order.DeclarePrice,
                               };

            var ienums_decHead = linq_decHead.ToArray();

            #endregion

            #region 币种、报关单的报关金额（判断 DecHead 是否换汇完成用到）

            var decLists = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>();

            var linq_currency = from decList in decLists
                                where decHeadIDs.Contains(decList.DeclarationID)
                                group decList by new { DecHeadID = decList.DeclarationID, } into g
                                select new
                                {
                                    DecHeadID = g.Key.DecHeadID,
                                    Currency = g.First() == null ? "" : g.First().TradeCurr,
                                    DecHeadTotalAmount = g.Sum(t => t.DeclTotal),
                                };

            var ienums_currency = linq_currency.ToArray();

            #endregion

            #region 已换汇金额

            var swapNotices = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNotices>();
            var swapNoticeItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticeItems>();

            var linq_swapedAmount = from swapNoticeItem in swapNoticeItems
                                    join swapNotice in swapNotices on swapNoticeItem.SwapNoticeID equals swapNotice.ID
                                    where swapNoticeItem.Status == (int)Enums.Status.Normal
                                       && swapNoticeIDs.Contains(swapNoticeItem.SwapNoticeID)
                                       && swapNotice.Status == (int)Enums.SwapStatus.Audited
                                    group swapNoticeItem by new { swapNoticeItem.DecHeadID, } into g
                                    select new
                                    {
                                        DecHeadID = g.Key.DecHeadID,
                                        SwapedAmount = g.Sum(t => t.Amount),
                                        LastestSwapNoticeItemID = g.OrderByDescending(t => t.CreateDate).FirstOrDefault().ID,
                                    };

            var ienums_swapedAmount = linq_swapedAmount.ToArray();

            #endregion

            var ienums_linq = from swapNoticeItem in ienum_swapNoticeItems
                              join decHead in ienums_decHead on swapNoticeItem.DecHeadID equals decHead.DecHeadID into ienums_decHead2
                              from decHead in ienums_decHead2.DefaultIfEmpty()
                              join currency in ienums_currency on swapNoticeItem.DecHeadID equals currency.DecHeadID into ienums_currency2
                              from currency in ienums_currency2.DefaultIfEmpty()
                              join swapedAmount in ienums_swapedAmount on swapNoticeItem.DecHeadID equals swapedAmount.DecHeadID into ienums_swapedAmount2
                              from swapedAmount in ienums_swapedAmount2.DefaultIfEmpty()
                              select new SwapedDetailListViewModel
                              {
                                  SwapNoticeItemID = swapNoticeItem.SwapNoticeItemID,
                                  SwapNoticeID = swapNoticeItem.SwapNoticeID,
                                  DecHeadID = swapNoticeItem.DecHeadID,
                                  Amount = swapNoticeItem.Amount,

                                  ContrNo = decHead != null ? decHead.ContrNo : "",
                                  OrderID = decHead != null ? decHead.OrderID : "",
                                  DDate = decHead != null ? decHead.DDate : null,

                                  Currency = currency != null ? currency.Currency : "",
                                  DecHeadTotalAmount = currency != null ? (decimal?)currency.DecHeadTotalAmount : null,
                                  SwapedAmount = swapedAmount != null ? swapedAmount.SwapedAmount : null,
                                  LastestSwapNoticeItemID = swapedAmount != null ? swapedAmount.LastestSwapNoticeItemID : null,
                                  IsBackwardExtrusion = false,
                                  OrderDeclarePrice = decHead != null ? (decimal?)decHead.OrderDeclarePrice : null,
                              };

            var results = ienums_linq.ToArray();


            #region 换汇委托金额

            //1. 首先判断 SwapNoticeItem 所在的 DecHead 是否换汇完成
            //2. 如果 SwapNoticeItem 所在的 DecHead 未换汇完成, 则 SwapNoticeItem 一定不是最后一笔, 按照比例计算
            //3. 如果 SwapNoticeItem 所在的 DecHead 已换汇完成, --> 如果这个 SwapNoticeItem 按时间排序不是最后一个, 则按照比例计算
            //                                                  --> 如果这个 SwapNoticeItem 按时间排序是最后一个, 则使用倒挤计算

            //也就是说, 要倒挤计算的需要同时满足两个条件, (1) SwapNoticeItem 所在的 DecHead 已换汇完成 (2) 这个 SwapNoticeItem 按时间排序是最后一个

            for (int i = 0; i < results.Length; i++)
            {
                if (results[i].DecHeadTotalAmount != null && results[i].SwapedAmount != null && results[i].OrderDeclarePrice != null
                    && (decimal)results[i].DecHeadTotalAmount <= (decimal)results[i].SwapedAmount
                    && results[i].SwapNoticeItemID == results[i].LastestSwapNoticeItemID)
                {
                    results[i].IsBackwardExtrusion = true;
                    //需要倒挤计算
                    decimal 除了这个值之外的计算 = ((decimal)(
                            (results[i].DecHeadTotalAmount - results[i].SwapedAmount) * results[i].OrderDeclarePrice / results[i].DecHeadTotalAmount)
                            ).ToRound(4);

                    results[i].换汇委托金额 = ((decimal)results[i].OrderDeclarePrice - 除了这个值之外的计算).ToRound(4);

                }
                else
                {
                    results[i].IsBackwardExtrusion = false;
                    //按比例计算
                    results[i].换汇委托金额 = ((decimal)(results[i].SwapedAmount * results[i].OrderDeclarePrice / results[i].DecHeadTotalAmount)).ToRound(4);
                }
            }

            #endregion


            Func<SwapedDetailListViewModel, object> convert = item => new
            {
                ID = item.SwapNoticeItemID,
                ContrNo = item.ContrNo,
                OrderID = item.OrderID,
                Currency = item.Currency,
                SwapAmount = item.Amount,
                CreateDate = item.DDate?.ToString("yyyy-MM-dd"),
                换汇委托金额 = item.换汇委托金额 != null ? item.换汇委托金额.ToString() : "",
                IsBackwardExtrusion = item.IsBackwardExtrusion,
            };

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return new
                {
                    total = total,
                    Size = pageSize ?? 20,
                    Index = pageIndex ?? 1,
                    rows = results.Select(convert).ToArray(),
                };
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
        /// 根据 SwapNoticeID 查询
        /// </summary>
        /// <param name="swapNoticeID"></param>
        /// <returns></returns>
        public SwapedDetailListView SearchBySwapNoticeID(string swapNoticeID)
        {
            var linq = from query in this.IQueryable
                       where query.SwapNoticeID == swapNoticeID
                       select query;

            var view = new SwapedDetailListView(this.Reponsitory, linq);
            return view;
        }
    }

    public class SwapedDetailListViewModel
    {
        /// <summary>
        /// SwapNoticeItemID
        /// </summary>
        public string SwapNoticeItemID { get; set; }

        /// <summary>
        /// SwapNoticeID
        /// </summary>
        public string SwapNoticeID { get; set; }

        /// <summary>
        /// DecHeadID
        /// </summary>
        public string DecHeadID { get; set; }

        /// <summary>
        /// 合同协议号
        /// </summary>
        public string ContrNo { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 换汇金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 报关日期
        /// </summary>
        public DateTime? DDate { get; set; }

        /// <summary>
        /// 报关单的 Declist 的金额求和
        /// </summary>
        public decimal? DecHeadTotalAmount { get; set; }

        /// <summary>
        /// 已换汇金额
        /// </summary>
        public decimal? SwapedAmount { get; set; }

        /// <summary>
        /// 同一个 DecHead 下, 最新的 SwapNoticeItemID
        /// </summary>
        public string LastestSwapNoticeItemID { get; set; }

        /// <summary>
        /// 是否需要倒挤计算
        /// </summary>
        public bool IsBackwardExtrusion { get; set; } = false;

        /// <summary>
        /// 订单中 DeclarePrice
        /// </summary>
        public decimal? OrderDeclarePrice { get; set; }

        /// <summary>
        /// 换汇委托金额
        /// </summary>
        public decimal? 换汇委托金额 { get; set; }
    }

}
