using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ExportUnSwapDecHeadListViewNew
    {
        private ScCustomsReponsitory Reponsitory { get; set; }

        public ExportUnSwapDecHeadListViewNew()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        public List<ExportUnSwapDecHeadListViewNewModel> GetResults(LambdaExpression[] expressions)
        {
            var unSwapDecHeadListBaseView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.UnSwapDecHeadListBaseView>();
            var decheads = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();

            var tabDecHeads2 = from baseView in unSwapDecHeadListBaseView
                               join dechead in decheads on baseView.DecHeadID equals dechead.ID
                               join order in orders on dechead.OrderID equals order.ID
                               select new ExportUnSwapDecHeadListViewNewModel
                               {
                                   DecHeadID = baseView.DecHeadID,
                                   DDate = baseView.DDate,
                                   OwnerName = baseView.OwnerName,
                                   ContrNo = baseView.ContrNo,
                                   OrderID = baseView.OrderID,
                                   EntryId = baseView.EntryId,
                                   Currency = baseView.Currency,

                                   ClientType = (Enums.ClientType)baseView.ClientType,
                                   IsOverDate = baseView.IsOverDate != 0,
                                   IsHasLimitArea = baseView.IsHasLimitArea != 0,
                                   IsPrePayExchange = baseView.IsPrePayExchange != 0,
                                   OrderAmount = baseView.OrderAmount,
                                   DecHeadAmount = baseView.DecHeadAmount,
                                   ConsignorCode = dechead.ConsignorCode,
                                   ClientID = order.ClientID,
                                   IsExcessive = null,
                               };

            foreach (var expression in expressions)
            {
                tabDecHeads2 = tabDecHeads2.Where(expression as Expression<Func<ExportUnSwapDecHeadListViewNewModel, bool>>);
            }

            //tabDecHeads2 = tabDecHeads2.OrderBy(t => t.DDate);

            //tabDecHeads2 = tabDecHeads2.OrderByDescending(t => t.OrderAmount);

            //获取数据
            var ienum_tabDecHeads2 = tabDecHeads2.ToArray();

            // DecHeadID
            var decHeadIDs = tabDecHeads2.Select(t => t.DecHeadID);
            var orderIDs = tabDecHeads2.Select(t => t.OrderID);
            var clientIDs = tabDecHeads2.Select(t => t.ClientID);

            var decLists = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>();
            var swapNoticeItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticeItems>();

            var declTotalSumTabs = (from decList in decLists
                                    where decHeadIDs.Contains(decList.DeclarationID)
                                    group decList by new { decList.DeclarationID, } into g
                                    select new ExportUnSwapDecHeadListModel
                                    {
                                        DecHeadID = g.Key.DeclarationID,
                                        DeclTotalSum = g.Sum(t => t.DeclTotal),
                                    }).ToArray();

            var swapedAmountSumTabs = (from swapNoticeItem in swapNoticeItems
                                       where decHeadIDs.Contains(swapNoticeItem.DecHeadID)
                                          && swapNoticeItem.Status == (int)Enums.Status.Normal
                                          && swapNoticeItem.Amount != null
                                       group swapNoticeItem by new { swapNoticeItem.DecHeadID, } into g
                                       select new ExportUnSwapDecHeadListModel
                                       {
                                           DecHeadID = g.Key.DecHeadID,
                                           SwapedAmount = g.Sum(t => (decimal)t.Amount),
                                       }).ToArray();



            var orderInfos = (from order in orders
                              where orderIDs.Contains(order.ID)
                                 && order.Status == (int)Enums.Status.Normal
                              select new
                              {
                                  order.ClientID,
                                  OrderID = order.ID,
                                  DeclarePrice = order.DeclarePrice,
                                  order.PaidExchangeAmount
                              }).ToArray();

            var ordersgroup = from g in orderInfos
                              group g by g.ClientID into g
                              select new
                              {
                                  ClientID = g.Key,
                                  PaidExchangeAmount = g.Sum(x => x.PaidExchangeAmount),
                                  DeclarePrice = g.Sum(x => x.DeclarePrice),
                                  未付汇总金额 = g.Sum(x => x.DeclarePrice) - g.Sum(x => x.PaidExchangeAmount)
                              }
                ;

            var companyView = new ClientCompanyView(this.Reponsitory).Where(t => clientIDs.Contains(t.ID)).ToArray();

            var results = from decHeadTab in ienum_tabDecHeads2
                          join declTotalSumTab in declTotalSumTabs on decHeadTab.DecHeadID equals declTotalSumTab.DecHeadID into declTotalSumTabs2
                          from declTotalSumTab in declTotalSumTabs2.DefaultIfEmpty()
                          join swapedAmountSumTab in swapedAmountSumTabs on decHeadTab.DecHeadID equals swapedAmountSumTab.DecHeadID into swapedAmountSumTabs2
                          from swapedAmountSumTab in swapedAmountSumTabs2.DefaultIfEmpty()
                          orderby decHeadTab.DDate
                          join orderInfo in orderInfos on decHeadTab.OrderID equals orderInfo.OrderID into orderInfos2
                          from orderInfo in orderInfos2.DefaultIfEmpty()
                          orderby decHeadTab.DDate ascending, decHeadTab.OrderAmount descending
                          join company in companyView on decHeadTab.ClientID equals company.ID
                          select new ExportUnSwapDecHeadListViewNewModel
                          {
                              DecHeadID = decHeadTab.DecHeadID,
                              DDate = decHeadTab.DDate,
                              //OwnerName = decHeadTab.OwnerName,
                              OwnerName = company.Company.Name,
                              ContrNo = decHeadTab.ContrNo,
                              OrderID = decHeadTab.OrderID,
                              EntryId = decHeadTab.EntryId,
                              Currency = decHeadTab.Currency,
                              ClientType = decHeadTab.ClientType,
                              DeclarePrice = orderInfo != null ? (decimal?)orderInfo.DeclarePrice : null,

                              DeclTotalSum = declTotalSumTab != null ? declTotalSumTab.DeclTotalSum : 0,
                              SwapedAmount = swapedAmountSumTab != null ? swapedAmountSumTab.SwapedAmount : 0,
                              UpSwapedAmount = (declTotalSumTab != null ? declTotalSumTab.DeclTotalSum.Value : 0)
                                                - (swapedAmountSumTab != null ? swapedAmountSumTab.SwapedAmount.Value : 0),
                              ConsignorCode = decHeadTab.ConsignorCode,
                              // 未付汇总金额 = ordergroup.DeclarePrice - ordergroup.PaidExchangeAmount,
                              //未换汇总金额 = declist.SwapAmount - baseView.DecHeadAmount,
                              IsExcessive = ordersgroup.FirstOrDefault(x => x.ClientID == company.ID).未付汇总金额 - declTotalSumTab.DeclTotalSum.Value > 0

                          };

            return results.ToList();
        }

    }

    public class ExportUnSwapDecHeadListViewNewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string DecHeadID { get; set; }

        /// <summary>
        /// 报关日期
        /// </summary>
        public DateTime? DDate { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string OwnerName { get; set; }

        /// <summary>
        /// 合同号
        /// </summary>
        public string ContrNo { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 海关编号
        /// </summary>
        public string EntryId { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 境外发货人
        /// </summary>
        public string ConsignorCode { get; set; }

        // ------------------------------------------------------

        /// <summary>
        /// 报关金额
        /// </summary>
        public decimal? DeclTotalSum { get; set; }

        /// <summary>
        /// 委托金额
        /// </summary>
        public decimal? DeclarePrice { get; set; }

        /// <summary>
        /// 未换汇金额
        /// </summary>
        public decimal UpSwapedAmount { get; set; }

        /// <summary>
        /// 已换汇金额
        /// </summary>
        public decimal? SwapedAmount { get; set; }

        // ------------------------------------------------------

        /// <summary>
        /// 客户类型
        /// </summary>
        public Enums.ClientType ClientType { get; set; }

        /// <summary>
        /// 是否超过90天
        /// </summary>
        public bool IsOverDate { get; set; }

        /// <summary>
        /// 是否有受限地区
        /// </summary>
        public bool IsHasLimitArea { get; set; }

        /// <summary>
        /// 是否预付汇
        /// </summary>
        public bool IsPrePayExchange { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal OrderAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal DecHeadAmount { get; set; }
        /// <summary>
        /// 订单中的ClientID
        /// </summary>
        public string ClientID { get; set; }


        public bool? IsExcessive { get; set; }
    }

}
