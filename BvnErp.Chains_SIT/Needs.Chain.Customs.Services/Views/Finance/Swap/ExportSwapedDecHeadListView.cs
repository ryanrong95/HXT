using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ExportSwapedDecHeadListView : View<ExportSwapedDecHeadListModel, ScCustomsReponsitory>
    {
        protected override IQueryable<ExportSwapedDecHeadListModel> GetIQueryable()
        {
            var swapNoticeItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticeItems>();
            var swapNotices = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNotices>();
            var decHeads = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var decLists = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>();

            var swapedItemTabs = from swapNoticeItem in swapNoticeItems
                                 join swapNotice in swapNotices
                                    on new
                                    {
                                        SwapNoticeID = swapNoticeItem.SwapNoticeID,
                                        SwapNoticeItemDataStatus = swapNoticeItem.Status,
                                        SwapStatus = (int)Enums.SwapStatus.Audited,
                                    }
                                    equals new
                                    {
                                        SwapNoticeID = swapNotice.ID,
                                        SwapNoticeItemDataStatus = (int)Enums.Status.Normal,
                                        SwapStatus = swapNotice.Status,
                                    }
                                 join decHead in decHeads
                                    on new { DecHeadID = swapNoticeItem.DecHeadID, }
                                    equals new { DecHeadID = decHead.ID, }
                                 join order in orders
                                    on new { OrderID = decHead.OrderID, OrderDataStatus = (int)Enums.Status.Normal, }
                                    equals new { OrderID = order.ID, OrderDataStatus = order.Status, }
                                 where decHead.CusDecStatus != "04"
                                 select new ExportSwapedDecHeadListModel
                                 {
                                     DecHeadID = decHead.ID,
                                     DDate = decHead.DDate,
                                     OwnerName = decHead.OwnerName,
                                     ContrNo = decHead.ContrNo,
                                     OrderID = decHead.OrderID,
                                     EntryId = decHead.EntryId,
                                     Currency = swapNotice.Currency,
                                     DeclarePrice = order.DeclarePrice,
                                     SwapedAmount = swapNoticeItem.Amount,
                                     SwapNoticeCreateDate = swapNotice.CreateDate,
                                     SwapNoticeUpdateDate = swapNotice.UpdateDate,
                                     SwapNoticeItemCreateDate = swapNoticeItem.CreateDate,
                                     BankName = swapNotice.BankName,
                                     ConsignorCode = decHead.ConsignorCode,
                                 };

            var declTotalSumTabs = from decList in decLists
                                   join swapedItemTab in swapedItemTabs on decList.DeclarationID equals swapedItemTab.DecHeadID
                                   group decList by new { decList.DeclarationID } into g
                                   select new ExportSwapedDecHeadListModel
                                   {
                                       DecHeadID = g.Key.DeclarationID,
                                       DeclTotalSum = g.Sum(t => t.DeclTotal),
                                   };

            var results = from swapedItemTab in swapedItemTabs
                          join declTotalSumTab in declTotalSumTabs on swapedItemTab.DecHeadID equals declTotalSumTab.DecHeadID into declTotalSumTabs2
                          from declTotalSumTab in declTotalSumTabs2.DefaultIfEmpty()
                          orderby swapedItemTab.SwapNoticeCreateDate descending, swapedItemTab.SwapNoticeUpdateDate descending
                          select new ExportSwapedDecHeadListModel
                          {
                              DecHeadID = swapedItemTab.ID,
                              DDate = swapedItemTab.DDate,
                              OwnerName = swapedItemTab.OwnerName,
                              ContrNo = swapedItemTab.ContrNo,
                              OrderID = swapedItemTab.OrderID,
                              EntryId = swapedItemTab.EntryId,
                              Currency = swapedItemTab.Currency,
                              DeclarePrice = swapedItemTab.DeclarePrice,
                              SwapedAmount = swapedItemTab.SwapedAmount,
                              SwapNoticeCreateDate = swapedItemTab.SwapNoticeCreateDate,
                              SwapNoticeUpdateDate = swapedItemTab.SwapNoticeUpdateDate,
                              SwapNoticeItemCreateDate = swapedItemTab.SwapNoticeItemCreateDate,
                              BankName = swapedItemTab.BankName,
                              ConsignorCode = swapedItemTab.ConsignorCode,

                              DeclTotalSum = declTotalSumTab.DeclTotalSum != null ? declTotalSumTab.DeclTotalSum : 0,
                          };

            return results;
        }
    }

    public class ExportSwapedDecHeadListModel : IUnique
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; } = string.Empty;

        /// <summary>
        /// DecHeadID
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
        /// 委托金额
        /// </summary>
        public decimal DeclarePrice { get; set; }

        /// <summary>
        /// 换汇金额
        /// </summary>
        public decimal? SwapedAmount { get; set; }

        /// <summary>
        /// 换汇日期
        /// </summary>
        public DateTime SwapNoticeCreateDate { get; set; }

        /// <summary>
        /// SwapNoticeUpdateDate
        /// </summary>
        public DateTime SwapNoticeUpdateDate { get; set; }

        /// <summary>
        /// SwapNoticeItemCreateDate
        /// </summary>
        public DateTime SwapNoticeItemCreateDate { get; set; }

        /// <summary>
        /// 换汇银行
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 报关金额
        /// </summary>
        public decimal? DeclTotalSum { get; set; }

        /// <summary>
        /// 境外发货人
        /// </summary>
        public string ConsignorCode { get; set; }
    }
}
