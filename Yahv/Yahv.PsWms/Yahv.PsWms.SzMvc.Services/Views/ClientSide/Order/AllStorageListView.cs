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
    /// 首页全部订单
    /// </summary>
    public class AllStorageListView : QueryView<AllStorageListViewModel, PsOrderRepository>
    {
        public AllStorageListView()
        {
        }

        protected AllStorageListView(PsOrderRepository reponsitory, IQueryable<AllStorageListViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<AllStorageListViewModel> GetIQueryable()
        {
            var orders = this.Reponsitory.ReadTable<Layers.Data.Sqls.PsOrder.Orders>();

            var iQuery = from order in orders
                         select new AllStorageListViewModel
                         {
                             OrderID = order.ID,
                             CreateDate = order.CreateDate,
                             OrderType = (Enums.OrderType)order.Type,
                             OrderStatus = (OrderStatus)order.Status,
                             ClientID = order.ClientID,
                             SiteuserID = order.SiteuserID,
                         };

            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public Tuple<T[], int> ToMyPage<T>(Func<AllStorageListViewModel, T> convert, int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<AllStorageListViewModel> iquery = this.IQueryable.Cast<AllStorageListViewModel>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_orders = iquery.ToArray();

            var ienums_linq = from order in ienum_orders
                              orderby order.CreateDate descending
                              select new AllStorageListViewModel
                              {
                                  OrderID = order.OrderID,
                                  CreateDate = order.CreateDate,
                                  OrderType = order.OrderType,
                                  OrderStatus = order.OrderStatus,
                                  ClientID = order.ClientID,
                                  SiteuserID = order.SiteuserID,
                              };

            var results = ienums_linq;

            return new Tuple<T[], int>(results.Select(convert).ToArray(), total);
        }

        /// <summary>
        /// 根据 SiteuserID 查询
        /// </summary>
        /// <param name="siteuserID"></param>
        /// <returns></returns>
        public AllStorageListView SearchBySiteuserID(string siteuserID)
        {
            var linq = from query in this.IQueryable
                       where query.SiteuserID == siteuserID
                       select query;

            var view = new AllStorageListView(this.Reponsitory, linq);
            return view;
        }
    }

    public class AllStorageListViewModel
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 下单日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 订单类型
        /// </summary>
        public Enums.OrderType OrderType { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public OrderStatus OrderStatus { get; set; }

        /// <summary>
        /// ClientID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// SiteuserID
        /// </summary>
        public string SiteuserID { get; set; }
    }
}
