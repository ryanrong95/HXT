using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.SzMvc.Services.Enums;

namespace Yahv.PsWms.SzMvc.Services.Views
{
    /// <summary>
    /// 账单详情中的数据的视图
    /// </summary>
    public class VoucherDetailListView : QueryView<VoucherDetailListViewModel, PsOrderRepository>
    {
        private string _clientID { get; set; }

        private int _cutDateIndex { get; set; }

        public VoucherDetailListView(string clientID, int cutDateIndex)
        {
            this._clientID = clientID;
            this._cutDateIndex = cutDateIndex;
        }

        protected override IQueryable<VoucherDetailListViewModel> GetIQueryable()
        {
            var payeeLeftsTopView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PsOrder.PayeeLefts>();

            var iQuery = from payeeLeft in payeeLeftsTopView
                         where payeeLeft.PayerID == this._clientID && payeeLeft.CutDateIndex == this._cutDateIndex
                         select new VoucherDetailListViewModel
                         {
                             PayeeLeftCreateDate = payeeLeft.CreateDate,
                             OrderID = payeeLeft.FormID,
                             Conduct = (Conduct)payeeLeft.Conduct,
                             Subject = payeeLeft.Subject,
                             UnitPrice = payeeLeft.UnitPrice,
                             Quantity = payeeLeft.Quantity,
                             Total = payeeLeft.Total,
                         };

            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public Tuple<T[], int> ToMyPage<T>(Func<VoucherDetailListViewModel, T> convert, int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<VoucherDetailListViewModel> iquery = this.IQueryable.Cast<VoucherDetailListViewModel>().OrderBy(item => item.PayeeLeftCreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_voucherDetails = iquery.ToArray();

            //OrderID
            var orderIDs = ienum_voucherDetails.Select(item => item.OrderID).Distinct();

            #region 订单中

            var orders = this.Reponsitory.ReadTable<Layers.Data.Sqls.PsOrder.Orders>();

            var linq_order = from order in orders
                             where orderIDs.Contains(order.ID)
                             select new
                             {
                                 OrderID = order.ID,
                                 OrderStatus = (OrderStatus)order.Status,
                                 OrderCreateDate = order.CreateDate,
                             };

            var ienums_order = linq_order.ToArray();

            #endregion

            var ienums_linq = from voucherDetail in ienum_voucherDetails
                              join order in ienums_order on voucherDetail.OrderID equals order.OrderID
                              select new VoucherDetailListViewModel
                              {
                                  PayeeLeftCreateDate = voucherDetail.PayeeLeftCreateDate,
                                  OrderID = voucherDetail.OrderID,
                                  OrderStatus = order.OrderStatus,
                                  OrderCreateDate = order.OrderCreateDate,
                                  Conduct = voucherDetail.Conduct,
                                  Subject = voucherDetail.Subject,
                                  UnitPrice = voucherDetail.UnitPrice,
                                  Quantity = voucherDetail.Quantity,
                                  Total = voucherDetail.Total,
                              };

            var results = ienums_linq;

            return new Tuple<T[], int>(results.Select(convert).ToArray(), total);
        }
    }

    public class VoucherDetailListViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime PayeeLeftCreateDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OrderStatus OrderStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime OrderCreateDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Conduct Conduct { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Total { get; set; }
    }
}
