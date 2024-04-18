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
    /// 已换汇申请
    /// </summary>
    public class PayExchangeSwapedNoticeListView
    {
        private ScCustomsReponsitory Reponsitory { get; set; }

        public PayExchangeSwapedNoticeListView()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        public PayExchangeSwapedNoticeListView(ScCustomsReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        private IQueryable<PayExchangeSwapedNoticeListViewModel> GetAll(LambdaExpression[] expressions)
        {
            var payExchangeSwapedNotices = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.PayExchangeSwapedNotices>();
            var decHeads = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.DecHeads>().Where(t => t.CusDecStatus != "04");
            var orders = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clients = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.Clients>();

            var results = from payExchangeSwapedNotice in payExchangeSwapedNotices
                          join decHead in decHeads
                                on new { DecHeadID = payExchangeSwapedNotice.DecHeadID, PayExchangeSwapedNoticeDataStatus = payExchangeSwapedNotice.Status, }
                                equals new { DecHeadID = decHead.ID, PayExchangeSwapedNoticeDataStatus = (int)Enums.Status.Normal, }
                          join order in orders
                                on new { OrderID = decHead.OrderID, OrderDataStatus = (int)Enums.Status.Normal, }
                                equals new { OrderID = order.ID, OrderDataStatus = order.Status, }
                          join client in clients
                                on new { ClientID = order.ClientID, ClientDataStatus = (int)Enums.Status.Normal, }
                                equals new { ClientID = client.ID, ClientDataStatus = client.Status, }
                          orderby payExchangeSwapedNotice.HandleStatus ascending, payExchangeSwapedNotice.CreateTime ascending
                          select new PayExchangeSwapedNoticeListViewModel
                          {
                              PayExchangeSwapedNoticeID = payExchangeSwapedNotice.ID,
                              DecHeadID = payExchangeSwapedNotice.DecHeadID,
                              ContrNo = decHead.ContrNo,
                              ClientCode = client.ClientCode,
                              OwnerName = decHead.OwnerName,
                              OrderID = decHead.OrderID,
                              UnHandleAmount = payExchangeSwapedNotice.UnHandleAmount,
                              ApplyDate = payExchangeSwapedNotice.CreateTime,
                              HandleStatus = (Enums.SwapedNoticeHandleStatus)payExchangeSwapedNotice.HandleStatus,
                          };

            foreach (var expression in expressions)
            {
                results = results.Where(expression as Expression<Func<PayExchangeSwapedNoticeListViewModel, bool>>);
            }

            return results;
        }

        public List<PayExchangeSwapedNoticeListViewModel> GetResults(out int totalCount, int pageIndex, int pageSize, LambdaExpression[] expressions)
        {
            var all = GetAll(expressions);

            totalCount = all.Count();

            var resultList = all.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();

            return resultList;
        }

    }

    public class PayExchangeSwapedNoticeListViewModel
    {
        /// <summary>
        /// PayExchangeSwapedNoticeID
        /// </summary>
        public string PayExchangeSwapedNoticeID { get; set; } = string.Empty;

        /// <summary>
        /// DecHeadID
        /// </summary>
        public string DecHeadID { get; set; } = string.Empty;

        /// <summary>
        /// 合同号
        /// </summary>
        public string ContrNo { get; set; } = string.Empty;

        /// <summary>
        /// 客户编号
        /// </summary>
        public string ClientCode { get; set; } = string.Empty;

        /// <summary>
        /// 客户名称
        /// </summary>
        public string OwnerName { get; set; } = string.Empty;

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; } = string.Empty;

        /// <summary>
        /// 需补充换汇金额
        /// </summary>
        public decimal UnHandleAmount { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime ApplyDate { get; set; }

        /// <summary>
        /// 处理状态
        /// </summary>
        public Enums.SwapedNoticeHandleStatus HandleStatus { get; set; }
    }

}
