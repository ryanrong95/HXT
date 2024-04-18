using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class SwapedListView : QueryView<SwapedListViewModel, ScCustomsReponsitory>
    {
        public SwapedListView()
        {
        }

        protected SwapedListView(ScCustomsReponsitory reponsitory, IQueryable<SwapedListViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<SwapedListViewModel> GetIQueryable()
        {
            var swapNotices = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNotices>();

            var iQuery = from swapNotice in swapNotices
                         where swapNotice.Status == (int)Enums.SwapStatus.Audited
                         select new SwapedListViewModel
                         {
                             SwapNoticeID = swapNotice.ID,
                             Currency = swapNotice.Currency,
                             TotalAmount = swapNotice.TotalAmount,
                             TotalAmountCNY = swapNotice.TotalAmountCNY,
                             ExchangeRate = swapNotice.ExchangeRate,
                             BankName = swapNotice.BankName,
                             CreateDate = swapNotice.CreateDate,
                             UpdateDate = swapNotice.UpdateDate,
                             SwapStatus = (Enums.SwapStatus)swapNotice.Status,
                             AdminID = swapNotice.AdminID,
                             ConsignorCode = swapNotice.ConsignorCode,
                             SwapCreSta = swapNotice.SwapCreSta,
                             RequestID = swapNotice.RequestID,
                             SwapCreWord = swapNotice.SwapCreWord,
                             SwapCreNo = swapNotice.SwapCreNo
                         };

            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<SwapedListViewModel> iquery = this.IQueryable.Cast<SwapedListViewModel>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue) //如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_myNotices = iquery.ToArray();

            var swapNoticeIDs = ienum_myNotices.Select(t => t.SwapNoticeID);

            var adminIDs = ienum_myNotices.Select(t => t.AdminID);

            #region 申请人

            var adminsTopView = new AdminsTopView(this.Reponsitory);

            var linq_admin = from admin in adminsTopView
                             where adminIDs.Contains(admin.ID)
                             select new
                             {
                                 AdminID = admin.ID,
                                 RealName = admin.RealName,
                             };
            var ienums_admin = linq_admin.ToArray();

            #endregion

            #region 已使用

            var swapNoticeReceiptUses = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticeReceiptUses>();

            var linq_useSwapNoticeAmountCNY = from swapNoticeReceiptUse in swapNoticeReceiptUses
                                              where swapNoticeIDs.Contains(swapNoticeReceiptUse.SwapNoticeID)
                                                 && swapNoticeReceiptUse.Status == (int)Enums.Status.Normal
                                                 && swapNoticeReceiptUse.Type == (int)Enums.ReceiptUseType.ResultData
                                              group swapNoticeReceiptUse by new { swapNoticeReceiptUse.SwapNoticeID, } into g
                                              select new
                                              {
                                                  SwapNoticeID = g.Key.SwapNoticeID,
                                                  UseSwapNoticeAmountCNY = g.Sum(t => t.SwapNoticeAmountCNY) ?? 0,
                                              };

            var ienums_useSwapNoticeAmountCNY = linq_useSwapNoticeAmountCNY.ToArray();

            #endregion

            var ienums_linq = from notice in ienum_myNotices
                              join admin in ienums_admin on notice.AdminID equals admin.AdminID into ienums_admin2
                              from admin in ienums_admin2.DefaultIfEmpty()
                              join useSwapNotice in ienums_useSwapNoticeAmountCNY on notice.SwapNoticeID equals useSwapNotice.SwapNoticeID into ienums_useSwapNoticeAmountCNY222
                              from useSwapNotice in ienums_useSwapNoticeAmountCNY222.DefaultIfEmpty()
                              select new SwapedListViewModel
                              {
                                  SwapNoticeID = notice.SwapNoticeID,
                                  Currency = notice.Currency,
                                  TotalAmount = notice.TotalAmount,
                                  TotalAmountCNY = notice.TotalAmountCNY,
                                  ExchangeRate = notice.ExchangeRate,
                                  BankName = notice.BankName,
                                  CreateDate = notice.CreateDate,
                                  UpdateDate = notice.UpdateDate,
                                  SwapStatus = notice.SwapStatus,
                                  ConsignorCode = notice.ConsignorCode,
                                  CreatorRealName = admin != null ? admin.RealName : "",
                                  UseSwapNoticeAmountCNY = useSwapNotice != null ? useSwapNotice.UseSwapNoticeAmountCNY : 0,
                                  RequestID = notice.RequestID,
                                  SwapCreSta = notice.SwapCreSta,
                                  SwapCreNo = notice.SwapCreNo,
                                  SwapCreWord = notice.SwapCreWord
                              };

            var results = ienums_linq;

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return results.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            Func<SwapedListViewModel, object> convert = item => new
            {
                ID = item.SwapNoticeID,
                Creator = item.CreatorRealName,
                item.Currency,
                TotalAmount = item.TotalAmount.ToRound(2),
                TotalAmountCNY = item.TotalAmountCNY != null ? ((decimal)item.TotalAmountCNY).ToRound(2).ToString() : "",
                ExchangeRate = item.ExchangeRate != null ? ((decimal)item.ExchangeRate).ToRound(4).ToString() : "",
                item.BankName,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                UpdateDate = item.UpdateDate.ToString("yyyy-MM-dd"),
                SwapStatus = item.SwapStatus.GetDescription(),
                ConsignorCode = item.ConsignorCode,
                UseSwapNoticeAmountCNY = item.UseSwapNoticeAmountCNY,
                RequestID = item.RequestID,
                SwapCreSta = item.SwapCreSta,
                SwapCreNo = item.SwapCreNo,
                SwapCreWord = item.SwapCreWord

            };


            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }

        /// <summary>
        /// 根据申请日期开始时间查询
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public SwapedListView SearchByStartDate(DateTime start)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate >= start
                       select query;

            var view = new SwapedListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据申请日期结束时间查询
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public SwapedListView SearchByEndDate(DateTime end)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate < end
                       select query;

            var view = new SwapedListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据换汇银行查询
        /// </summary>
        /// <param name="bankName">换汇银行</param>
        /// <returns></returns>
        public SwapedListView SearchByBankName(string bankName)
        {
            var linq = from query in this.IQueryable
                       where query.BankName == bankName
                       select query;

            var view = new SwapedListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据合同号查询
        /// </summary>
        /// <param name="contrNo"></param>
        /// <returns></returns>
        public SwapedListView SearchByContrNo(string contrNo)
        {
            var linq = this.IQueryable;

            var swapNoticeIDs = linq.Select(t => t.SwapNoticeID);

            var swapNoticeItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticeItems>();
            var decHeads = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();

            var linq_swapNoticeItem = from swapNoticeItem in swapNoticeItems
                                      join decHead in decHeads on swapNoticeItem.DecHeadID equals decHead.ID
                                      where swapNoticeIDs.Contains(swapNoticeItem.SwapNoticeID)
                                         && swapNoticeItem.Status == (int)Enums.Status.Normal
                                         && decHead.ContrNo == contrNo
                                      select new
                                      {
                                          SwapNoticeID = swapNoticeItem.SwapNoticeID,
                                      };

            var ienums_swapNoticeItem = linq_swapNoticeItem.ToArray();

            var distinct_swapNoticeIDs = ienums_swapNoticeItem.Select(t => t.SwapNoticeID).Distinct();

            linq = linq.Where(t => distinct_swapNoticeIDs.Contains(t.SwapNoticeID));

            var view = new SwapedListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据报关号查询
        /// </summary>
        /// <param name="entryId"></param>
        /// <returns></returns>
        public SwapedListView SearchByEntryId(string entryId)
        {
            var linq = this.IQueryable;

            var swapNoticeIDs = linq.Select(t => t.SwapNoticeID);

            var swapNoticeItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticeItems>();
            var decHeads = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();

            var linq_swapNoticeItem = from swapNoticeItem in swapNoticeItems
                                      join decHead in decHeads on swapNoticeItem.DecHeadID equals decHead.ID
                                      where swapNoticeIDs.Contains(swapNoticeItem.SwapNoticeID)
                                         && swapNoticeItem.Status == (int)Enums.Status.Normal
                                         && decHead.EntryId == entryId
                                      select new
                                      {
                                          SwapNoticeID = swapNoticeItem.SwapNoticeID,
                                      };

            var ienums_swapNoticeItem = linq_swapNoticeItem.ToArray();

            var distinct_swapNoticeIDs = ienums_swapNoticeItem.Select(t => t.SwapNoticeID).Distinct();

            linq = linq.Where(t => distinct_swapNoticeIDs.Contains(t.SwapNoticeID));

            var view = new SwapedListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据换汇银行查询
        /// </summary>
        /// <param name="bankName">换汇银行</param>
        /// <returns></returns>
        public SwapedListView SearchBySwapCreSta(bool creSta)
        {
            var linq = from query in this.IQueryable
                       where query.SwapCreSta == creSta
                       select query;

            var view = new SwapedListView(this.Reponsitory, linq);
            return view;
        }
    }

    public class SwapedListViewModel
    {
        /// <summary>
        /// SwapNoticeID
        /// </summary>
        public string SwapNoticeID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CreatorRealName { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 境外发货人
        /// </summary>
        public string ConsignorCode { get; set; }

        /// <summary>
        /// 换汇金额
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 换汇金额RMB
        /// </summary>
        public decimal? TotalAmountCNY { get; set; }

        /// <summary>
        /// 换汇汇率
        /// </summary>
        public decimal? ExchangeRate { get; set; }

        /// <summary>
        /// 换汇银行
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Enums.SwapStatus SwapStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 已使用 Amount CNY
        /// </summary>
        public decimal UseSwapNoticeAmountCNY { get; set; }
        public bool? SwapCreSta { get; set; }
        public string RequestID { get; set; }
        public string SwapCreWord { get; set; }
        public string SwapCreNo { get; set; }

    }
}
