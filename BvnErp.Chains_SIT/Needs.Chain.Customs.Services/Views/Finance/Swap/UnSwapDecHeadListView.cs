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
    /// 财务 未换汇的报关单列表视图
    /// </summary>
    public class UnSwapDecHeadListView
    {
        private ScCustomsReponsitory Reponsitory { get; set; }

        public UnSwapDecHeadListView()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        public UnSwapDecHeadListView(ScCustomsReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        public class UnSwapDecHeadListModel
        {
            /// <summary>
            /// DecHeadID
            /// </summary>
            public string DecHeadID { get; set; } = string.Empty;

            /// <summary>
            /// 合同号
            /// </summary>
            public string ContrNo { get; set; } = string.Empty;

            /// <summary>
            /// 海关编号
            /// </summary>
            public string EntryId { get; set; } = string.Empty;

            /// <summary>
            /// 订单编号
            /// </summary>
            public string OrderID { get; set; } = string.Empty;

            /// <summary>
            /// 客户名称
            /// </summary>
            public string OwnerName { get; set; } = string.Empty;

            /// <summary>
            /// 境外发货人
            /// </summary>
            public string ConsignorCode { get; set; } = string.Empty;

            /// <summary>
            /// 币种
            /// </summary>
            public string Currency { get; set; } = string.Empty;

            /// <summary>
            /// 报关金额
            /// </summary>
            public decimal SwapAmount { get; set; }

            /// <summary>
            /// 报关单创建日期
            /// </summary>
            public DateTime CreateTime { get; set; }

            /// <summary>
            /// 报关日期
            /// </summary>
            public DateTime? DDate { get; set; }

            /// <summary>
            /// 报关单换汇状态
            /// </summary>
            public Enums.SwapStatus SwapStatus { get; set; }

            /// <summary>
            /// DecheadFile 的 Url
            /// </summary>
            public string Url { get; set; } = string.Empty;

            /// <summary>
            /// 报关单状态
            /// </summary>
            public string CusDecStatus { get; set; } = string.Empty;

            /// <summary>
            /// 已换汇金额
            /// </summary>
            public decimal SwapedAmount { get; set; }

            /// <summary>
            /// 用户当前申请付汇金额
            /// </summary>
            public decimal UserCurrentPayApply { get; set; }



            public int DecHeadFilesCount { get; set; }

            public string DecListID { get; set; } = string.Empty;

            public string TradeCurr { get; set; } = string.Empty;
            ///// <summary>
            ///// 报关金额
            ///// </summary>
            public decimal DeclTotal { get; set; }

            /// <summary>
            /// 客户类型(用于区分内外单)
            /// </summary>
            public Enums.ClientType ClientType { get; set; }

            /// <summary>
            /// 是否超过90天
            /// </summary>
            public bool IsOverDate { get; set; } = false;

            /// <summary>
            /// 是否有受限地区
            /// </summary>
            public bool IsHasLimitArea { get; set; } = false;

            /// <summary>
            /// 临时变量，银行名称
            /// </summary>
            public string TempBankName { get; set; } = string.Empty;

            /// <summary>
            /// 受限银行名称
            /// </summary>
            public string[] LimitBankNames { get; set; }

            /// <summary>
            /// 是否是预付汇
            /// </summary>
            public bool IsPrePayExchange { get; set; } = false;

            /// <summary>
            /// 换汇金额
            /// </summary>
            public decimal DecHeadAmount { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public decimal OrderAmount { get; set; }

            /// <summary>
            /// 是否申请付汇
            /// </summary>
            public int IsApplyPayExchange { get; set; }
            /// <summary>
            /// 订单中的客户ID
            /// </summary>
            public string ClientID { get; set; }

            /// <summary>
            /// 付汇金额
            /// </summary>
            public decimal PaidExchangeAmount { get; set; }
            /// <summary>
            /// 订单金额
            /// </summary>
            public decimal DeclarePrice { get; set; }

            //public bool isExcessive;
            public bool? IsExcessive
            {
                get; set;
            }

            public decimal 未付汇总金额
            {
                get; set;
            }

            public decimal 未换汇总金额
            {
                get; set;
            }
            //public bool IsExcessive
            //{

            //    get
            //    {
            //        var unPayexchageAmount = this.DeclarePrice - this.PaidExchangeAmount;//未付汇金额
            //        var unSwapAmount = this.DeclTotal - this.DecHeadAmount;//报关金额 -换汇金额
            //        return unPayexchageAmount - unSwapAmount > 0;

            //    }

            //}
        }

        /*
        private IQueryable<UnSwapDecHeadListModel> GetTargetDecHeads(LambdaExpression[] expressions)
        {
            var decHeads = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
            var decHeadFiles = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeadFiles>();
            var decLists = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>();
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clients = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();

            //string targetCusDecStatus1 = MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.R);
            //string targetCusDecStatus2 = MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.E1);
            ////海关单一的原因，部分报关单不会有“已结关”回执，只到“放行”（P）
            //string targetCusDecStatus3 = MultiEnumUtils.ToCode<Enums.CusDecStatus>(Enums.CusDecStatus.P);

            var tabDecHeads1 = from decHead in decHeads
                               where (decHead.SwapStatus == (int)Enums.SwapStatus.UnAuditing || decHead.SwapStatus == (int)Enums.SwapStatus.PartAudit)
                                  //&& (decHead.CusDecStatus == targetCusDecStatus1 || decHead.CusDecStatus == targetCusDecStatus2 || decHead.CusDecStatus == targetCusDecStatus3)
                                  && decHead.IsSuccess
                               select new UnSwapDecHeadListModel
                               {
                                   DecHeadID = decHead.ID,
                                   ContrNo = decHead.ContrNo,
                                   EntryId = decHead.EntryId,
                                   OrderID = decHead.OrderID,
                                   OwnerName = decHead.OwnerName,
                                   CreateTime = decHead.CreateTime,
                                   DDate = decHead.DDate,
                                   SwapStatus = (Enums.SwapStatus)decHead.SwapStatus,
                                   CusDecStatus = decHead.CusDecStatus,
                               };

            var tabDecHeadFileCount = from decHeadFile in decHeadFiles
                                      where tabDecHeads1.Select(t => t.DecHeadID).Contains(decHeadFile.DecHeadID)
                                         && decHeadFile.FileType == (int)Enums.FileType.DecHeadFile
                                      group decHeadFile by new { decHeadFile.DecHeadID } into g
                                      select new UnSwapDecHeadListModel
                                      {
                                          DecHeadID = g.Key.DecHeadID,
                                          DecHeadFilesCount = g.Count(),
                                          Url = g.FirstOrDefault() == null ? string.Empty : g.FirstOrDefault().Url,
                                      };

            tabDecHeadFileCount = tabDecHeadFileCount.Where(t => t.DecHeadFilesCount > 0);
            string[] decHeadIDInTabDecHeadFileCount = tabDecHeadFileCount.Select(t => t.DecHeadID).ToArray();

            var tabDecHeads2 = from tabDecHeads1_Item in tabDecHeads1

                               join tabDecHeadFileCount_Item in tabDecHeadFileCount
                                    on new { DecHeadID = tabDecHeads1_Item.DecHeadID, }
                                    equals new { DecHeadID = tabDecHeadFileCount_Item.DecHeadID, }
                                    into tabDecHeadFileCount2
                               from tabDecHeadFileCount_Item in tabDecHeadFileCount2.DefaultIfEmpty()

                               where decHeadIDInTabDecHeadFileCount.Contains(tabDecHeads1_Item.DecHeadID)
                               select new UnSwapDecHeadListModel
                               {
                                   DecHeadID = tabDecHeads1_Item.DecHeadID,
                                   ContrNo = tabDecHeads1_Item.ContrNo,
                                   EntryId = tabDecHeads1_Item.EntryId,
                                   OrderID = tabDecHeads1_Item.OrderID,
                                   OwnerName = tabDecHeads1_Item.OwnerName,
                                   CreateTime = tabDecHeads1_Item.CreateTime,
                                   DDate = tabDecHeads1_Item.DDate,
                                   SwapStatus = tabDecHeads1_Item.SwapStatus,
                                   Url = tabDecHeadFileCount_Item.Url,
                                   CusDecStatus = tabDecHeads1_Item.CusDecStatus,
                               };


            //var decListTab = GetDecListTab(tabDecHeads2.ToList());

            tabDecHeads2 = from tabDecHeads2_Item in tabDecHeads2

                           join order in orders
                                on new { OrderID = tabDecHeads2_Item.OrderID, OrderDataStatus = (int)Enums.Status.Normal, }
                                equals new { OrderID = order.ID, OrderDataStatus = order.Status, }
                           join client in clients
                                on new { ClientID = order.ClientID, ClientDataStatus = (int)Enums.Status.Normal, }
                                equals new { ClientID = client.ID, ClientDataStatus = client.Status, }

                           //join decListTab_Item in decListTab
                           //     on new { DecHeadID = tabDecHeads2_Item.DecHeadID, }
                           //     equals new { DecHeadID = decListTab_Item.DecHeadID, }
                           //     into decListTab2

                           select new UnSwapDecHeadListModel
                           {
                               DecHeadID = tabDecHeads2_Item.DecHeadID,
                               ContrNo = tabDecHeads2_Item.ContrNo,
                               EntryId = tabDecHeads2_Item.EntryId,
                               OrderID = tabDecHeads2_Item.OrderID,
                               OwnerName = tabDecHeads2_Item.OwnerName,
                               Currency = order.Currency, //decListTab2.First() == null ? "" : decListTab2.First().TradeCurr,
                               CreateTime = tabDecHeads2_Item.CreateTime,
                               DDate = tabDecHeads2_Item.DDate,
                               SwapStatus = tabDecHeads2_Item.SwapStatus,
                               Url = tabDecHeads2_Item.Url,
                               CusDecStatus = tabDecHeads2_Item.CusDecStatus,
                               ClientType = (Enums.ClientType)client.ClientType,
                           };


            foreach (var expression in expressions)
            {
                tabDecHeads2 = tabDecHeads2.Where(expression as Expression<Func<UnSwapDecHeadListModel, bool>>);
            }

            tabDecHeads2 = tabDecHeads2.OrderBy(t => t.DDate);

            return tabDecHeads2;
        }
        */

        private IQueryable<UnSwapDecHeadListModel> GetTargetDecHeads(LambdaExpression[] expressions, bool 是至少一种特殊报关单 = false)
        {
            var unSwapDecHeadListBaseView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.UnSwapDecHeadListBaseView>();
            var DecHead = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();

            var decListgroup = from declist in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>()
                               group declist by new { DecHeadID = declist.DeclarationID } into p
                               select new
                               {
                                   DecHeadID = p.Key.DecHeadID,
                                   SwapAmount = p.Sum(x => x.DeclTotal),
                               };
            var TemdecTotalView = from g in decListgroup
                                  join s in unSwapDecHeadListBaseView on g.DecHeadID equals s.DecHeadID
                                  select new
                                  {
                                      g.DecHeadID,
                                      g.SwapAmount,
                                      s.DecHeadAmount,
                                      s.OwnerName
                                  };

            var dectotalView = from x in TemdecTotalView
                               group x by x.OwnerName into g
                               select new
                               {
                                   OwnerName = g.Key,
                                   SwapAmount = g.Sum(x => x.SwapAmount),
                                   DecHeadAmount = g.Sum(x => x.DecHeadAmount)

                               };



            var ordersgroup = from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                              join decHead in DecHead on entity.ID equals decHead.OrderID
                              where decHead.IsSuccess == true && decHead.EntryId != "需富森提供"
                              group entity by entity.ClientID into g
                              select new
                              {
                                  ClientID = g.Key,
                                  PaidExchangeAmount = g.Sum(x => x.PaidExchangeAmount),
                                  DeclarePrice = g.Sum(x => x.DeclarePrice),
                                  weifuhui = g.Sum(x => x.DeclarePrice) - g.Sum(x => x.PaidExchangeAmount)
                              };

            var tabDecHeads2 = from baseView in unSwapDecHeadListBaseView
                               join DecHeads in DecHead on baseView.DecHeadID equals DecHeads.ID
                               join order in orders on DecHeads.OrderID equals order.ID
                               join decTotal in dectotalView on baseView.OwnerName equals decTotal.OwnerName
                               join ordergroup in ordersgroup on order.ClientID equals ordergroup.ClientID
                               select new UnSwapDecHeadListModel
                               {
                                   DecHeadID = baseView.DecHeadID,
                                   ContrNo = baseView.ContrNo,
                                   EntryId = baseView.EntryId,
                                   OrderID = baseView.OrderID,
                                   OwnerName = baseView.OwnerName,
                                   Currency = baseView.Currency,
                                   CreateTime = baseView.CreateTime,
                                   DDate = baseView.DDate,
                                   SwapStatus = (Enums.SwapStatus)baseView.SwapStatus,
                                   Url = baseView.Url,
                                   CusDecStatus = baseView.CusDecStatus,
                                   ClientType = (Enums.ClientType)baseView.ClientType,

                                   IsOverDate = baseView.IsOverDate != 0,
                                   IsHasLimitArea = baseView.IsHasLimitArea != 0,
                                   IsPrePayExchange = baseView.IsPrePayExchange != 0,
                                   DecHeadAmount = baseView.DecHeadAmount,
                                   OrderAmount = baseView.OrderAmount,
                                   IsApplyPayExchange = baseView.IsApplyPayExchange,
                                   ConsignorCode = DecHeads.ConsignorCode,
                                   ClientID = order.ClientID,
                                   //未付汇总金额 = ordersgroup.Where(item => item.ClientID == order.ClientID).Sum(item => item.weifuhui),
                                   DeclarePrice = order.DeclarePrice,
                                   // SwapAmount = declist.SwapAmount,
                                   //未付汇总金额 = (ordergroup.DeclarePrice - ordergroup.PaidExchangeAmount),
                                   //未付汇总金额 = ordergroup.weifuhui,
                                   //未换汇总金额 = decTotal.SwapAmount - decTotal.DecHeadAmount,
                                   IsExcessive = (ordergroup.weifuhui) - (decTotal.SwapAmount - decTotal.DecHeadAmount) > 0
                               };

            foreach (var expression in expressions)
            {
                tabDecHeads2 = tabDecHeads2.Where(expression as Expression<Func<UnSwapDecHeadListModel, bool>>);
            }

            if (是至少一种特殊报关单)
            {
                tabDecHeads2 = tabDecHeads2.Where(t => t.IsOverDate == true || t.IsHasLimitArea == true || t.IsPrePayExchange == true);
            }

            tabDecHeads2 = tabDecHeads2.OrderBy(t => t.DDate);

            tabDecHeads2 = tabDecHeads2.OrderByDescending(t => t.IsApplyPayExchange);

            return tabDecHeads2;
        }

        private IQueryable<UnSwapDecHeadListModel> GetDecListTab(List<UnSwapDecHeadListModel> tabDecHeads2)
        {
            var decLists = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>();
            // dechead.ConsignorCode   2020-09-07  by yeshuangshuang
            var decHeads = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();

            string[] decHeadIDInDecLists = tabDecHeads2.Select(t => t.DecHeadID).ToArray();

            var decListTab = from decList in decLists
                             join decHead in decHeads on decList.DeclarationID equals decHead.ID
                             where decHeadIDInDecLists.Contains(decList.DeclarationID)
                             select new UnSwapDecHeadListModel
                             {
                                 DecListID = decList.ID,
                                 DecHeadID = decList.DeclarationID,
                                 TradeCurr = decList.TradeCurr,
                                 DeclTotal = decList.DeclTotal,
                                 ConsignorCode = decHead.ConsignorCode//境外发货人
                             };

            return decListTab;
        }

        private IQueryable<UnSwapDecHeadListModel> GetDeclTotalSumTab(IQueryable<UnSwapDecHeadListModel> decListTab)
        {
            var declTotalSumTab = from decListTab_Item in decListTab
                                  group decListTab_Item by new { decListTab_Item.DecHeadID } into g
                                  select new UnSwapDecHeadListModel
                                  {
                                      DecHeadID = g.Key.DecHeadID,
                                      SwapAmount = g.Sum(t => t.DeclTotal),
                                  };

            return declTotalSumTab;
        }

        private IQueryable<UnSwapDecHeadListModel> GetSwapedAmountSumTab(List<UnSwapDecHeadListModel> tabDecHeads2)
        {
            var swapNoticeItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticeItems>();
            string[] targetDecHeadIDs = tabDecHeads2.Select(t => t.DecHeadID).ToArray();

            var swapedAmountTab = from swapNoticeItem in swapNoticeItems
                                  where targetDecHeadIDs.Contains(swapNoticeItem.DecHeadID)
                                     && swapNoticeItem.Status == (int)Enums.Status.Normal
                                  select new UnSwapDecHeadListModel
                                  {
                                      DecHeadID = swapNoticeItem.DecHeadID,
                                      SwapedAmount = swapNoticeItem.Amount == null ? (decimal)0 : swapNoticeItem.Amount.Value,
                                  };



            var swapedAmountSumTab = from swapNoticeItem in swapedAmountTab
                                     group swapNoticeItem by new { swapNoticeItem.DecHeadID } into g
                                     select new UnSwapDecHeadListModel
                                     {
                                         DecHeadID = g.Key.DecHeadID,
                                         SwapedAmount = g.Sum(t => t.SwapedAmount),
                                     };

            return swapedAmountSumTab;
        }

        private IQueryable<UnSwapDecHeadListModel> GetUserCurrentPayApplySumTab(List<UnSwapDecHeadListModel> tabDecHeads2)
        {
            var payExchangeApplyItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>();
            var payExchangeApplies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>();

            string[] targetOrderIDs = tabDecHeads2.Select(t => t.OrderID).ToArray();

            var tabs = from payExchangeApplyItem in payExchangeApplyItems
                       join payExchangeApply in payExchangeApplies
                            on new
                            {
                                PayExchangeApplyID = payExchangeApplyItem.PayExchangeApplyID,
                                PayExchangeApplyDataStatus = (int)Enums.Status.Normal,
                                //PayExchangeApplyStatus = (int)Enums.PayExchangeApplyStatus.Approvaled,
                            }
                            equals new
                            {
                                PayExchangeApplyID = payExchangeApply.ID,
                                PayExchangeApplyDataStatus = payExchangeApply.Status,
                                //PayExchangeApplyStatus = payExchangeApply.PayExchangeApplyStatus,
                            }
                       where targetOrderIDs.Contains(payExchangeApplyItem.OrderID)
                            && payExchangeApplyItem.Status == (int)Enums.Status.Normal
                            && payExchangeApplyItem.ApplyStatus == (int)Enums.ApplyItemStatus.Appling
                            && (payExchangeApply.PayExchangeApplyStatus == (int)Enums.PayExchangeApplyStatus.Approvaled
                                || payExchangeApply.PayExchangeApplyStatus == (int)Enums.PayExchangeApplyStatus.Completed)
                       select new UnSwapDecHeadListModel
                       {
                           OrderID = payExchangeApplyItem.OrderID,
                           UserCurrentPayApply = payExchangeApplyItem.Amount * ConstConfig.TransPremiumInsurance,
                       };

            var linq = from tab in tabs
                       group tab by new { tab.OrderID, } into g
                       select new UnSwapDecHeadListModel
                       {
                           OrderID = g.Key.OrderID,
                           UserCurrentPayApply = g.Sum(t => t.UserCurrentPayApply),
                       };

            return linq;
        }

        private List<UnSwapDecHeadListModel> GetLimitBankNames(List<UnSwapDecHeadListModel> tabDecHeads2)
        {
            var decLists = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>();
            var swapLimitCountries = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapLimitCountries>();
            var swapBanks = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapBanks>();

            var results = (from decList in decLists
                           join swapLimitCountry in swapLimitCountries
                                 on new { OriginCode = decList.OriginCountry, SwapLimitCountryDataStatus = (int)Enums.Status.Normal, }
                                 equals new { OriginCode = swapLimitCountry.Code, SwapLimitCountryDataStatus = swapLimitCountry.Status, }
                           join swapBank in swapBanks
                                 on new { BankID = swapLimitCountry.BankID, SwapBankDataStatus = (int)Enums.Status.Normal, }
                                 equals new { BankID = swapBank.ID, SwapBankDataStatus = swapBank.Status, }
                           where tabDecHeads2.Select(t => t.DecHeadID).ToArray().Contains(decList.DeclarationID)
                           group new { decList, swapBank, } by new { decList.DeclarationID, BankName = swapBank.Name, } into g
                           select new UnSwapDecHeadListModel
                           {
                               DecHeadID = g.Key.DeclarationID,
                               TempBankName = g.Key.BankName,
                           }).ToList();

            return results;
        }

        private List<UnSwapDecHeadListModel> AttachInfo(List<UnSwapDecHeadListModel> tabDecHeads2)
        {
            var decListTab = GetDecListTab(tabDecHeads2);
            var declTotalSumTab = GetDeclTotalSumTab(decListTab);
            var swapedAmountSumTab = GetSwapedAmountSumTab(tabDecHeads2);

            var result = from tabDecHeads2_Item in tabDecHeads2
                         join declTotalSumTab_Item in declTotalSumTab
                            on new { DecHeadID = tabDecHeads2_Item.DecHeadID, }
                            equals new { DecHeadID = declTotalSumTab_Item.DecHeadID, }
                            into declTotalSumTab2
                         from declTotalSumTab_Item in declTotalSumTab2.DefaultIfEmpty()

                         join swapedAmountSumTab_Item in swapedAmountSumTab
                            on new { DecHeadID = tabDecHeads2_Item.DecHeadID, }
                            equals new { DecHeadID = swapedAmountSumTab_Item.DecHeadID, }
                            into swapedAmountSumTab2
                         from swapedAmountSumTab_Item in swapedAmountSumTab2.DefaultIfEmpty()

                         select new UnSwapDecHeadListModel
                         {
                             DecHeadID = tabDecHeads2_Item.DecHeadID,
                             ContrNo = tabDecHeads2_Item.ContrNo,
                             EntryId = tabDecHeads2_Item.EntryId,
                             OrderID = tabDecHeads2_Item.OrderID,
                             OwnerName = tabDecHeads2_Item.OwnerName,
                             Currency = tabDecHeads2_Item.Currency,
                             SwapAmount = declTotalSumTab_Item == null ? 0 : declTotalSumTab_Item.SwapAmount,
                             CreateTime = tabDecHeads2_Item.CreateTime,
                             DDate = tabDecHeads2_Item.DDate,
                             SwapStatus = tabDecHeads2_Item.SwapStatus,
                             Url = tabDecHeads2_Item.Url,
                             CusDecStatus = tabDecHeads2_Item.CusDecStatus,
                             SwapedAmount = swapedAmountSumTab_Item == null ? 0 : swapedAmountSumTab_Item.SwapedAmount,
                             ClientType = tabDecHeads2_Item.ClientType,

                             IsOverDate = tabDecHeads2_Item.IsOverDate,
                             IsHasLimitArea = tabDecHeads2_Item.IsHasLimitArea,
                             IsPrePayExchange = tabDecHeads2_Item.IsPrePayExchange,
                             ConsignorCode = tabDecHeads2_Item.ConsignorCode,
                             ClientID = tabDecHeads2_Item.ClientID,
                         };

            return result.ToList();
        }

        //public IQueryable<UnSwapDecHeadListModel> SearchByIsExpisive(IQueryable<UnSwapDecHeadListModel> param)
        //{
        //    var tabDecHeads2_orgin = param;

        //    var declistView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>();
        //    var linq = from query in tabDecHeads2_orgin
        //               join declist in declistView on query.DecHeadID equals declist.DeclarationID
        //               select new UnSwapDecHeadListModel
        //               {
        //                   DecHeadID = query.DecHeadID,
        //                   ContrNo = query.ContrNo,
        //                   EntryId = query.EntryId,
        //                   OrderID = query.OrderID,
        //                   OwnerName = query.OwnerName,
        //                   Currency = query.Currency,
        //                   CreateTime = query.CreateTime,
        //                   DDate = query.DDate,
        //                   SwapStatus = (Enums.SwapStatus)query.SwapStatus,
        //                   Url = query.Url,
        //                   CusDecStatus = query.CusDecStatus,
        //                   ClientType = (Enums.ClientType)query.ClientType,
        //                   IsOverDate = query.IsOverDate,
        //                   IsHasLimitArea = query.IsHasLimitArea,
        //                   IsPrePayExchange = query.IsPrePayExchange,
        //                   DecHeadAmount = query.DecHeadAmount,
        //                   OrderAmount = query.OrderAmount,
        //                   IsApplyPayExchange = query.IsApplyPayExchange,
        //                   ConsignorCode = query.ConsignorCode,
        //                   ClientID = query.ClientID,
        //                   PaidExchangeAmount = query.PaidExchangeAmount,
        //                   DeclarePrice = query.DeclarePrice,
        //                   IsExcessive = (query.DeclarePrice - query.PaidExchangeAmount) - (declist.DeclTotal - query.DecHeadAmount) > 0
        //               };

        //    //foreach (var expression in expressions)
        //    //{
        //    //    tabDecHeads2 = tabDecHeads2.Where(expression as Expression<Func<UnSwapDecHeadListModel, bool>>);
        //    //}

        //    //if (是至少一种特殊报关单)
        //    //{
        //    //    tabDecHeads2 = tabDecHeads2.Where(t => t.IsOverDate == true || t.IsHasLimitArea == true || t.IsPrePayExchange == true);
        //    //}

        //    //tabDecHeads2 = tabDecHeads2.OrderBy(t => t.DDate);

        //    //tabDecHeads2 = tabDecHeads2.OrderByDescending(t => t.IsApplyPayExchange);

        //    return linq;
        //}

        public List<UnSwapDecHeadListModel> GetResult(out int totalCount, int pageIndex, int pageSize, LambdaExpression[] expressions, bool 是至少一种特殊报关单 = false)
        {
            var tabDecHeads2_orgin = GetTargetDecHeads(expressions, 是至少一种特殊报关单);

            totalCount = tabDecHeads2_orgin.Count();

            var tabDecHeads2 = tabDecHeads2_orgin.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();

            var results = AttachInfo(tabDecHeads2).ToList();

            var clientIDs = results.Select(o => o.ClientID).ToArray();
            var companyView = new ClientCompanyView(this.Reponsitory).Where(t => clientIDs.Contains(t.ID)).ToArray();

            //这个单独查一次是因为, 合在上面一起写, 在执行的时候会报错：
            //传入的请求具有过多的参数。该服务器支持最多 2100 个参数。请减少参数的数目，然后重新发送该请求。
            var userCurrentPayApplySumTab = GetUserCurrentPayApplySumTab(tabDecHeads2).ToList();
            results = (from result in results
                       join userCurrentPayApplySumTab_Item in userCurrentPayApplySumTab
                             on new { OrderID = result.OrderID, }
                             equals new { OrderID = userCurrentPayApplySumTab_Item.OrderID, }
                             into userCurrentPayApplySumTab2
                       from userCurrentPayApplySumTab_Item in userCurrentPayApplySumTab2.DefaultIfEmpty()
                       join company in companyView on result.ClientID equals company.ID
                       select new UnSwapDecHeadListModel
                       {
                           DecHeadID = result.DecHeadID,
                           ContrNo = result.ContrNo,
                           EntryId = result.EntryId,
                           OrderID = result.OrderID,
                           //OwnerName = result.OwnerName,
                           OwnerName = company.Company.Name,
                           Currency = result.Currency,
                           SwapAmount = result.SwapAmount,
                           CreateTime = result.CreateTime,
                           DDate = result.DDate,
                           SwapStatus = result.SwapStatus,
                           Url = result.Url,
                           CusDecStatus = result.CusDecStatus,
                           SwapedAmount = result.SwapedAmount,
                           ClientType = result.ClientType,

                           UserCurrentPayApply = userCurrentPayApplySumTab_Item == null ? 0 : userCurrentPayApplySumTab_Item.UserCurrentPayApply,

                           IsOverDate = result.IsOverDate,
                           IsHasLimitArea = result.IsHasLimitArea,
                           IsPrePayExchange = result.IsPrePayExchange,
                           ConsignorCode = result.ConsignorCode,
                           //IsExcessive = (result.DeclarePrice - result.PaidExchangeAmount) - (result.SwapAmount - result.DecHeadAmount) > 0
                       }).ToList();

            var limitBankNames = GetLimitBankNames(tabDecHeads2);
            foreach (var result in results)
            {
                result.LimitBankNames = limitBankNames.Where(t => t.DecHeadID == result.DecHeadID).Select(t => t.TempBankName).ToArray();
            }

            return results;
        }

        public List<UnSwapDecHeadListModel> GetAll(LambdaExpression[] expressions)
        {
            var tabDecHeads2 = GetTargetDecHeads(expressions).ToList();

            var results = AttachInfo(tabDecHeads2).ToList();

            //这个单独查一次是因为, 合在上面一起写, 在执行的时候会报错：
            //传入的请求具有过多的参数。该服务器支持最多 2100 个参数。请减少参数的数目，然后重新发送该请求。
            var userCurrentPayApplySumTab = GetUserCurrentPayApplySumTab(tabDecHeads2).ToList();
            results = (from result in results
                       join userCurrentPayApplySumTab_Item in userCurrentPayApplySumTab
                             on new { OrderID = result.OrderID, }
                             equals new { OrderID = userCurrentPayApplySumTab_Item.OrderID, }
                             into userCurrentPayApplySumTab2
                       from userCurrentPayApplySumTab_Item in userCurrentPayApplySumTab2.DefaultIfEmpty()
                       select new UnSwapDecHeadListModel
                       {
                           DecHeadID = result.DecHeadID,
                           ContrNo = result.ContrNo,
                           EntryId = result.EntryId,
                           OrderID = result.OrderID,
                           OwnerName = result.OwnerName,
                           Currency = result.Currency,
                           SwapAmount = result.SwapAmount,
                           CreateTime = result.CreateTime,
                           DDate = result.DDate,
                           SwapStatus = result.SwapStatus,
                           Url = result.Url,
                           CusDecStatus = result.CusDecStatus,
                           SwapedAmount = result.SwapedAmount,
                           ClientType = result.ClientType,
                           ConsignorCode = result.ConsignorCode,

                           UserCurrentPayApply = userCurrentPayApplySumTab_Item == null ? 0 : userCurrentPayApplySumTab_Item.UserCurrentPayApply,
                       }).ToList();

            return results;
        }


    }
}
