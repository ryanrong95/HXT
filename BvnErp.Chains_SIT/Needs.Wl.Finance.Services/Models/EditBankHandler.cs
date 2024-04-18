using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Finance.Services.Models
{
    /// <summary>
    /// 如果 this.BankName 为null或空字符串，则不更新 银行名称
    /// </summary>
    public class EditBankHandler
    {
        private string SwapNoticeID { get; set; } = string.Empty;

        private string[] CleanDecHeadID_Array { get; set; }

        private string BankName { get; set; } = string.Empty;

        public EditBankHandler(string swapNoticeID, string cleanDecHeadIDs, string bankName)
        {
            this.SwapNoticeID = swapNoticeID;
            this.CleanDecHeadID_Array = cleanDecHeadIDs.Split(',');
            this.BankName = bankName;
        }

        public EditBankHandler(string swapNoticeID, string cleanDecHeadIDs)
        {
            this.SwapNoticeID = swapNoticeID;
            this.CleanDecHeadID_Array = cleanDecHeadIDs.Split(',');
        }

        public void Execute()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //SwapNoticeItems 表：对应 SwapNoticeID 和 DecHeadID 的至为 400
                var deleteSwapNoticeItems = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticeItems>()
                    .Where(t => t.SwapNoticeID == this.SwapNoticeID
                             && !this.CleanDecHeadID_Array.Contains(t.DecHeadID)
                             && t.Status == (int)Needs.Wl.Models.Enums.Status.Normal)
                    .ToList();

                reponsitory.Update<Layer.Data.Sqls.ScCustoms.SwapNoticeItems>(new
                {
                    Status = (int)Needs.Wl.Models.Enums.Status.Delete,
                }, item => deleteSwapNoticeItems.Select(t => t.ID).Contains(item.ID));

                //SwapNotices 表：
                //TotalAmount 按照剩下有效的(200) SwapNoticeItems 求和，算出TotalAmount
                //BankName 银行名称直接修改
                var currentTotalAmount = (from swapNoticeItem in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticeItems>()
                                          where swapNoticeItem.SwapNoticeID == this.SwapNoticeID
                                             && swapNoticeItem.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                                          group swapNoticeItem by new { swapNoticeItem.SwapNoticeID } into g
                                          select new
                                          {
                                              TotalAmount = g.Sum(t => t.Amount) ?? 0,
                                          }).FirstOrDefault();

                if (string.IsNullOrEmpty(this.BankName))
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.SwapNotices>(new
                    {
                        TotalAmount = currentTotalAmount.TotalAmount,
                    }, item => item.ID == this.SwapNoticeID);
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.SwapNotices>(new
                    {
                        TotalAmount = currentTotalAmount.TotalAmount,
                        BankName = this.BankName,
                    }, item => item.ID == this.SwapNoticeID);
                }

                //PayApplySwapNoticeItemRelation 表：将对应 SwapNoticeItemID 的数据置为 400
                var deletePayApplySwapNoticeItemRelations = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayApplySwapNoticeItemRelation>()
                    .Where(t => deleteSwapNoticeItems.Select(u => u.ID).Contains(t.SwapNoticeItemID)
                             && t.Status == (int)Needs.Wl.Models.Enums.Status.Normal)
                    .ToList();

                reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayApplySwapNoticeItemRelation>(new
                {
                    Status = (int)Needs.Wl.Models.Enums.Status.Delete,
                    UpdateDate = DateTime.Now,
                }, item => deletePayApplySwapNoticeItemRelations.Select(t => t.ID).Contains(item.ID));

                //PayExchangeApplyItems 表：
                //按照 PayApplySwapNoticeItemRelation 表 中的对应数据，将 PayExchangeApplyItems 中对应的数据置为 ApplyItemStatus.Appling
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>(new
                {
                    ApplyStatus = (int)Needs.Wl.Models.Enums.ApplyItemStatus.Appling,
                }, item => deletePayApplySwapNoticeItemRelations.Select(t => t.PayExchangeApplyItemID).Contains(item.ID));



                //修改报关单状态
                //根据 swapNoticeItems 中的 Amount 求和，和 报关单的 DecList 求和相比较，判断 DecHeads 中的 SwapStatus 字段使用哪个枚举
                string[] decHeadIDs = deleteSwapNoticeItems.Select(t => t.DecHeadID).ToArray();

                var decHeadSwapedAmounts = (from swapNoticeItem in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticeItems>()
                                            where decHeadIDs.Contains(swapNoticeItem.DecHeadID) && swapNoticeItem.Status == (int)Wl.Models.Enums.Status.Normal
                                            group swapNoticeItem by new { swapNoticeItem.DecHeadID, } into g
                                            select new
                                            {
                                                DecHeadID = g.Key.DecHeadID,
                                                SwapedAmount = g.Sum(t => t.Amount),
                                            }).ToList();

                var decHeadTotalSwapAmounts = (from decList in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>()
                                               where decHeadIDs.Contains(decList.DeclarationID)
                                               group decList by new { decList.DeclarationID } into g
                                               select new
                                               {
                                                   DecHeadID = g.Key.DeclarationID,
                                                   TotalSwapAmount = g.Sum(t => t.DeclTotal),
                                               }).ToList();

                foreach (var item in deleteSwapNoticeItems)
                {
                    string decHeadID = item.DecHeadID;
                    decimal totalSwapAmount = decHeadTotalSwapAmounts
                        .FirstOrDefault(t => t.DecHeadID == decHeadID).TotalSwapAmount;
                    //item.SwapDecHead.Lists.Sum(t => t.DeclTotal);

                    Wl.Models.Enums.SwapStatus targetDecHeadSwapStatus;

                    var oneSwapedAmount = decHeadSwapedAmounts.Where(t => t.DecHeadID == decHeadID).FirstOrDefault();
                    if (oneSwapedAmount == null || oneSwapedAmount.SwapedAmount == 0)
                    {
                        targetDecHeadSwapStatus = Wl.Models.Enums.SwapStatus.UnAuditing;
                    }
                    else
                    {
                        if (totalSwapAmount <= oneSwapedAmount.SwapedAmount)
                        {
                            targetDecHeadSwapStatus = Wl.Models.Enums.SwapStatus.Auditing;
                        }
                        else
                        {
                            targetDecHeadSwapStatus = Wl.Models.Enums.SwapStatus.PartAudit;
                        }
                    }

                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new
                    {
                        SwapStatus = (int)targetDecHeadSwapStatus,
                    }, t => t.ID == decHeadID);
                }




            }
        }

    }
}
