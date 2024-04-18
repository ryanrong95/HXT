using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 已经申请换汇的报关单信息
    /// (DecHeadID、ContrNo、SwapAmount、SwapedAmount)
    /// </summary>
    public class PreSwapDecHeadListView
    {
        private ScCustomsReponsitory Reponsitory { get; set; }

        public PreSwapDecHeadListView()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        public PreSwapDecHeadListView(ScCustomsReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        private IQueryable<PreSwapDecHeadListViewModel> GetTargetDecHeads(LambdaExpression[] expressions)
        {
            var decHeads = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();

            var targetDecHeads = from decHead in decHeads
                                 where (decHead.SwapStatus == (int)Needs.Ccs.Services.Enums.SwapStatus.Auditing
                                     || decHead.SwapStatus == (int)Needs.Ccs.Services.Enums.SwapStatus.PartAudit)
                                     && decHead.IsSuccess == true
                                 select new PreSwapDecHeadListViewModel
                                 {
                                     DecHeadID = decHead.ID,
                                     ContrNo = decHead.ContrNo,
                                 };

            foreach (var expression in expressions)
            {
                targetDecHeads = targetDecHeads.Where(expression as Expression<Func<PreSwapDecHeadListViewModel, bool>>);
            }

            return targetDecHeads;
        }

        //private IQueryable<PreSwapDecHeadListViewModel> GetDecListTab(List<PreSwapDecHeadListViewModel> tabDecHeads2)
        //{
        //    var decLists = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>();
        //    string[] decHeadIDInDecLists = tabDecHeads2.Select(t => t.DecHeadID).ToArray();

        //    var decListTab = from decList in decLists
        //                     where decHeadIDInDecLists.Contains(decList.DeclarationID)
        //                     select new PreSwapDecHeadListViewModel
        //                     {
        //                         DecListID = decList.ID,
        //                         DecHeadID = decList.DeclarationID,
        //                         TradeCurr = decList.TradeCurr,
        //                         DeclTotal = decList.DeclTotal,
        //                     };

        //    return decListTab;
        //}

        //private IQueryable<PreSwapDecHeadListViewModel> GetDeclTotalSumTab(IQueryable<PreSwapDecHeadListViewModel> decListTab)
        //{
        //    var declTotalSumTab = from decListTab_Item in decListTab
        //                          group decListTab_Item by new { decListTab_Item.DecHeadID } into g
        //                          select new PreSwapDecHeadListViewModel
        //                          {
        //                              DecHeadID = g.Key.DecHeadID,
        //                              SwapAmount = g.Sum(t => t.DeclTotal),
        //                          };

        //    return declTotalSumTab;
        //}

        private IQueryable<PreSwapDecHeadListViewModel> GetSwapedAmountSumTab(List<PreSwapDecHeadListViewModel> tabDecHeads2, string swapNoticeID)
        {
            var swapNoticeItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticeItems>();
            string[] targetDecHeadIDs = tabDecHeads2.Select(t => t.DecHeadID).ToArray();

            var swapedAmountTab = from swapNoticeItem in swapNoticeItems
                                  where targetDecHeadIDs.Contains(swapNoticeItem.DecHeadID)
                                     && swapNoticeItem.Status == (int)Enums.Status.Normal
                                     && swapNoticeItem.SwapNoticeID == swapNoticeID
                                  select new PreSwapDecHeadListViewModel
                                  {
                                      DecHeadID = swapNoticeItem.DecHeadID,
                                      SwapedAmount = swapNoticeItem.Amount == null ? (decimal)0 : swapNoticeItem.Amount.Value,
                                  };



            var swapedAmountSumTab = from swapNoticeItem in swapedAmountTab
                                     group swapNoticeItem by new { swapNoticeItem.DecHeadID } into g
                                     select new PreSwapDecHeadListViewModel
                                     {
                                         DecHeadID = g.Key.DecHeadID,
                                         SwapedAmount = g.Sum(t => t.SwapedAmount),
                                     };

            return swapedAmountSumTab;
        }

        private List<PreSwapDecHeadListViewModel> AttachInfo(List<PreSwapDecHeadListViewModel> tabDecHeads2, string swapNoticeID)
        {
            //var decListTab = GetDecListTab(tabDecHeads2);
            //var declTotalSumTab = GetDeclTotalSumTab(decListTab);
            var swapedAmountSumTab = GetSwapedAmountSumTab(tabDecHeads2, swapNoticeID);

            var result = from tabDecHeads2_Item in tabDecHeads2
                         //join declTotalSumTab_Item in declTotalSumTab
                         //   on new { DecHeadID = tabDecHeads2_Item.DecHeadID, }
                         //   equals new { DecHeadID = declTotalSumTab_Item.DecHeadID, }
                         //   into declTotalSumTab2
                         //from declTotalSumTab_Item in declTotalSumTab2.DefaultIfEmpty()

                         join swapedAmountSumTab_Item in swapedAmountSumTab
                            on new { DecHeadID = tabDecHeads2_Item.DecHeadID, }
                            equals new { DecHeadID = swapedAmountSumTab_Item.DecHeadID, }
                            into swapedAmountSumTab2
                         from swapedAmountSumTab_Item in swapedAmountSumTab2.DefaultIfEmpty()

                         select new PreSwapDecHeadListViewModel
                         {
                             DecHeadID = tabDecHeads2_Item.DecHeadID,
                             ContrNo = tabDecHeads2_Item.ContrNo,                           
                             //SwapAmount = declTotalSumTab_Item == null ? 0 : declTotalSumTab_Item.SwapAmount,
                             SwapedAmount = swapedAmountSumTab_Item == null ? 0 : swapedAmountSumTab_Item.SwapedAmount,
                         };

            return result.ToList();
        }

        public List<PreSwapDecHeadListViewModel> GetResults(LambdaExpression[] expressions, string swapNoticeID)
        {
            var tabDecHeads2 = GetTargetDecHeads(expressions).ToList();

            var results = AttachInfo(tabDecHeads2, swapNoticeID).ToList();

            return results;
        }


    }

    public class PreSwapDecHeadListViewModel
    {
        public string DecHeadID { get; set; } = string.Empty;

        public string ContrNo { get; set; } = string.Empty;

        //public decimal SwapAmount { get; set; }

        public decimal SwapedAmount { get; set; }

        //

        public string DecListID { get; set; } = string.Empty;

        public string TradeCurr { get; set; } = string.Empty;

        public decimal DeclTotal { get; set; }
    }

}
