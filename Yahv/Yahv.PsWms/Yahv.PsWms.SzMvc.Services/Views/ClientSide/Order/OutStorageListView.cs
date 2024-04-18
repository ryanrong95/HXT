using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.SzMvc.Services.Enums;
using Yahv.Underly;

namespace Yahv.PsWms.SzMvc.Services.Views
{
    /// <summary>
    /// 出库订单列表查询
    /// </summary>
    public class OutStorageListView : QueryView<OutStorageListViewModel, PsOrderRepository>
    {
        public OutStorageListView()
        {
        }

        protected OutStorageListView(PsOrderRepository reponsitory, IQueryable<OutStorageListViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<OutStorageListViewModel> GetIQueryable()
        {
            var orders = this.Reponsitory.ReadTable<Layers.Data.Sqls.PsOrder.Orders>();
            var orderTransports = this.Reponsitory.ReadTable<Layers.Data.Sqls.PsOrder.OrderTransports>();

            var iQuery = from order in orders
                         join orderTransport in orderTransports on order.ID equals orderTransport.OrderID
                         where order.Type == (int)Enums.OrderType.Outbound
                         select new OutStorageListViewModel
                         {
                             OrderID = order.ID,
                             ClientID = order.ClientID,
                             SiteuserID = order.SiteuserID,
                             CreateDate = order.CreateDate,
                             TransportMode = (TransportMode)orderTransport.TransportMode,
                             OrderStatus = (OrderStatus)order.Status,
                         };

            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<OutStorageListViewModel> iquery = this.IQueryable.Cast<OutStorageListViewModel>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_orders = iquery.ToArray();

            var ienums_linq = from order in ienum_orders
                              orderby order.CreateDate descending
                              select new OutStorageListViewModel
                              {
                                  OrderID = order.OrderID,
                                  CreateDate = order.CreateDate,
                                  TransportMode = order.TransportMode,
                                  OrderStatus = order.OrderStatus,
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

            Func<OutStorageListViewModel, object> convert = item => new
            {
                CreateDateDes = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                OrderID = item.OrderID,
                TransportModeDes = item.TransportMode.GetDescription(),
                OrderStatusDes = item.OrderStatus.GetDescription(),
            };


            return new
            {
                list = results.Select(convert).ToArray(),
                total = total,
            };
        }

        /// <summary>
        /// 根据 SiteuserID 查询
        /// </summary>
        /// <param name="siteuserID"></param>
        /// <returns></returns>
        public OutStorageListView SearchBySiteuserID(string siteuserID)
        {
            var linq = from query in this.IQueryable
                       where query.SiteuserID == siteuserID
                       select query;

            var view = new OutStorageListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据订单状态查询
        /// </summary>
        /// <param name="orderStatus"></param>
        /// <returns></returns>
        public OutStorageListView SearchByOrderStatus(OrderStatus orderStatus)
        {
            var linq = from query in this.IQueryable
                       where query.OrderStatus == orderStatus
                       select query;

            var view = new OutStorageListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据下单日期开始时间查询
        /// </summary>
        /// <param name="begin"></param>
        /// <returns></returns>
        public OutStorageListView SearchByCreateDateBegin(DateTime begin)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate >= begin
                       select query;

            var view = new OutStorageListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据下单日期结束时间查询
        /// </summary>
        /// <param name="end"></param>
        /// <returns></returns>
        public OutStorageListView SearchByCreateDateEnd(DateTime end)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate < end
                       select query;

            var view = new OutStorageListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据订单号查询
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public OutStorageListView SearchByOrderID(string orderID)
        {
            var linq = from query in this.IQueryable
                       where query.OrderID.Contains(orderID)
                       select query;

            var view = new OutStorageListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据型号查询
        /// </summary>
        /// <param name="partNumber"></param>
        /// <returns></returns>
        public OutStorageListView SearchByPartNumber(string partNumber)
        {
            var orderItems = this.Reponsitory.ReadTable<Layers.Data.Sqls.PsOrder.OrderItems>();
            var products = this.Reponsitory.ReadTable<Layers.Data.Sqls.PsOrder.Products>();

            var linq = from query in this.IQueryable
                       join orderItem in orderItems on query.OrderID equals orderItem.OrderID
                       join product in products on orderItem.ProductID equals product.ID
                       where product.Partnumber.Contains(partNumber)
                       select query;

            linq = linq.Distinct();

            var view = new OutStorageListView(this.Reponsitory, linq);
            return view;
        }
    }

    public class OutStorageListViewModel
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// ClientID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// SiteuserID
        /// </summary>
        public string SiteuserID { get; set; }

        /// <summary>
        /// 下单日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 收货方式
        /// </summary>
        public TransportMode TransportMode { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public OrderStatus OrderStatus { get; set; }
    }
}
