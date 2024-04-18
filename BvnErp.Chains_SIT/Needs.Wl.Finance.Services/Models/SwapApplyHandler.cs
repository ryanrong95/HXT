using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Finance.Services.Models
{
    public class SwapApplyHandler
    {
        private Needs.Wl.Models.SwapNotice SwapNotice { get; set; }

        private Needs.Wl.Models.SwapNoticeItem[] SwapNoticeItems { get; set; }

        private DecHeadTotalSwapAmount[] DecHeadTotalSwapAmounts { get; set; }

        private Needs.Wl.Models.PayApplySwapNoticeItemRelation[] PayApplySwapNoticeItemRelations { get; set; }

        public SwapApplyHandler(
            Needs.Wl.Models.SwapNotice swapNotice,
            Needs.Wl.Models.SwapNoticeItem[] swapNoticeItems,
            DecHeadTotalSwapAmount[] decHeadTotalSwapAmounts,
            Needs.Wl.Models.PayApplySwapNoticeItemRelation[] payApplySwapNoticeItemRelations
            )
        {
            this.SwapNotice = swapNotice;
            this.SwapNoticeItems = swapNoticeItems;
            this.DecHeadTotalSwapAmounts = decHeadTotalSwapAmounts;
            this.PayApplySwapNoticeItemRelations = payApplySwapNoticeItemRelations;
        }

        public void DoApplay()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //创建换汇通知
                var swapNotice = new Layer.Data.Sqls.ScCustoms.SwapNotices();
                swapNotice.ID = this.SwapNotice.ID;
                swapNotice.AdminID = this.SwapNotice.AdminID;
                swapNotice.Currency = this.SwapNotice.Currency;
                swapNotice.TotalAmount = this.SwapNotice.TotalAmount;
                swapNotice.BankName = this.SwapNotice.BankName;
                swapNotice.CreateDate = this.SwapNotice.CreateDate;
                swapNotice.UpdateDate = this.SwapNotice.UpdateDate;
                swapNotice.Status = (int)this.SwapNotice.Status;
                swapNotice.ConsignorCode = this.SwapNotice.ConsignorCode;

                reponsitory.Insert(swapNotice);

                //创建换汇通知项明细
                foreach (var swapNoticeItem in this.SwapNoticeItems)
                {
                    var item = new Layer.Data.Sqls.ScCustoms.SwapNoticeItems(); ;
                    item.ID = swapNoticeItem.ID;
                    item.SwapNoticeID = swapNoticeItem.SwapNoticeID;
                    item.DecHeadID = swapNoticeItem.DecHeadID;
                    item.CreateDate = swapNoticeItem.CreateDate;
                    item.Amount = swapNoticeItem.Amount;
                    item.Status = (int)swapNoticeItem.Status;
                    item.CustomizeAmount = swapNoticeItem.CustomizeAmount;
                    reponsitory.Insert(item);
                }

                //创建 PayExchangeApplyItems 和 SwapNoticeItems 的对应关系
                foreach (var payApplySwapNoticeItemRelation in this.PayApplySwapNoticeItemRelations)
                {
                    var item = new Layer.Data.Sqls.ScCustoms.PayApplySwapNoticeItemRelation()
                    {
                        ID = payApplySwapNoticeItemRelation.ID,
                        PayExchangeApplyItemID = payApplySwapNoticeItemRelation.PayExchangeApplyItemID,
                        SwapNoticeItemID = payApplySwapNoticeItemRelation.SwapNoticeItemID,
                        Status = (int)payApplySwapNoticeItemRelation.Status,
                        CreateDate = payApplySwapNoticeItemRelation.CreateDate,
                        UpdateDate = payApplySwapNoticeItemRelation.UpdateDate,
                        Summary = payApplySwapNoticeItemRelation.Summary,
                    };
                    reponsitory.Insert(item);

                    //将 PayExchangeApplyItems 中的 ApplyStatus 置为 Applied
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>(new
                    {
                        ApplyStatus = (int)Needs.Wl.Models.Enums.ApplyItemStatus.Applied,
                    }, t => t.ID == payApplySwapNoticeItemRelation.PayExchangeApplyItemID);
                }
            }

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                string[] decHeadIDs = this.SwapNoticeItems.Select(t => t.DecHeadID).ToArray();
                var swapNoticeItems = new Needs.Wl.Models.Views.SwapNoticeItemsView(reponsitory);

                //更新报关单的状态
                //TODO:代码Review  1、生成换汇通知与变更报关单状态应该在同一个事务中
                var decHeadSwapedAmounts = (from swapNoticeItem in swapNoticeItems
                                            where decHeadIDs.Contains(swapNoticeItem.DecHeadID) && swapNoticeItem.Status == Needs.Wl.Models.Enums.Status.Normal
                                            group swapNoticeItem by new { swapNoticeItem.DecHeadID, } into g
                                            select new
                                            {
                                                DecHeadID = g.Key.DecHeadID,
                                                SwapedAmount = g.Sum(t => t.Amount),
                                            }).ToList();

                foreach (var decHeadTotalSwapAmount in this.DecHeadTotalSwapAmounts)
                {
                    Needs.Wl.Models.Enums.SwapStatus targetDecHeadSwapStatus;

                    var oneSwapedAmount = decHeadSwapedAmounts.Where(t => t.DecHeadID == decHeadTotalSwapAmount.DecHeadID).FirstOrDefault();

                    if (oneSwapedAmount != null)
                    {
                        if (decHeadTotalSwapAmount.TotalSwapAmount <= oneSwapedAmount.SwapedAmount)
                        {
                            targetDecHeadSwapStatus = Wl.Models.Enums.SwapStatus.Auditing;
                        }
                        else
                        {
                            targetDecHeadSwapStatus = Wl.Models.Enums.SwapStatus.PartAudit;
                        }

                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new
                        {
                            SwapStatus = (int)targetDecHeadSwapStatus,
                        }, t => t.ID == decHeadTotalSwapAmount.DecHeadID);
                    }
                }
            }


        }


    }

    public class DecHeadTotalSwapAmount
    {
        /// <summary>
        /// DecHeadID
        /// </summary>
        public string DecHeadID { get; set; } = string.Empty;

        /// <summary>
        /// 报关单总的换汇金额
        /// </summary>
        public decimal TotalSwapAmount { get; set; }
    }
}
