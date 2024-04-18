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
    /// 应收账款双抬头服务费列表视图
    /// </summary>
    public class DualServiceListView : QueryView<DualServiceListViewModel, ScCustomsReponsitory>
    {
        public DualServiceListView()
        {
        }

        protected DualServiceListView(ScCustomsReponsitory reponsitory, IQueryable<DualServiceListViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<DualServiceListViewModel> GetIQueryable()
        {
            var decHeads = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
            var decTaxs = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxs>();

            var iQuery = from decHead in decHeads
                         join decTax in decTaxs on decHead.ID equals decTax.ID
                         where decHead.IsSuccess == true
                            && decHead.CusDecStatus != "04"
                            && decTax.InvoiceType == (int)Enums.InvoiceType.Service
                         orderby decHead.DDate descending
                         select new DualServiceListViewModel
                         {
                             DecHeadID = decHead.ID,
                             OrderID = decHead.OrderID,
                             OwnerName = decHead.OwnerName,
                             DDate = decHead.DDate,
                         };

            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<DualServiceListViewModel> iquery = this.IQueryable.Cast<DualServiceListViewModel>().OrderByDescending(item => item.DDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_myDecHead = iquery.ToArray();

            //DecHeadID
            var decHeadIDs = ienum_myDecHead.Select(item => item.DecHeadID);

            //OrderID
            var orderIDs = ienum_myDecHead.Select(item => item.OrderID);

            #region 服务费金额

            var orderReceipts = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>();

            int[] ServiceOrderFeeTypes = new int[]
            {
                (int)Enums.OrderFeeType.AgencyFee,
                (int)Enums.OrderFeeType.Incidental,
            };

            var linq_orderReceipt = from orderReceipt in orderReceipts
                                    where orderIDs.Contains(orderReceipt.OrderID)
                                       && orderReceipt.Type == (int)Enums.OrderReceiptType.Receivable
                                       && ServiceOrderFeeTypes.Contains(orderReceipt.FeeType)
                                       && orderReceipt.Status == (int)Enums.Status.Normal
                                    group orderReceipt by new { orderReceipt.OrderID, } into g
                                    select new
                                    {
                                        OrderID = g.Key.OrderID,
                                        ServiceAmount = g.Sum(t => t.Amount),
                                    };

            var ienums_orderReceipt = linq_orderReceipt.ToArray();

            #endregion


            var ienums_linq = from decHead in ienum_myDecHead
                              join orderReceipt in ienums_orderReceipt on decHead.OrderID equals orderReceipt.OrderID into ienums_orderReceipt2
                              from orderReceipt in ienums_orderReceipt2.DefaultIfEmpty()
                              select new DualServiceListViewModel
                              {
                                  DecHeadID = decHead.DecHeadID,
                                  OrderID = decHead.OrderID,
                                  OwnerName = decHead.OwnerName,
                                  DDate = decHead.DDate,

                                  ServiceAmount = orderReceipt != null ? orderReceipt.ServiceAmount : 0,
                              };

            var results = ienums_linq.ToArray();

            #region 发票金额、发票号、开票日期

            var invoiceNotices = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNotices>();
            var invoiceNoticeItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>();

            List<InvoiceNoticeItemMiddleModel> invoiceNoticeItemDatas = new List<InvoiceNoticeItemMiddleModel>();

            if (orderIDs != null && orderIDs.Any())
            {
                foreach (var orderID in orderIDs)
                {
                    var alreadyItemData = invoiceNoticeItemDatas.Where(t => t.OrderIDs.Contains(orderID)).FirstOrDefault();

                    if (alreadyItemData != null)
                    {
                        continue;
                    }

                    var linq_invoiceNoticeItem = from invoiceNoticeItem in invoiceNoticeItems
                                                 join invoiceNotice in invoiceNotices on invoiceNoticeItem.InvoiceNoticeID equals invoiceNotice.ID
                                                 where invoiceNoticeItem.Status == (int)Enums.Status.Normal
                                                    && invoiceNoticeItem.OrderID.Contains(orderID)
                                                    && invoiceNoticeItem.OrderItemID == null
                                                 select new InvoiceNoticeItemMiddleModel
                                                 {
                                                     OrderIDs = invoiceNoticeItem.OrderID,
                                                     InvoiceAmount = invoiceNoticeItem.Amount,
                                                     InvoiceNo = invoiceNoticeItem.InvoiceNo,
                                                     InvoiceTime = invoiceNotice.UpdateDate,
                                                 };

                    var ienum_invoiceNoticeItem = linq_invoiceNoticeItem.FirstOrDefault();

                    if (ienum_invoiceNoticeItem != null)
                    {
                        invoiceNoticeItemDatas.Add(ienum_invoiceNoticeItem);
                    }
                }
            }


            for (int i = 0; i < results.Length; i++)
            {
                var alreadyItemData = invoiceNoticeItemDatas.Where(t => t.OrderIDs.Contains(results[i].OrderID)).FirstOrDefault();

                if (alreadyItemData != null)
                {
                    results[i].InvoiceAmount = alreadyItemData.InvoiceAmount;
                    results[i].InvoiceNo = alreadyItemData.InvoiceNo;
                    results[i].InvoiceTime = alreadyItemData.InvoiceTime;
                }
            }

            #endregion

            Func<DualServiceListViewModel, object> convert = item => new
            {
                DecHeadID = item.DecHeadID,
                OwnerName = item.OwnerName,
                OrderID = item.OrderID,
                DDate = item.DDate?.ToString("yyyy-MM-dd"),
                ServiceAmount = item.ServiceAmount,
                UnInvoiceAmount = item.InvoiceNo != null ? 0 : item.ServiceAmount,
                InvoiceAmount = item.InvoiceNo != null ? Convert.ToString(item.InvoiceAmount) : "-",
                InvoiceNo = item.InvoiceNo != null ? item.InvoiceNo : "-",
                InvoiceTime = item.InvoiceTime?.ToString("yyyy-MM-dd HH:mm:ss"),
            };

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                Func<dynamic, object> convertAgain = item => new
                {
                    item.OwnerName,
                    item.OrderID,
                    item.DDate,
                    item.ServiceAmount,
                    item.UnInvoiceAmount,
                    item.InvoiceAmount,
                    item.InvoiceNo,
                    item.InvoiceTime,
                };

                return results.Select(convert).Select(convertAgain).ToArray();
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
        /// 根据客户名称查询
        /// </summary>
        /// <param name="ownerName"></param>
        /// <returns></returns>
        public DualServiceListView SearchByOwnerName(string ownerName)
        {
            var linq = from query in this.IQueryable
                       where query.OwnerName.Contains(ownerName)
                       select query;

            var view = new DualServiceListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据订单编号查询
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public DualServiceListView SearchByOrderID(string orderID)
        {
            var linq = from query in this.IQueryable
                       where query.OrderID.Contains(orderID)
                       select query;

            var view = new DualServiceListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据报关日期开始时间查询
        /// </summary>
        /// <param name="begin"></param>
        /// <returns></returns>
        public DualServiceListView SearchByDDateBegin(DateTime begin)
        {
            var linq = from query in this.IQueryable
                       where query.DDate >= begin
                       select query;

            var view = new DualServiceListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据报关日期结束时间查询
        /// </summary>
        /// <param name="begin"></param>
        /// <returns></returns>
        public DualServiceListView SearchByDDateEnd(DateTime end)
        {
            var linq = from query in this.IQueryable
                       where query.DDate < end
                       select query;

            var view = new DualServiceListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据发票号查询
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <returns></returns>
        public DualServiceListView SearchByInvoiceNo(string invoiceNo)
        {
            var linq = this.IQueryable;


            var invoiceNoticeItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>();

            var likeInvoiceNoticeItem = (from invoiceNoticeItem in invoiceNoticeItems
                                         where invoiceNoticeItem.InvoiceNo.Contains(invoiceNo)
                                            && invoiceNoticeItem.Status == (int)Enums.Status.Normal
                                            && invoiceNoticeItem.OrderItemID == null
                                         select new
                                         {
                                             OrderIDs = invoiceNoticeItem.OrderID,
                                         }).ToList();

            if (likeInvoiceNoticeItem != null && likeInvoiceNoticeItem.Any())
            {
                List<string> orderIDList = new List<string>();

                string[] likeOrderIDs = likeInvoiceNoticeItem.Select(t => t.OrderIDs).ToArray();

                foreach (var likeOrderID in likeOrderIDs)
                {
                    string[] likeOrderID_Array = likeOrderID.Split(',');

                    orderIDList.AddRange(likeOrderID_Array);
                }

                orderIDList = orderIDList.Distinct().ToList();


                linq = linq.Where(t => orderIDList.Contains(t.OrderID));
            }

            var view = new DualServiceListView(this.Reponsitory, linq);
            return view;
        }
    }

    public class DualServiceListViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string DecHeadID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OwnerName { get; set; }

        /// <summary>
        /// 报关日期
        /// </summary>
        public DateTime? DDate { get; set; }

        /// <summary>
        /// 服务费金额
        /// </summary>
        public decimal ServiceAmount { get; set; }

        /// <summary>
        /// 发票金额
        /// </summary>
        public decimal InvoiceAmount { get; set; }

        /// <summary>
        /// 发票号
        /// </summary>
        public string InvoiceNo { get; set; }

        /// <summary>
        /// 开票时间
        /// </summary>
        public DateTime? InvoiceTime { get; set; }



    }

    public class InvoiceNoticeItemMiddleModel
    {
        /// <summary>
        /// 一些订单号
        /// </summary>
        public string OrderIDs { get; set; }

        /// <summary>
        /// 发票金额
        /// </summary>
        public decimal InvoiceAmount { get; set; }

        /// <summary>
        /// 发票号
        /// </summary>
        public string InvoiceNo { get; set; }

        /// <summary>
        /// 开票时间
        /// </summary>
        public DateTime? InvoiceTime { get; set; }
    }
}
