using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ExportUnSwapDecHeadListView : View<ExportUnSwapDecHeadListModel, ScCustomsReponsitory>
    {
        protected override IQueryable<ExportUnSwapDecHeadListModel> GetIQueryable()
        {
            var decHeads = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clients = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var decHeadFiles = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeadFiles>();
            var decLists = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>();
            var swapNoticeItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticeItems>();

            string targetCusDecStatus1 = MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.R);
            string targetCusDecStatus2 = MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.E1);
            //海关单一的原因，部分报关单不会有“已结关”回执，只到“放行”（P）
            string targetCusDecStatus3 = MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.P);

            var decHeadTabs = from decHead in decHeads
                              join order in orders
                                    on new { OrderID = decHead.OrderID, OrderDataStatus = (int)Enums.Status.Normal, }
                                    equals new { OrderID = order.ID, OrderDataStatus = order.Status, }
                              join client in clients
                                    on new { ClientID = order.ClientID, ClientDataStatus = (int)Enums.Status.Normal, }
                                    equals new { ClientID = client.ID, ClientDataStatus = client.Status, }
                              where (decHead.SwapStatus == (int)Enums.SwapStatus.UnAuditing || decHead.SwapStatus == (int)Enums.SwapStatus.PartAudit)
                                  && (decHead.CusDecStatus == targetCusDecStatus1 || decHead.CusDecStatus == targetCusDecStatus2 || decHead.CusDecStatus == targetCusDecStatus3)
                              select new ExportUnSwapDecHeadListModel
                              {
                                  DecHeadID = decHead.ID,
                                  DDate = decHead.DDate,
                                  OwnerName = decHead.OwnerName,
                                  ContrNo = decHead.ContrNo,
                                  OrderID = decHead.OrderID,
                                  EntryId = decHead.EntryId,
                                  Currency = order.Currency,
                                  ClientType = (Enums.ClientType)client.ClientType,
                                  DeclarePrice = order.DeclarePrice,
                              };

            var decHeadFileCountTabs = from decHeadTab in decHeadTabs
                                       join decHeadFile in decHeadFiles
                                            on new { DecHeadID = decHeadTab.DecHeadID, FileType = (int)Enums.FileType.DecHeadFile, }
                                            equals new { DecHeadID = decHeadFile.DecHeadID, FileType = decHeadFile.FileType, }
                                       group decHeadFile by new { decHeadFile.DecHeadID } into g
                                       select new ExportUnSwapDecHeadListModel
                                       {
                                           DecHeadID = g.Key.DecHeadID,
                                           DecHeadFileCount = g.Count(),
                                       };

            decHeadTabs = from decHeadTab in decHeadTabs
                          join decHeadFileCountTab in decHeadFileCountTabs on decHeadTab.DecHeadID equals decHeadFileCountTab.DecHeadID
                          where decHeadFileCountTab.DecHeadFileCount > 0
                          select new ExportUnSwapDecHeadListModel
                          {
                              DecHeadID = decHeadTab.DecHeadID,
                              DDate = decHeadTab.DDate,
                              OwnerName = decHeadTab.OwnerName,
                              ContrNo = decHeadTab.ContrNo,
                              OrderID = decHeadTab.OrderID,
                              EntryId = decHeadTab.EntryId,
                              Currency = decHeadTab.Currency,
                              ClientType = decHeadTab.ClientType,
                              DeclarePrice = decHeadTab.DeclarePrice,
                          };


            var declTotalSumTabs = from decHeadTab in decHeadTabs
                                   join decList in decLists on decHeadTab.DecHeadID equals decList.DeclarationID
                                   group decList by new { decList.DeclarationID, } into g
                                   select new ExportUnSwapDecHeadListModel
                                   {
                                       DecHeadID = g.Key.DeclarationID,
                                       DeclTotalSum = g.Sum(t => t.DeclTotal),
                                   };

            var swapedAmountSumTabs = from decHeadTab in decHeadTabs
                                      join swapNoticeItem in swapNoticeItems
                                            on new { DecHeadID = decHeadTab.DecHeadID, SwapNoticeItemDataStatus = (int)Enums.Status.Normal, }
                                            equals new { DecHeadID = swapNoticeItem.DecHeadID, SwapNoticeItemDataStatus = swapNoticeItem.Status, }
                                      where swapNoticeItem.Amount != null
                                      group swapNoticeItem by new { swapNoticeItem.DecHeadID, } into g
                                      select new ExportUnSwapDecHeadListModel
                                      {
                                          DecHeadID = g.Key.DecHeadID,
                                          SwapedAmount = g.Sum(t => (decimal)t.Amount),
                                      };

            var results = from decHeadTab in decHeadTabs
                          join declTotalSumTab in declTotalSumTabs on decHeadTab.DecHeadID equals declTotalSumTab.DecHeadID into declTotalSumTabs2
                          from declTotalSumTab in declTotalSumTabs2.DefaultIfEmpty()
                          join swapedAmountSumTab in swapedAmountSumTabs on decHeadTab.DecHeadID equals swapedAmountSumTab.DecHeadID into swapedAmountSumTabs2
                          from swapedAmountSumTab in swapedAmountSumTabs2.DefaultIfEmpty()
                          orderby decHeadTab.DDate
                          select new ExportUnSwapDecHeadListModel
                          {
                              DecHeadID = decHeadTab.DecHeadID,
                              DDate = decHeadTab.DDate,
                              OwnerName = decHeadTab.OwnerName,
                              ContrNo = decHeadTab.ContrNo,
                              OrderID = decHeadTab.OrderID,
                              EntryId = decHeadTab.EntryId,
                              Currency = decHeadTab.Currency,
                              ClientType = decHeadTab.ClientType,
                              DeclarePrice = decHeadTab.DeclarePrice,

                              DeclTotalSum = declTotalSumTab.DeclTotalSum != null ? declTotalSumTab.DeclTotalSum : 0,
                              SwapedAmount = swapedAmountSumTab.SwapedAmount != null ? swapedAmountSumTab.SwapedAmount : 0,
                              UpSwapedAmount = (declTotalSumTab.DeclTotalSum != null ? declTotalSumTab.DeclTotalSum.Value : 0) 
                                                - (swapedAmountSumTab.SwapedAmount != null ? swapedAmountSumTab.SwapedAmount.Value : 0),
                          };

            return results;
        }
    }

    public class ExportUnSwapDecHeadListModel : IUnique
    {
        public string ID { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string DecHeadID { get; set; } = string.Empty;

        /// <summary>
        /// 报关日期
        /// </summary>
        public DateTime? DDate { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string OwnerName { get; set; } = string.Empty;

        /// <summary>
        /// 合同号
        /// </summary>
        public string ContrNo { get; set; } = string.Empty;

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderID { get; set; } = string.Empty;

        /// <summary>
        /// 海关编号
        /// </summary>
        public string EntryId { get; set; } = string.Empty;

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// 客户类型
        /// </summary>
        public Enums.ClientType ClientType { get; set; }

        /// <summary>
        /// 报关金额
        /// </summary>
        public decimal? DeclTotalSum { get; set; }

        /// <summary>
        /// 委托金额
        /// </summary>
        public decimal DeclarePrice { get; set; }

        /// <summary>
        /// 已换汇金额
        /// </summary>
        public decimal? SwapedAmount { get; set; }

        /// <summary>
        /// 未换汇金额
        /// </summary>
        public decimal UpSwapedAmount { get; set; }

        public int DecHeadFileCount { get; set; }
    }
}
