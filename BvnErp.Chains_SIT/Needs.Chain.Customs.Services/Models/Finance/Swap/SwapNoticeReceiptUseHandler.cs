using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 处理 SwapNoticeReceiptUses 表中的未处理数据
    /// </summary>
    public class SwapNoticeReceiptUseHandler
    {
        public void Execute()
        {

            List<SwapNoticeReceiptUse> newSwapNoticeReceiptUses = new List<SwapNoticeReceiptUse>();
            List<string> goUseOutSwapNoticeIDs = new List<string>();

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory(false))
            {
                //按创建时间正序排列查出 SwapNoticeReceiptUses 表中第一条未处理的
                var unHandleSwapNoticeReceiptUse = new Views.Origins.SwapNoticeReceiptUsesOrigin(reponsitory)
                                                        .Where(t => t.Type == Enums.ReceiptUseType.UnHandle
                                                                 && t.Status == Enums.Status.Normal
                                                                 && t.ID != "09562d94efb64d34b49d88bd69c7418c"
                                                                 && t.ID != "c6e2e9a314594f95a1511f5e20a01f94"
                                                                 && t.ID != "b7794500efc64b9cbff9c937db7589a3"
                                                                 && t.ID != "0238de0a5e1c417a8664c3ae0e6afaf4"
                                                                 && t.ID != "e4aff145ea4c4cef89d627755088cf86"
                                                                 && t.ID != "fd92c2fb330c49578fcf806aaeba23cf"
                                                                 && t.ID != "e7d38c246ae44fc29bef8c962506e98f"
                                                                 && t.ID != "c0bb313788164289ac6582f8d32c3c28"
                                                                 && t.ID != "7bc3f44618924ed496dfdfe6a4097cb6"
                                                                 && t.ID != "c582c96ddc854775934387e6ee6e06b0"
                                                                 && t.ID != "d3d218b4126d49adb92fec9c5be15e35"
                                                                 && t.ID != "1cb6df5dedf342c5995b2be94f44925c"
                                                                 && t.ID != "48a71c3e51ff4f21a02e43006098c31f"
                                                                 && t.ID != "093f19dbd67f47bba91b31e339eb7ede"
                                                                 && t.ID != "9a8357283a27496e95c724612912f79a"
                                                                 && t.ID != "72b727553efd4f68a98076ced68b981d"
                                                                 && t.ID != "8814ea234caf42049863066605a52de1"
                                                                 && t.ID != "88ed9445a31b430db1b273fc0e6f9d85"
                                                                 && t.ID != "12a39c0b21274aadae16373dd7ade529"
                                                                 && t.ID != "86500ad14cbf486dbf05a702ff1b44d3"
                                                                 && t.ID != "4b954c8f98d64cc1b5caefcf988edcee")
                                                        .OrderBy(t => t.CreateDate)
                                                        .FirstOrDefault();

                if (unHandleSwapNoticeReceiptUse == null)
                {
                    return;
                }

                //找出该 OrderReceipts 对应的供应商
                var orderReceipts = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>();
                var decHeads = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();

                var consignorCodeModel = (from orderReceipt in orderReceipts
                                          join decHead in decHeads on orderReceipt.OrderID equals decHead.OrderID
                                          where orderReceipt.ID == unHandleSwapNoticeReceiptUse.OrderReceiptID
                                          select new
                                          {
                                              ConsignorCode = decHead.ConsignorCode,
                                          }).FirstOrDefault();

                if (consignorCodeModel == null)
                {
                    return;
                }

                string consignorCode = consignorCodeModel.ConsignorCode;

                //依次查找，前 50, 100 ...   状态为 已换汇(2)、未标记为 IsReceiptUseOut == 1 的 SwapNotice , 顺序按照  UpdateDate 正序排序

                var usableSwapNotices = (from swapNotice in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNotices>()
                                         where swapNotice.Status == (int)Enums.SwapStatus.Audited
                                            && swapNotice.ConsignorCode == consignorCode
                                            && (swapNotice.IsReceiptUseOut == null || swapNotice.IsReceiptUseOut != (int)Enums.ReceiptUseOutType.Out)
                                            && swapNotice.UpdateDate >= new DateTime(2020, 9, 28)
                                         orderby swapNotice.UpdateDate
                                         select new
                                         {
                                             SwapNoticeID = swapNotice.ID,
                                             TotalAmountCNY = swapNotice.TotalAmountCNY ?? 0,
                                             UsedAmountCNY = (decimal)0,
                                             UsableAmountCNY = swapNotice.TotalAmountCNY ?? 0 - (decimal)0,
                                             UpdateDate = swapNotice.UpdateDate,
                                         }).Take(50).ToList();

                var usableSwapNoticeIDs = usableSwapNotices.Select(t => t.SwapNoticeID).ToArray();

                //带上 usableSwapNotice 中已经使用的金额, 算出未使用的金额
                var usedSwapNotices = (from swapNoticeReceiptUse in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticeReceiptUses>()
                                       where usableSwapNoticeIDs.Contains(swapNoticeReceiptUse.SwapNoticeID)
                                          && swapNoticeReceiptUse.Status == (int)Enums.Status.Normal
                                       group swapNoticeReceiptUse by new { swapNoticeReceiptUse.SwapNoticeID, } into g
                                       select new
                                       {
                                           SwapNoticeID = g.Key.SwapNoticeID,
                                           UsedAmountCNY = g.Sum(t => t.SwapNoticeAmountCNY) ?? 0,
                                       }).ToList();

                usableSwapNotices = (from usableSwapNotice in usableSwapNotices
                                     join usedSwapNotice in usedSwapNotices on usableSwapNotice.SwapNoticeID equals usedSwapNotice.SwapNoticeID into usedSwapNotices2
                                     from usedSwapNotice in usedSwapNotices2.DefaultIfEmpty()
                                     orderby usableSwapNotice.UpdateDate
                                     select new
                                     {
                                         SwapNoticeID = usableSwapNotice.SwapNoticeID,
                                         TotalAmountCNY = usableSwapNotice.TotalAmountCNY,
                                         UsedAmountCNY = usedSwapNotice != null ? usedSwapNotice.UsedAmountCNY : 0,
                                         UsableAmountCNY = usableSwapNotice.TotalAmountCNY - (usedSwapNotice != null ? usedSwapNotice.UsedAmountCNY : 0),
                                         UpdateDate = usableSwapNotice.UpdateDate,
                                     }).ToList();




                int needSwapNoticeCount = 0;

                //计算需要使用多少个, 哪些 SwapNotice
                for (int i = 1; i <= usableSwapNotices.Count; i++)
                {
                    if (unHandleSwapNoticeReceiptUse.OrderReceiptAmount <= usableSwapNotices.Take(i).ToList().Sum(t => t.UsableAmountCNY))
                    {
                        needSwapNoticeCount = i;
                        break;
                    }
                }

                if (needSwapNoticeCount <= 0)
                {
                    return;
                }



                var topiUsable = usableSwapNotices.Take(needSwapNoticeCount).ToList();

                decimal topiUsableSum = topiUsable.Sum(t => t.UsableAmountCNY);

                if (needSwapNoticeCount != 1)
                {
                    //如果不是第一个, 则前 i - 1 个都是全用的
                    var topi_1Usable = usableSwapNotices.Take(needSwapNoticeCount - 1).ToList();

                    foreach (var item_topi_1Usable in topi_1Usable)
                    {
                        string newSwapNoticeReceiptUseID111 = Guid.NewGuid().ToString("N");
                        newSwapNoticeReceiptUses.Add(new SwapNoticeReceiptUse()
                        {
                            ID = newSwapNoticeReceiptUseID111,
                            SourceID = unHandleSwapNoticeReceiptUse.ID,
                            Type = Enums.ReceiptUseType.ResultData,
                            OrderReceiptID = unHandleSwapNoticeReceiptUse.OrderReceiptID,
                            OrderReceiptAmount = unHandleSwapNoticeReceiptUse.OrderReceiptAmount,
                            SwapNoticeID = item_topi_1Usable.SwapNoticeID,
                            SwapNoticeAmountCNY = item_topi_1Usable.UsableAmountCNY,
                            Status = Enums.Status.Normal,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            Summary = "新增关联结果",
                        });

                        goUseOutSwapNoticeIDs.Add(item_topi_1Usable.SwapNoticeID);
                    }
                }

                decimal topi_1UsableSum = newSwapNoticeReceiptUses.Sum(t => t.SwapNoticeAmountCNY ?? 0);

                var theiUsable = usableSwapNotices.Skip(needSwapNoticeCount - 1).Take(1).FirstOrDefault();

                string newSwapNoticeReceiptUseID = Guid.NewGuid().ToString("N");
                newSwapNoticeReceiptUses.Add(new SwapNoticeReceiptUse()
                {
                    ID = newSwapNoticeReceiptUseID,
                    SourceID = unHandleSwapNoticeReceiptUse.ID,
                    Type = Enums.ReceiptUseType.ResultData,
                    OrderReceiptID = unHandleSwapNoticeReceiptUse.OrderReceiptID,
                    OrderReceiptAmount = unHandleSwapNoticeReceiptUse.OrderReceiptAmount,
                    SwapNoticeID = theiUsable.SwapNoticeID,
                    SwapNoticeAmountCNY = unHandleSwapNoticeReceiptUse.OrderReceiptAmount - topi_1UsableSum,
                    Status = Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Summary = "新增关联结果",
                });

                if (theiUsable.UsableAmountCNY == unHandleSwapNoticeReceiptUse.OrderReceiptAmount - topi_1UsableSum)
                {
                    goUseOutSwapNoticeIDs.Add(theiUsable.SwapNoticeID);
                }

                // ===========================================================================================================

                //更新 UnHandle => Handled 和 UpdateDate
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.SwapNoticeReceiptUses>(new
                {
                    Type = (int)Enums.ReceiptUseType.Handled,
                    UpdateDate = DateTime.Now,
                }, item => item.ID == unHandleSwapNoticeReceiptUse.ID);

                reponsitory.Insert<Layer.Data.Sqls.ScCustoms.SwapNoticeReceiptUseLogs>(new Layer.Data.Sqls.ScCustoms.SwapNoticeReceiptUseLogs
                {
                    ID = Guid.NewGuid().ToString(),
                    SwapNoticeReceiptUseID = unHandleSwapNoticeReceiptUse.ID,
                    OrderReceiptID = unHandleSwapNoticeReceiptUse.OrderReceiptID,
                    OrderReceiptAmount = unHandleSwapNoticeReceiptUse.OrderReceiptAmount,
                    SwapNoticeID = unHandleSwapNoticeReceiptUse.SwapNoticeID,
                    SwapNoticeAmountCNY = unHandleSwapNoticeReceiptUse.SwapNoticeAmountCNY,
                    Status = (int)Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Summary = "修改为已处理",
                });

                foreach (var goUseOutSwapNoticeID in goUseOutSwapNoticeIDs)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.SwapNotices>(new
                    {
                        IsReceiptUseOut = (int)Enums.ReceiptUseOutType.Out,
                    }, item => item.ID == goUseOutSwapNoticeID);
                }

                //插入 newSwapNoticeReceiptUses 
                foreach (var item in newSwapNoticeReceiptUses)
                {
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.SwapNoticeReceiptUses>(new Layer.Data.Sqls.ScCustoms.SwapNoticeReceiptUses
                    {
                        ID = item.ID,
                        SourceID = item.SourceID,
                        Type = (int)item.Type,
                        OrderReceiptID = item.OrderReceiptID,
                        OrderReceiptAmount = item.OrderReceiptAmount,
                        SwapNoticeID = item.SwapNoticeID,
                        SwapNoticeAmountCNY = item.SwapNoticeAmountCNY,
                        Status = (int)item.Status,
                        CreateDate = item.CreateDate,
                        UpdateDate = item.UpdateDate,
                        Summary = item.Summary,
                    });

                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.SwapNoticeReceiptUseLogs>(new Layer.Data.Sqls.ScCustoms.SwapNoticeReceiptUseLogs
                    {
                        ID = Guid.NewGuid().ToString(),
                        SwapNoticeReceiptUseID = item.ID,
                        OrderReceiptID = item.OrderReceiptID,
                        OrderReceiptAmount = item.OrderReceiptAmount,
                        SwapNoticeID = item.SwapNoticeID,
                        SwapNoticeAmountCNY = item.SwapNoticeAmountCNY,
                        Status = (int)Enums.Status.Normal,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        Summary = "生成关联数据",
                    });
                }

                reponsitory.Submit();

                // ===========================================================================================================

            }

        }
    }


    /// <summary>
    /// 取消收款时, 处理 OrderReceipt 和 SwapNotice 的关联关系
    /// </summary>
    public class UnmackSwapNoticeUseHandler
    {
        private OrderReceipt[] TargetUnmack { get; set; }

        public UnmackSwapNoticeUseHandler(OrderReceipt[] orderReceipts)
        {
            this.TargetUnmack = orderReceipts;
        }

        public void Execute()
        {
            string[] orderReceiptIDs = this.TargetUnmack.Select(t => t.ID).ToArray();

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int[] containTypes = new int[]
                {
                    (int)Enums.ReceiptUseType.UnHandle,
                    (int)Enums.ReceiptUseType.Handled,
                };

                var containSwapNoticeReceiptUseIDs = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticeReceiptUses>()
                                                        .Where(t => orderReceiptIDs.Contains(t.OrderReceiptID)
                                                                    && t.Status == (int)Enums.Status.Normal
                                                                    && containTypes.Contains(t.Type))
                                                        .Select(t => t.ID)
                                                        .ToArray();

                if (containSwapNoticeReceiptUseIDs != null && containSwapNoticeReceiptUseIDs.Length > 0)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.SwapNoticeReceiptUses>(new
                    {
                        Status = (int)Enums.Status.Delete,
                        UpdateDate = DateTime.Now,
                    }, item => containSwapNoticeReceiptUseIDs.Contains(item.ID));


                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.SwapNoticeReceiptUseLogs>(new Layer.Data.Sqls.ScCustoms.SwapNoticeReceiptUseLogs
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        SwapNoticeReceiptUseID = "",
                        OrderReceiptID = "",
                        OrderReceiptAmount = 0,
                        Status = (int)Enums.Status.Normal,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        Summary = "取消收款：" + string.Join(",", containSwapNoticeReceiptUseIDs),
                    });
                }


                var resultDataModels = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticeReceiptUses>()
                                            .Where(t => orderReceiptIDs.Contains(t.OrderReceiptID)
                                                        && t.Status == (int)Enums.Status.Normal
                                                        && t.Type == (int)Enums.ReceiptUseType.ResultData)
                                            .Select(item => new
                                            {
                                                SwapNoticeReceiptUseID = item.ID,
                                                SwapNoticeID = item.SwapNoticeID,
                                            }).ToArray();

                if (resultDataModels != null && resultDataModels.Length > 0)
                {
                    string[] resultDataSwapNoticeReceiptUseIDs = resultDataModels.Select(t => t.SwapNoticeReceiptUseID).ToArray();

                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.SwapNoticeReceiptUses>(new
                    {
                        Status = (int)Enums.Status.Delete,
                        UpdateDate = DateTime.Now,
                    }, item => resultDataSwapNoticeReceiptUseIDs.Contains(item.ID));


                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.SwapNoticeReceiptUseLogs>(new Layer.Data.Sqls.ScCustoms.SwapNoticeReceiptUseLogs
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        SwapNoticeReceiptUseID = "",
                        OrderReceiptID = "",
                        OrderReceiptAmount = 0,
                        Status = (int)Enums.Status.Normal,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        Summary = "取消收款：" + string.Join(",", resultDataSwapNoticeReceiptUseIDs),
                    });


                    string[] resultDataSwapNoticeIDs = resultDataModels.Select(t => t.SwapNoticeID).ToArray();
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.SwapNotices>(new
                    {
                        IsReceiptUseOut = (int)Enums.ReceiptUseOutType.NoOut,
                    }, item => resultDataSwapNoticeIDs.Contains(item.ID));

                }
            }

        }

    }


}
