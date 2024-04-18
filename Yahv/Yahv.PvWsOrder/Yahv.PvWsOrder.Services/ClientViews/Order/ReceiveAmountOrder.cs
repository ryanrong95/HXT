using Layers.Data.Sqls;
using System.Linq;
using System.Linq.Expressions;
using Yahv.Linq.Generic;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.PvWsOrder.Services.Views.Origins;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    /// <summary>
    /// 代收货款列表订单
    /// </summary>
    public class ReceiveAmountOrder : OrderViewBase
    {
        readonly IUser _user;

        public ReceiveAmountOrder(IUser user) : base(user, new OrderType[] { OrderType.Delivery, OrderType.Transport })
        {
            this._user = user;
        }

        public ReceiveAmountOrder(IUser user, PvWsOrderReponsitory reponsitory, IQueryable<WsOrder> iquery) : base(user, reponsitory, iquery, new OrderType[] { OrderType.Delivery, OrderType.Transport })
        {
            this._user = user;
        }
        /// <summary>
        /// 根据供应商查询
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns></returns>
        public ReceiveAmountOrder SearchBySupplier(string supplier)
        {
            return new ReceiveAmountOrder(this._user, this.Reponsitory, SearchBySupplierName(supplier));
        }

        /// <summary>
        /// 获取代收货款数据
        /// </summary>
        /// <param name="expressions"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public PageList<OrderExtends> GetToChargeOrders(LambdaExpression[] expressions, int pageSize, int pageIndex)
        {
            var orders = GetOrders(expressions).OrderByDescending(item => item.CreateDate);
            var amounts = from item in new ApplicationItemsOrigin(this.Reponsitory)
                          join application in new ApplicationsOrigin(this.Reponsitory) on item.ApplicationID equals application.ID
                          where application.Type == Enums.ApplicationType.Receival && item.Status == GeneralStatus.Normal && application.Status == GeneralStatus.Normal
                          select new
                          {
                              item.OrderID,
                              item.Amount,
                          };

            var data = from order in orders
                       join amount in amounts on order.ID equals amount.OrderID into _amounts
                       where order.TotalPrice.GetValueOrDefault() > _amounts.Sum(item => (decimal?)item.Amount).GetValueOrDefault()
                       select order;

            var total = data.Count();
            var linq = data.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToArray();

            var waybills = new WayBillAlls(this.Reponsitory).Where(item => linq.Select(a => a.Output.WayBillID).Contains(item.ID));

            var result = from order in linq
                         join waybill in waybills on order.Output.WayBillID equals waybill.ID into waybills2
                         from waybill in waybills2.DefaultIfEmpty()
                         select new OrderExtends
                         {
                             ID = order.ID,
                             MainStatus = order.MainStatus,
                             PaymentStatus = order.PaymentStatus,
                             CreateDate = order.CreateDate,
                             SupplierID = order.SupplierID,
                             OutWaybill = waybill,
                             Type = order.Type,
                             Output = new OrderOutput
                             {
                                 ID = order.Output.ID,
                                 Currency = order.Output.Currency,
                             }
                         };

            return new PageList<OrderExtends>(pageIndex, pageSize, result, total);
        }
    }
}

