using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 代理订单的视图
    /// </summary>
    public class OrdersViewBase<T> : UniqueView<T, ScCustomsReponsitory> where T : Models.Order, Interfaces.IOrder, new()
    {
        public OrdersViewBase()
        {
        }

        internal OrdersViewBase(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<T> GetIQueryable()
        {
            var clientsView = new ClientsView(this.Reponsitory);
            var clientAgreementsView = new ClientAgreementsView(this.Reponsitory);
            var orderConsigneesView = new OrderConsigneesView(this.Reponsitory);

            return from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                   join client in clientsView on order.ClientID equals client.ID
                   join clientAgreement in clientAgreementsView on order.ClientAgreementID equals clientAgreement.ID
                   join orderConsignee in orderConsigneesView on order.ID equals orderConsignee.OrderID
                   where order.Status == (int)Enums.Status.Normal
                   select new T
                   {
                       ID = order.ID,
                       Type = (Enums.OrderType)order.Type,
                       AdminID = order.AdminID,
                       UserID = order.UserID,
                       Client = client,
                       ClientAgreement = clientAgreement,
                       OrderConsignee = orderConsignee,
                       Currency = order.Currency,
                       CustomsExchangeRate = order.CustomsExchangeRate,
                       RealExchangeRate = order.RealExchangeRate,
                       IsFullVehicle = order.IsFullVehicle,
                       IsLoan = order.IsLoan,
                       PackNo = order.PackNo,
                       WarpType = order.WarpType,
                       DeclarePrice = order.DeclarePrice,
                       InvoiceStatus = (Enums.InvoiceStatus)order.InvoiceStatus,
                       PaidExchangeAmount = order.PaidExchangeAmount,
                       IsHangUp = order.IsHangUp,
                       OrderStatus = (Enums.OrderStatus)order.OrderStatus,
                       Status = (Enums.Status)order.Status,
                       CreateDate = order.CreateDate,
                       UpdateDate = order.UpdateDate,
                       Summary = order.Summary,
                       MainOrderID = order.MainOrderId,
                       OrderBillType = (Enums.OrderBillType)order.OrderBillType,
                   };
        }
    }

    /// <summary>
    /// 代理订单1
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Orders1ViewBase<T> : Needs.Linq.Generic.Unique1Classics<T, ScCustomsReponsitory> where T : Models.Order, Interfaces.IOrder, new()
    {
        public Orders1ViewBase()
        {
        }

        internal Orders1ViewBase(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<T> GetIQueryable(Expression<Func<T, bool>> expression, params LambdaExpression[] expressions)
        {
            var linq = from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                       where order.Status == (int)Enums.Status.Normal
                       select new T
                       {
                           ID = order.ID,
                           Type = (Enums.OrderType)order.Type,
                           AdminID = order.AdminID,
                           UserID = order.UserID,
                           ClientID = order.ClientID,
                           ClientAgreementID = order.ClientAgreementID,
                           Currency = order.Currency,
                           CustomsExchangeRate = order.CustomsExchangeRate,
                           RealExchangeRate = order.RealExchangeRate,
                           IsFullVehicle = order.IsFullVehicle,
                           IsLoan = order.IsLoan,
                           PackNo = order.PackNo,
                           WarpType = order.WarpType,
                           DeclarePrice = order.DeclarePrice,
                           InvoiceStatus = (Enums.InvoiceStatus)order.InvoiceStatus,
                           PaidExchangeAmount = order.PaidExchangeAmount,
                           IsHangUp = order.IsHangUp,
                           OrderStatus = (Enums.OrderStatus)order.OrderStatus,
                           Status = (Enums.Status)order.Status,
                           CreateDate = order.CreateDate,
                           UpdateDate = order.UpdateDate,
                           Summary = order.Summary,
                           MainOrderID = order.MainOrderId,
                           DeclareFlag = order.DeclareFlag == null ? Enums.DeclareFlagEnums.Unable : (Enums.DeclareFlagEnums)order.DeclareFlag,
                           CollectStatus = order.CollectStatus == null ? Enums.CollectStatusEnums.UnCollected : (Enums.CollectStatusEnums)order.CollectStatus,
                           CollectedAmount = order.CollectedAmount,
                           EnterCode = order.EnterCode,
                       };

            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<T, bool>>);
            }

            return linq.Where(expression);
        }

        protected override IEnumerable<T> OnReadShips(T[] results)
        {
            var orderIds = results.Select(o => o.ID).ToArray();
            var clientsView = new ClientsView(this.Reponsitory).Where(item => results.Select(a => a.ClientID).ToArray().Contains(item.ID)).ToArray();
            var clientAgreementsView = new ClientAgreementsView(this.Reponsitory).Where(item => results.Select(a => a.ClientAgreementID).ToArray().Contains(item.ID)).ToArray();
            var orderConsigneesView = new OrderConsigneesView(this.Reponsitory).Where(item => orderIds.Contains(item.OrderID)).ToArray();
            var orderVoyagesView = new OrderVoyagesOriginView(this.Reponsitory).Where(item => orderIds.Contains(item.Order.ID)).ToArray();

            return from order in results
                   join client in clientsView on order.ClientID equals client.ID
                   join clientAgreement in clientAgreementsView on order.ClientAgreementID equals clientAgreement.ID
                   join orderConsignee in orderConsigneesView on order.ID equals orderConsignee.OrderID
                   select new T
                   {
                       ID = order.ID,
                       Type = order.Type,
                       AdminID = order.AdminID,
                       UserID = order.UserID,
                       ClientID = order.ClientID,
                       Client = client,
                       ClientAgreementID = order.ClientAgreementID,
                       ClientAgreement = clientAgreement,
                       OrderConsignee = orderConsignee,
                       Currency = order.Currency,
                       CustomsExchangeRate = order.CustomsExchangeRate,
                       RealExchangeRate = order.RealExchangeRate,
                       IsFullVehicle = order.IsFullVehicle,
                       IsLoan = order.IsLoan,
                       PackNo = order.PackNo,
                       WarpType = order.WarpType,
                       DeclarePrice = order.DeclarePrice,
                       InvoiceStatus = order.InvoiceStatus,
                       PaidExchangeAmount = order.PaidExchangeAmount,
                       IsHangUp = order.IsHangUp,
                       OrderStatus = order.OrderStatus,
                       Status = order.Status,
                       CreateDate = order.CreateDate,
                       UpdateDate = order.UpdateDate,
                       Summary = order.Summary,
                       OrderVoyages = orderVoyagesView.Where(t => t.Order.ID == order.ID).ToList(),
                       MainOrderID = order.MainOrderID,
                       EnterCode = order.EnterCode,
                   };
        }
    }

    /// <summary>
    /// 代理订单的视图
    /// </summary>
    public class OrdersView : OrdersViewBase<Models.Order>
    {
        public OrdersView() : base()
        {
        }

        public OrdersView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Order> GetIQueryable()
        {
            return base.GetIQueryable();
        }
    }

    /// <summary>
    /// 代理订单的视图
    /// </summary>
    public class Orders1View : Orders1ViewBase<Models.Order>
    {
        public Orders1View() : base()
        {
        }

        public Orders1View(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Order> GetIQueryable(Expression<Func<Models.Order, bool>> expression, params LambdaExpression[] expressions)
        {
            return base.GetIQueryable(expression, expressions);
        }

        virtual protected Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            var items = new OrderItemsView(this.Reponsitory);
            return items.Where(item => item.OrderID == orderid).ToArray();
        }
    }

    /// <summary>
    /// 草稿订单的视图
    /// </summary>
    public class DraftOrdersView : OrdersViewBase<Models.DraftOrder>
    {
        protected override IQueryable<Models.DraftOrder> GetIQueryable()
        {
            return from order in base.GetIQueryable()
                   where order.OrderStatus == Enums.OrderStatus.Draft
                   select order;
        }

        /// <summary>
        /// 仅供参考
        /// </summary>
        /// <param name="ids"></param>
        public void BatchDelete(string[] ids)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { Status = Enums.Status.Delete }, item => ids.Contains(item.ID));
            }
        }
    }

    /// <summary>
    /// 代理订单的视图
    /// </summary>
    public class DraftOrdersView1 : Orders1ViewBase<Models.DraftOrder>
    {
        public DraftOrdersView1() : base()
        {
        }

        internal DraftOrdersView1(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.DraftOrder> GetIQueryable(Expression<Func<Models.DraftOrder, bool>> expression, params LambdaExpression[] expressions)
        {
            return base.GetIQueryable(expression, expressions).Where(item => item.OrderStatus == Enums.OrderStatus.Draft);
        }

        virtual protected Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            var items = new OrderItemsView(this.Reponsitory);
            return items.Where(item => item.OrderID == orderid).ToArray();
        }
    }

    /// <summary>
    /// 已归类，待报价订单的视图
    /// </summary>
    public class ClassifiedOrdersView : OrdersViewBase<Models.ClassifiedOrder>
    {
        public ClassifiedOrdersView() : base()
        {
        }

        internal ClassifiedOrdersView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<ClassifiedOrder> GetIQueryable()
        {
            return from order in base.GetIQueryable()
                   where order.OrderStatus == Enums.OrderStatus.Classified && !order.IsHangUp
                   select order;
        }
    }

    /// <summary>
    /// 已归类，待报价订单的视图--接口内单下单使用
    /// </summary>
    public class ClassifiedInsideOrdersView : OrdersViewBase<Models.ClassifiedOrder>
    {
        public ClassifiedInsideOrdersView() : base()
        {
        }

        internal ClassifiedInsideOrdersView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<ClassifiedOrder> GetIQueryable()
        {
            return from order in base.GetIQueryable()
                   where order.OrderStatus == Enums.OrderStatus.Classified
                   select order;
        }
    }

    /// <summary>
    /// 已归类，待报价订单的视图
    /// </summary>
    public class ClassifiedOrdersView1 : Orders1ViewBase<Models.Order>
    {
        public ClassifiedOrdersView1() : base()
        {
        }

        internal ClassifiedOrdersView1(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Order> GetIQueryable(Expression<Func<Models.Order, bool>> expression, params LambdaExpression[] expressions)
        {
            return base.GetIQueryable(expression, expressions).Where(item => item.OrderStatus == Enums.OrderStatus.Classified && !item.IsHangUp);
        }

        virtual protected Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            var items = new OrderItemsView(this.Reponsitory);
            return items.Where(item => item.OrderID == orderid).ToArray();
        }
    }

    /// <summary>
    /// 未匹配到货信息的视图
    /// </summary>
    public class UnMatchedOrdersView : OrdersDeliveryViewBase<Models.Order>
    {
        public UnMatchedOrdersView() : base()
        {
        }

        internal UnMatchedOrdersView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Order> GetIQueryable(Expression<Func<Models.Order, bool>> expression, params LambdaExpression[] expressions)
        {

            //var cgDeliveryView = new Needs.Ccs.Services.Views.CgDeliveriesTopViewOrigin(this.Reponsitory);

            //return base.GetIQueryable(expression, expressions).Where(item => item.DeclareFlag == Enums.DeclareFlagEnums.Unable
            //                                                        && !item.IsHangUp
            //                                                        && (item.OrderStatus != Enums.OrderStatus.Returned && item.OrderStatus != Enums.OrderStatus.Canceled)
            //                                                        //ryan 20200701 过滤掉没有到货的订单
            //                                                        && cgDeliveryView.Any(t => t.MainOrderID == item.MainOrderID && (t.OrderID == item.ID || t.OrderID == null))
            //                                                        );

            //return base.GetIQueryable(expression, expressions).Where(item => item.DeclareFlag == Enums.DeclareFlagEnums.Unable
            //                                                       && !item.IsHangUp
            //                                                       && (item.OrderStatus != Enums.OrderStatus.Returned || item.OrderStatus != Enums.OrderStatus.Canceled));

            return base.GetIQueryable(expression, expressions).Where(item =>
                                                                   item.OrderStatus == Enums.OrderStatus.Draft
                                                                   || item.OrderStatus == Enums.OrderStatus.Confirmed
                                                                   || item.OrderStatus == Enums.OrderStatus.Classified
                                                                   || item.OrderStatus == Enums.OrderStatus.Quoted
                                                                   || item.OrderStatus == Enums.OrderStatus.QuoteConfirmed);
        }

        virtual protected Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            var items = new OrderItemsView(this.Reponsitory);
            return items.Where(item => item.OrderID == orderid).ToArray();
        }
    }

    /// <summary>
    /// 已报价，待客户确认订单的视图
    /// </summary>
    public class QuotedOrdersView : OrdersViewBase<Models.QuotedOrder>
    {
        public QuotedOrdersView() : base()
        {
        }

        internal QuotedOrdersView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.QuotedOrder> GetIQueryable()
        {
            return from order in base.GetIQueryable()
                   where order.OrderStatus == Enums.OrderStatus.Quoted && !order.IsHangUp
                   select order;
        }
    }

    /// <summary>
    /// 已报价，待客户确认订单的视图--接口内单下单使用
    /// </summary>
    public class QuotedInsideOrdersView : OrdersViewBase<Models.QuotedOrder>
    {
        public QuotedInsideOrdersView() : base()
        {
        }

        internal QuotedInsideOrdersView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.QuotedOrder> GetIQueryable()
        {
            return from order in base.GetIQueryable()
                   where order.OrderStatus == Enums.OrderStatus.Quoted
                   select order;
        }
    }

    /// <summary>
    /// 已报价，待客户确认订单的视图
    /// </summary>
    public class QuotedOrdersView1 : Orders1ViewBase<Models.Order>
    {
        public QuotedOrdersView1() : base()
        {
        }

        internal QuotedOrdersView1(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Order> GetIQueryable(Expression<Func<Models.Order, bool>> expression, params LambdaExpression[] expressions)
        {
            return base.GetIQueryable(expression, expressions).Where(item => item.OrderStatus == OrderStatus.Quoted && item.IsHangUp == false);
        }

        virtual protected Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            var items = new OrderItemsView(this.Reponsitory);
            return items.Where(item => item.OrderID == orderid).ToArray();
        }
    }

    /// <summary>
    /// 客户确认报价，待报关订单的视图
    /// </summary>
    public class QuoteConfirmedOrdersView : OrdersViewBase<Models.QuoteConfirmedOrder>
    {
        protected override IQueryable<Models.QuoteConfirmedOrder> GetIQueryable()
        {
            return from order in base.GetIQueryable()
                   where order.OrderStatus == Enums.OrderStatus.QuoteConfirmed && !order.IsHangUp
                   select order;
        }
    }

    /// <summary>
    /// 已报关，待出库订单的视图
    /// </summary>
    public class DeclaredOrdersView : OrdersViewBase<Models.DeclaredOrder>
    {
        public DeclaredOrdersView() : base()
        {
        }

        internal DeclaredOrdersView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.DeclaredOrder> GetIQueryable()
        {
            var orderConsignorsView = new OrderConsignorsView(this.Reponsitory);
            var orderItemsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();
            var exitNoticeItemsView = from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNoticeItems>()
                                      join sorting in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>() on item.SortingID equals sorting.ID
                                      where item.Status == (int)Enums.Status.Normal
                                      select new
                                      {
                                          item.ID,
                                          item.Quantity,
                                          sorting.OrderID
                                      };

            return from order in base.GetIQueryable()
                   join orderConsignor in orderConsignorsView on order.ID equals orderConsignor.OrderID
                   join orderItem in orderItemsView on order.ID equals orderItem.OrderID into orderItems
                   join exitNoticeItem in exitNoticeItemsView on order.ID equals exitNoticeItem.OrderID into exitNoticeItems
                   where order.OrderStatus == Enums.OrderStatus.Declared
                   select new Models.DeclaredOrder
                   {
                       ID = order.ID,
                       Type = order.Type,
                       Client = order.Client,
                       UserID = order.UserID,
                       OrderConsignee = order.OrderConsignee,
                       OrderConsignor = orderConsignor,
                       Currency = order.Currency,
                       DeclarePrice = order.DeclarePrice,
                       CreateDate = order.CreateDate,
                       OrderStatus = order.OrderStatus,
                       HasNotified = exitNoticeItems.Count() > 0 && (orderItems.Sum(item => item.Quantity) == exitNoticeItems.Sum(item => item.Quantity))
                   };
        }
    }

    /// <summary>
    /// 已报关，待出库订单的视图
    /// </summary>
    public class DeclaredOrdersView1 : Orders1ViewBase<Models.DeclaredOrder>
    {
        public DeclaredOrdersView1() : base()
        {
        }

        internal DeclaredOrdersView1(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.DeclaredOrder> GetIQueryable(Expression<Func<Models.DeclaredOrder, bool>> expression, params LambdaExpression[] expressions)
        {
            return base.GetIQueryable(expression, expressions).Where(item => item.OrderStatus == Enums.OrderStatus.Declared);
        }

        protected override IEnumerable<DeclaredOrder> OnReadShips(DeclaredOrder[] results)
        {
            var orderIds = results.Select(o => o.ID).ToArray();
            var orderItemsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();
            var exitNoticeItemsView = from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNoticeItems>()
                                      join sorting in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>() on item.SortingID equals sorting.ID
                                      where item.Status == (int)Enums.Status.Normal && orderIds.Contains(sorting.OrderID)
                                      select new
                                      {
                                          item.ID,
                                          item.Quantity,
                                          sorting.OrderID
                                      };
            var clientsView = new ClientsView(this.Reponsitory).Where(item => results.Select(a => a.ClientID).ToArray().Contains(item.ID)).ToArray();


            var orderConsigneesView = new OrderConsigneesView(this.Reponsitory).Where(item => orderIds.Contains(item.OrderID)).ToArray();
            var orderConsignorsView = new OrderConsignorsView(this.Reponsitory).Where(item => orderIds.Contains(item.OrderID)).ToArray();

            return from order in results
                   join exitNoticeItem in exitNoticeItemsView on order.ID equals exitNoticeItem.OrderID into exitNoticeItems
                   join orderItem in orderItemsView on order.ID equals orderItem.OrderID into orderItems
                   join orderConsignee in orderConsigneesView on order.ID equals orderConsignee.OrderID
                   join orderConsignor in orderConsignorsView on order.ID equals orderConsignor.OrderID
                   join client in clientsView on order.ClientID equals client.ID
                   select new DeclaredOrder
                   {
                       ID = order.ID,
                       Type = order.Type,
                       Client = client,
                       UserID = order.UserID,
                       OrderConsignee = order.OrderConsignee,
                       OrderConsignor = orderConsignor,
                       Currency = order.Currency,
                       DeclarePrice = order.DeclarePrice,
                       CreateDate = order.CreateDate,
                       OrderStatus = order.OrderStatus,
                       HasNotified = exitNoticeItems.Count() > 0 && (orderItems.Sum(item => item.Quantity) == exitNoticeItems.Sum(item => item.Quantity))
                   };
        }

        virtual protected Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            var items = new OrderItemsView(this.Reponsitory);
            return items.Where(item => item.OrderID == orderid).ToArray();
        }
    }

    /// <summary>
    /// 已出库，待收货订单的视图
    /// </summary>
    public class WarehouseExitedOrdersView : OrdersViewBase<Models.WarehouseExitedOrder>
    {
        protected override IQueryable<Models.WarehouseExitedOrder> GetIQueryable()
        {
            return from order in base.GetIQueryable()
                   where order.OrderStatus == Enums.OrderStatus.WarehouseExited
                   select order;
        }
    }

    /// <summary>
    /// 已出库，待收货订单的视图
    /// </summary>
    public class WarehouseExitedOrdersView1 : Orders1ViewBase<Models.WarehouseExitedOrder>
    {
        public WarehouseExitedOrdersView1() : base()
        {
        }

        internal WarehouseExitedOrdersView1(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.WarehouseExitedOrder> GetIQueryable(Expression<Func<Models.WarehouseExitedOrder, bool>> expression, params LambdaExpression[] expressions)
        {
            return base.GetIQueryable(expression, expressions).Where(item => item.OrderStatus == Enums.OrderStatus.WarehouseExited);
        }

        virtual protected Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            var items = new OrderItemsView(this.Reponsitory);
            return items.Where(item => item.OrderID == orderid).ToArray();
        }
    }

    /// <summary>
    /// 已完成订单的视图
    /// </summary>
    public class CompletedOrdersView : OrdersViewBase<Models.CompletedOrder>
    {
        protected override IQueryable<Models.CompletedOrder> GetIQueryable()
        {
            return from order in base.GetIQueryable()
                   where order.OrderStatus == Enums.OrderStatus.Completed
                   select order;
        }
    }

    /// <summary>
    /// 已完成订单的视图1
    /// </summary>
    public class CompletedOrdersView1 : Orders1ViewBase<Models.CompletedOrder>
    {
        public CompletedOrdersView1() : base()
        {
        }

        internal CompletedOrdersView1(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.CompletedOrder> GetIQueryable(Expression<Func<Models.CompletedOrder, bool>> expression, params LambdaExpression[] expressions)
        {
            return base.GetIQueryable(expression, expressions).Where(item => item.OrderStatus == Enums.OrderStatus.Completed);
        }

        virtual protected Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            var items = new OrderItemsView(this.Reponsitory);
            return items.Where(item => item.OrderID == orderid).ToArray();
        }
    }

    /// <summary>
    /// 已退回订单的视图
    /// </summary>
    public class ReturnedOrdersView : OrdersViewBase<Models.ReturnedOrder>
    {
        protected override IQueryable<Models.ReturnedOrder> GetIQueryable()
        {
            return from order in base.GetIQueryable()
                   where order.OrderStatus == Enums.OrderStatus.Returned
                   select new Models.ReturnedOrder
                   {
                       ID = order.ID,
                       Type = order.Type,
                       Client = order.Client,
                       UserID = order.UserID,
                       OrderConsignee = order.OrderConsignee,
                       Currency = order.Currency,
                       DeclarePrice = order.DeclarePrice,
                       InvoiceStatus = order.InvoiceStatus,
                       PaidExchangeAmount = order.PaidExchangeAmount,
                       CreateDate = order.CreateDate,
                       OrderStatus = order.OrderStatus,
                       ReturnedSummary = (from log in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderLogs>()
                                          where log.OrderID == order.ID && log.OrderStatus == (int)Enums.OrderStatus.Returned
                                          orderby log.CreateDate descending
                                          select log.Summary).FirstOrDefault()
                   };
        }
    }

    /// <summary>
    ///已退回订单的视图
    /// </summary>
    public class ReturnedOrdersView1 : Orders1ViewBase<Models.ReturnedOrder>
    {
        public ReturnedOrdersView1() : base()
        {
        }

        internal ReturnedOrdersView1(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ReturnedOrder> GetIQueryable(Expression<Func<Models.ReturnedOrder, bool>> expression, params LambdaExpression[] expressions)
        {
            return base.GetIQueryable(expression, expressions).Where(item => item.OrderStatus == Enums.OrderStatus.Returned);
        }

        protected override IEnumerable<ReturnedOrder> OnReadShips(ReturnedOrder[] results)
        {
            var orderIds = results.Select(o => o.ID).ToArray();
            var clientsView = new ClientsView(this.Reponsitory).Where(item => results.Select(a => a.ClientID).ToArray().Contains(item.ID)).ToArray();
            var clientAgreementsView = new ClientAgreementsView(this.Reponsitory).Where(item => results.Select(a => a.ClientAgreementID).ToArray().Contains(item.ID)).ToArray();
            var orderConsigneesView = new OrderConsigneesView(this.Reponsitory).Where(item => orderIds.Contains(item.OrderID)).ToArray();

            return from order in results
                   join client in clientsView on order.ClientID equals client.ID
                   join clientAgreement in clientAgreementsView on order.ClientAgreementID equals clientAgreement.ID
                   join orderConsignee in orderConsigneesView on order.ID equals orderConsignee.OrderID
                   select new ReturnedOrder
                   {
                       ID = order.ID,
                       Type = order.Type,
                       Client = client,
                       UserID = order.UserID,
                       OrderConsignee = orderConsignee,
                       Currency = order.Currency,
                       DeclarePrice = order.DeclarePrice,
                       InvoiceStatus = order.InvoiceStatus,
                       PaidExchangeAmount = order.PaidExchangeAmount,
                       CreateDate = order.CreateDate,
                       OrderStatus = order.OrderStatus,
                       ReturnedSummary = (from log in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderLogs>()
                                          where log.OrderID == order.ID && log.OrderStatus == (int)Enums.OrderStatus.Returned
                                          orderby log.CreateDate descending
                                          select log.Summary).FirstOrDefault()
                   };
        }

        virtual protected Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            var items = new OrderItemsView(this.Reponsitory);
            return items.Where(item => item.OrderID == orderid).ToArray();
        }
    }

    /// <summary>
    /// 已取消订单的视图
    /// </summary>
    public class CanceledOrdersView : OrdersViewBase<Models.CanceledOrder>
    {
        protected override IQueryable<Models.CanceledOrder> GetIQueryable()
        {
            return from order in base.GetIQueryable()
                   where order.OrderStatus == Enums.OrderStatus.Canceled
                   select new Models.CanceledOrder
                   {
                       ID = order.ID,
                       Type = order.Type,
                       Client = order.Client,
                       UserID = order.UserID,
                       OrderConsignee = order.OrderConsignee,
                       Currency = order.Currency,
                       DeclarePrice = order.DeclarePrice,
                       InvoiceStatus = order.InvoiceStatus,
                       PaidExchangeAmount = order.PaidExchangeAmount,
                       CreateDate = order.CreateDate,
                       OrderStatus = order.OrderStatus,
                       CanceledSummary = (from log in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderLogs>()
                                          where log.OrderID == order.ID && log.OrderStatus == (int)Enums.OrderStatus.Canceled
                                          orderby log.CreateDate descending
                                          select log.Summary).FirstOrDefault()
                   };
        }
    }

    /// <summary>
    ///已取消订单的视图
    /// </summary>
    public class CanceledOrdersView1 : Orders1ViewBase<Models.CanceledOrder>
    {
        public CanceledOrdersView1() : base()
        {
        }

        internal CanceledOrdersView1(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.CanceledOrder> GetIQueryable(Expression<Func<Models.CanceledOrder, bool>> expression, params LambdaExpression[] expressions)
        {
            return base.GetIQueryable(expression, expressions).Where(item => item.OrderStatus == Enums.OrderStatus.Canceled);
        }

        protected override IEnumerable<CanceledOrder> OnReadShips(CanceledOrder[] results)
        {
            var orderIds = results.Select(o => o.ID).ToArray();
            var clientsView = new ClientsView(this.Reponsitory).Where(item => results.Select(a => a.ClientID).ToArray().Contains(item.ID)).ToArray();
            var clientAgreementsView = new ClientAgreementsView(this.Reponsitory).Where(item => results.Select(a => a.ClientAgreementID).ToArray().Contains(item.ID)).ToArray();
            var orderConsigneesView = new OrderConsigneesView(this.Reponsitory).Where(item => orderIds.Contains(item.OrderID)).ToArray();

            return from order in results
                   join client in clientsView on order.ClientID equals client.ID
                   join clientAgreement in clientAgreementsView on order.ClientAgreementID equals clientAgreement.ID
                   join orderConsignee in orderConsigneesView on order.ID equals orderConsignee.OrderID
                   select new CanceledOrder
                   {
                       ID = order.ID,
                       Type = order.Type,
                       Client = client,
                       UserID = order.UserID,
                       OrderConsignee = orderConsignee,
                       Currency = order.Currency,
                       DeclarePrice = order.DeclarePrice,
                       InvoiceStatus = order.InvoiceStatus,
                       PaidExchangeAmount = order.PaidExchangeAmount,
                       CreateDate = order.CreateDate,
                       OrderStatus = order.OrderStatus,
                       CanceledSummary = (from log in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderLogs>()
                                          where log.OrderID == order.ID && log.OrderStatus == (int)Enums.OrderStatus.Canceled
                                          orderby log.CreateDate descending
                                          select log.Summary).FirstOrDefault()
                   };
        }

        virtual protected Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            var items = new OrderItemsView(this.Reponsitory);
            return items.Where(item => item.OrderID == orderid).ToArray();
        }
    }

    /// <summary>
    /// 挂起订单的视图
    /// </summary>
    public class HangUpOrdersView : OrdersViewBase<Models.HangUpOrder>
    {
        protected override IQueryable<Models.HangUpOrder> GetIQueryable()
        {
            return from order in base.GetIQueryable()
                   where order.IsHangUp
                   select order;
        }
    }

    /// <summary>
    ///已取消订单的视图
    /// </summary>
    public class HangUpOrdersView1 : Orders1ViewBase<Models.HangUpOrder>
    {
        public HangUpOrdersView1() : base()
        {
        }

        internal HangUpOrdersView1(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.HangUpOrder> GetIQueryable(Expression<Func<Models.HangUpOrder, bool>> expression, params LambdaExpression[] expressions)
        {
            return base.GetIQueryable(expression, expressions).Where(item => item.IsHangUp);
        }

        virtual protected Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            var items = new OrderItemsView(this.Reponsitory);
            return items.Where(item => item.OrderID == orderid).ToArray();
        }
    }

    /// <summary>
    /// 待付汇订单的视图
    /// </summary>
    public class UnPayExchangeOrdersView : OrdersViewBase<Models.UnPayExchangeOrder>
    {
        protected UnPayExchangeOrdersView()
        {

        }

        public UnPayExchangeOrdersView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.UnPayExchangeOrder> GetIQueryable()
        {
            var payExchangeSuppliersView = new Views.OrderPayExchangeSuppliersView(this.Reponsitory);
            return from order in base.GetIQueryable()
                   join payExchangeSupplier in payExchangeSuppliersView on order.ID equals payExchangeSupplier.OrderID into payExchangeSuppliers
                   where order.PaidExchangeAmount < Math.Round(order.DeclarePrice, 2, MidpointRounding.AwayFromZero) &&
                         order.OrderStatus >= Enums.OrderStatus.QuoteConfirmed && order.OrderStatus <= Enums.OrderStatus.Completed
                   select new Models.UnPayExchangeOrder
                   {
                       ID = order.ID,
                       Type = order.Type,
                       Client = order.Client,
                       UserID = order.UserID,
                       ClientAgreement = order.ClientAgreement,
                       OrderConsignee = order.OrderConsignee,
                       Currency = order.Currency,
                       DeclarePrice = Math.Round(order.DeclarePrice, 2, MidpointRounding.AwayFromZero),
                       PaidExchangeAmount = order.PaidExchangeAmount,
                       OrderStatus = order.OrderStatus,
                       CreateDate = order.CreateDate,
                       DeclareDate = (from decHead in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>()
                                      where decHead.OrderID == order.ID
                                      orderby decHead.CreateTime
                                      select decHead.DDate).FirstOrDefault(),
                       Status = order.Status,
                       PayExchangeSuppliers = new OrderPayExchangeSuppliers(payExchangeSuppliers),
                   };
        }
    }

    /// <summary>
    /// 待付汇订单的视图
    /// </summary>
    public class UnPayExchangeOrdersView1 : Orders1ViewBase<UnPayExchangeOrder>
    {
        public UnPayExchangeOrdersView1()
        {
        }

        protected UnPayExchangeOrdersView1(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<UnPayExchangeOrder> GetIQueryable(Expression<Func<UnPayExchangeOrder, bool>> expression, params LambdaExpression[] expressions)
        {
            return base.GetIQueryable(expression, expressions).Where(item => item.PaidExchangeAmount < item.DeclarePrice &&
                          item.OrderStatus >= Enums.OrderStatus.QuoteConfirmed && item.OrderStatus <= Enums.OrderStatus.Completed);
        }


        protected override IEnumerable<UnPayExchangeOrder> OnReadShips(UnPayExchangeOrder[] results)
        {
            var orderIds = results.Select(o => o.ID).ToArray();
            var clientsView = new ClientsView(this.Reponsitory).Where(item => results.Select(a => a.ClientID).ToArray().Contains(item.ID)).ToArray();
            var clientAgreementsView = new ClientAgreementsView(this.Reponsitory).Where(item => results.Select(a => a.ClientAgreementID).ToArray().Contains(item.ID)).ToArray();
            var payExchangeSuppliersView = new Views.OrderPayExchangeSuppliersView(this.Reponsitory);

            return from order in results
                   join client in clientsView on order.ClientID equals client.ID
                   join clientAgreement in clientAgreementsView on order.ClientAgreementID equals clientAgreement.ID
                   join payExchangeSupplier in payExchangeSuppliersView on order.ID equals payExchangeSupplier.OrderID into payExchangeSuppliers

                   select new UnPayExchangeOrder
                   {
                       ID = order.ID,
                       Type = order.Type,
                       Client = client,
                       UserID = order.UserID,
                       ClientAgreement = clientAgreement,
                       OrderConsignee = order.OrderConsignee,
                       Currency = order.Currency,
                       DeclarePrice = order.DeclarePrice,
                       PaidExchangeAmount = order.PaidExchangeAmount,
                       OrderStatus = order.OrderStatus,
                       CreateDate = order.CreateDate,
                       DeclareDate = (from decHead in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>()
                                      where decHead.OrderID == order.ID && decHead.IsSuccess
                                      orderby decHead.CreateTime
                                      select decHead.DDate).FirstOrDefault(),
                       Status = order.Status,
                       PayExchangeSuppliers = new OrderPayExchangeSuppliers(payExchangeSuppliers),
                       MainOrderID = order.MainOrderID,
                   };
        }

        virtual protected Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            var items = new OrderItemsView(this.Reponsitory);
            return items.Where(item => item.OrderID == orderid).ToArray();
        }
    }

    /// <summary>
    /// 待开票订单的视图
    /// </summary>
    public class UnInvoicedOrdersView : OrdersViewBase<Models.UnInvoicedOrder>
    {
        protected override IQueryable<Models.UnInvoicedOrder> GetIQueryable()
        {
            return from order in base.GetIQueryable()
                   where order.InvoiceStatus == Enums.InvoiceStatus.UnInvoiced &&
                         order.OrderStatus >= Enums.OrderStatus.Declared && order.OrderStatus <= Enums.OrderStatus.Completed
                   select order;
        }
    }

    /// <summary>
    /// 开票查询
    /// </summary>
    public class UnInvoicedOrdersView1 : Orders1ViewBase<Models.UnInvoicedOrder>
    {
        public UnInvoicedOrdersView1() : base()
        {
        }

        internal UnInvoicedOrdersView1(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.UnInvoicedOrder> GetIQueryable(Expression<Func<Models.UnInvoicedOrder, bool>> expression, params LambdaExpression[] expressions)
        {
            return base.GetIQueryable(expression, expressions).Where(item => item.InvoiceStatus == InvoiceStatus.UnInvoiced &&
                       item.OrderStatus >= OrderStatus.Declared && item.OrderStatus <= OrderStatus.Completed);
        }

        virtual protected Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            var items = new OrderItemsView(this.Reponsitory);
            return items.Where(item => item.OrderID == orderid).ToArray();
        }
    }


    /// <summary>
    /// 待归类查询
    /// </summary>
    public class UnClassifyOrdersView1 : Orders1ViewBase<Models.UnClassifyOrder>
    {
        public UnClassifyOrdersView1() : base()
        {
        }

        internal UnClassifyOrdersView1(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.UnClassifyOrder> GetIQueryable(Expression<Func<Models.UnClassifyOrder, bool>> expression, params LambdaExpression[] expressions)
        {
            return base.GetIQueryable(expression, expressions).Where(item => item.OrderStatus == OrderStatus.Confirmed);
        }
        virtual protected Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            var items = new OrderItemsView(this.Reponsitory);
            return items.Where(item => item.OrderID == orderid).ToArray();
        }
    }

    /// <summary>
    /// 可维护费用订单的视图
    /// </summary>
    public class FeeMaintenanceOrdersView : OrdersViewBase<Models.FeeMaintenanceOrder>
    {
        protected override IQueryable<Models.FeeMaintenanceOrder> GetIQueryable()
        {
            return from order in base.GetIQueryable()
                   where order.OrderStatus >= Enums.OrderStatus.QuoteConfirmed && order.OrderStatus <= Enums.OrderStatus.Completed
                   select order;
        }
    }

    /// <summary>
    /// Icgoo订单
    /// </summary>
    public class IcgooOrdersView : OrdersViewBase<Models.IcgooOrder>
    {
        public IcgooOrdersView() : base()
        {
        }

        internal IcgooOrdersView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.IcgooOrder> GetIQueryable()
        {
            return base.GetIQueryable();
        }
    }

    public class Orders2ViewBase<T> : UniqueView<T, ScCustomsReponsitory> where T : Models.Order, Interfaces.IOrder, new()
    {
        public Orders2ViewBase()
        {
        }

        internal Orders2ViewBase(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<T> GetIQueryable()
        {
            return from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                   select new T
                   {
                       ID = order.ID,
                       Type = (Enums.OrderType)order.Type,
                       AdminID = order.AdminID,
                       UserID = order.UserID,
                       ClientID = order.ClientID,
                       Currency = order.Currency,
                       CustomsExchangeRate = order.CustomsExchangeRate,
                       RealExchangeRate = order.RealExchangeRate,
                       IsFullVehicle = order.IsFullVehicle,
                       IsLoan = order.IsLoan,
                       PackNo = order.PackNo,
                       WarpType = order.WarpType,
                       DeclarePrice = order.DeclarePrice,
                       InvoiceStatus = (Enums.InvoiceStatus)order.InvoiceStatus,
                       PaidExchangeAmount = order.PaidExchangeAmount,
                       IsHangUp = order.IsHangUp,
                       OrderStatus = (Enums.OrderStatus)order.OrderStatus,
                       Status = (Enums.Status)order.Status,
                       CreateDate = order.CreateDate,
                       UpdateDate = order.UpdateDate,
                       Summary = order.Summary,
                       MainOrderID = order.MainOrderId,
                       DeclareFlag = order.DeclareFlag == null ? Enums.DeclareFlagEnums.Unable : (Enums.DeclareFlagEnums)order.DeclareFlag,
                       CollectStatus = order.CollectStatus == null ? Enums.CollectStatusEnums.UnCollected : (Enums.CollectStatusEnums)order.CollectStatus
                   };
        }
    }

    /// <summary>
    /// 只有Order表
    /// </summary>
    public class Orders2View : Orders2ViewBase<Models.Order>
    {
        public Orders2View() : base()
        {
        }

        public Orders2View(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Order> GetIQueryable()
        {
            return base.GetIQueryable();
        }
    }

    /// <summary>
    /// 到货信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OrdersDeliveryViewBase<T> : Needs.Linq.Generic.Unique1Classics<T, ScCustomsReponsitory> where T : Models.Order, Interfaces.IOrder, new()
    {
        public OrdersDeliveryViewBase()
        {
        }

        internal OrdersDeliveryViewBase(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<T> GetIQueryable(Expression<Func<T, bool>> expression, params LambdaExpression[] expressions)
        {
            var linq = from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                       where order.Status == (int)Enums.Status.Normal
                       select new T
                       {
                           ID = order.ID,
                           Type = (Enums.OrderType)order.Type,
                           AdminID = order.AdminID,
                           UserID = order.UserID,
                           ClientID = order.ClientID,
                           ClientAgreementID = order.ClientAgreementID,
                           Currency = order.Currency,
                           CustomsExchangeRate = order.CustomsExchangeRate,
                           RealExchangeRate = order.RealExchangeRate,
                           IsFullVehicle = order.IsFullVehicle,
                           IsLoan = order.IsLoan,
                           PackNo = order.PackNo,
                           WarpType = order.WarpType,
                           DeclarePrice = order.DeclarePrice,
                           InvoiceStatus = (Enums.InvoiceStatus)order.InvoiceStatus,
                           PaidExchangeAmount = order.PaidExchangeAmount,
                           IsHangUp = order.IsHangUp,
                           OrderStatus = (Enums.OrderStatus)order.OrderStatus,
                           Status = (Enums.Status)order.Status,
                           CreateDate = order.CreateDate,
                           UpdateDate = order.UpdateDate,
                           Summary = order.Summary,
                           MainOrderID = order.MainOrderId,
                           DeclareFlag = order.DeclareFlag == null ? Enums.DeclareFlagEnums.Unable : (Enums.DeclareFlagEnums)order.DeclareFlag
                       };

            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<T, bool>>);
            }

            return linq.Where(expression);
        }

        protected override IEnumerable<T> OnReadShips(T[] results)
        {
            var orderIds = results.Select(o => o.ID).ToArray();
            var mainOrderIds = results.Select(o => o.MainOrderID).ToArray();
            var clientsView = new ClientsView(this.Reponsitory).Where(item => results.Select(a => a.ClientID).ToArray().Contains(item.ID)).ToArray();
            var clientAgreementsView = new ClientAgreementsView(this.Reponsitory).Where(item => results.Select(a => a.ClientAgreementID).ToArray().Contains(item.ID)).ToArray();
            var orderConsigneesView = new OrderConsigneesView(this.Reponsitory).Where(item => orderIds.Contains(item.OrderID)).ToArray();
            var orderVoyagesView = new OrderVoyagesOriginView(this.Reponsitory).Where(item => orderIds.Contains(item.Order.ID)).ToArray();
            var deliveryInfoView = new PackingsView(this.Reponsitory).Where(item => orderIds.Contains(item.OrderID)).ToArray();
            var finalTinyOrderID = deliveryInfoView.Select(t => t.OrderID).Distinct();
            var linq_entryNotice = from c in Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNotices>().
                                     Where(item => orderIds.Contains(item.OrderID) && item.Status == (int)Enums.Status.Normal)
                                   select new
                                   {
                                       OrderID = c.OrderID,
                                       EntryNoticeStatus = (Enums.EntryNoticeStatus)c.EntryNoticeStatus
                                   };

            var ienums_entryNotice = linq_entryNotice.ToArray();

            return from order in results
                   join client in clientsView on order.ClientID equals client.ID
                   join clientAgreement in clientAgreementsView on order.ClientAgreementID equals clientAgreement.ID
                   join orderConsignee in orderConsigneesView on order.ID equals orderConsignee.OrderID
                   join entryNotice in ienums_entryNotice on order.ID equals entryNotice.OrderID
                   join deliveryInfo in finalTinyOrderID on order.ID equals deliveryInfo into g
                   from c in g.DefaultIfEmpty()
                   select new T
                   {
                       ID = order.ID,
                       Type = order.Type,
                       AdminID = order.AdminID,
                       UserID = order.UserID,
                       ClientID = order.ClientID,
                       Client = client,
                       ClientAgreementID = order.ClientAgreementID,
                       ClientAgreement = clientAgreement,
                       OrderConsignee = orderConsignee,
                       Currency = order.Currency,
                       CustomsExchangeRate = order.CustomsExchangeRate,
                       RealExchangeRate = order.RealExchangeRate,
                       IsFullVehicle = order.IsFullVehicle,
                       IsLoan = order.IsLoan,
                       PackNo = order.PackNo,
                       WarpType = order.WarpType,
                       DeclarePrice = order.DeclarePrice,
                       InvoiceStatus = order.InvoiceStatus,
                       PaidExchangeAmount = order.PaidExchangeAmount,
                       IsHangUp = order.IsHangUp,
                       OrderStatus = order.OrderStatus,
                       Status = order.Status,
                       CreateDate = order.CreateDate,
                       UpdateDate = order.UpdateDate,
                       Summary = order.Summary,
                       OrderVoyages = orderVoyagesView.Where(t => t.Order.ID == order.ID).ToList(),
                       MainOrderID = order.MainOrderID,
                       IsArrived = c == null ? false : true,
                       DeclareFlag = order.DeclareFlag,
                       EntryNoticeStatus = entryNotice.EntryNoticeStatus
                   };
        }
    }


    /// <summary>
    /// 销售合同的订单视图
    /// </summary>
    public class SalesContractOrdersView : OrdersViewBase<Models.SalesContractOrder>
    {
        protected override IQueryable<Models.SalesContractOrder> GetIQueryable()
        {
            return from order in base.GetIQueryable()
                   where order.OrderStatus >= Enums.OrderStatus.Quoted && order.OrderStatus <= Enums.OrderStatus.Completed
                   select order;
        }
    }

    public class UnCollectedOrdersView : Orders1ViewBase<UnCollectedOrder>
    {
        public UnCollectedOrdersView()
        {
        }

        protected UnCollectedOrdersView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<UnCollectedOrder> GetIQueryable(Expression<Func<UnCollectedOrder, bool>> expression, params LambdaExpression[] expressions)
        {
            //大于0是已申请付汇的，但是不能判断有没有审核
            return base.GetIQueryable(expression, expressions).Where(item => item.PaidExchangeAmount > 0 && item.CollectStatus != CollectStatusEnums.Collected &&
                          item.OrderStatus >= Enums.OrderStatus.QuoteConfirmed && item.OrderStatus <= Enums.OrderStatus.Completed);
        }


        protected override IEnumerable<UnCollectedOrder> OnReadShips(UnCollectedOrder[] results)
        {
            var orderIds = results.Select(o => o.ID).ToArray();
            var clientsView = new ClientsView(this.Reponsitory).Where(item => results.Select(a => a.ClientID).ToArray().Contains(item.ID)).ToArray();


            return from order in results
                   join client in clientsView on order.ClientID equals client.ID
                   select new UnCollectedOrder
                   {
                       ID = order.ID,
                       Type = order.Type,
                       Client = client,
                       UserID = order.UserID,
                       Currency = order.Currency,
                       DeclarePrice = order.DeclarePrice,
                       PaidExchangeAmount = order.PaidExchangeAmount,
                       OrderStatus = order.OrderStatus,
                       CreateDate = order.CreateDate,
                       DeclareDate = (from decHead in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>()
                                      where decHead.OrderID == order.ID && decHead.IsSuccess
                                      orderby decHead.CreateTime
                                      select decHead.DDate).FirstOrDefault(),
                       Status = order.Status,
                       CollectedAmount = order.CollectedAmount,
                       MainOrderID = order.MainOrderID,
                   };
        }

        virtual protected Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            var items = new OrderItemsView(this.Reponsitory);
            return items.Where(item => item.OrderID == orderid).ToArray();
        }
    }



    public class RiskOrderViewRJ : QueryView<Models.Order, ScCustomsReponsitory>
    {
        public RiskOrderViewRJ()
        {
        }

        internal RiskOrderViewRJ(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected RiskOrderViewRJ(ScCustomsReponsitory reponsitory, IQueryable<Models.Order> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Models.Order> GetIQueryable()
        {
            var ordersView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clientView = new ClientsView(this.Reponsitory);
            var advanceRecordView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdvanceRecords>();
            var linq_Advance = from advance in advanceRecordView
                               where advance.Status == (int)Enums.Status.Normal
                               group advance by advance.OrderID into g_advance
                               select new
                               {
                                   OrderID = g_advance.Key,
                                   AdvanceAmount = g_advance.Sum(t => t.Amount - t.PaidAmount)
                               };

            var iQuery = from order in ordersView
                         join client in clientView on order.ClientID equals client.ID
                         join advance in linq_Advance on order.ID equals advance.OrderID into temp_advance
                         from advance in temp_advance.DefaultIfEmpty()
                         where order.OrderStatus != (int)Enums.OrderStatus.Returned && order.Status != (int)Enums.OrderStatus.Draft && order.Status != (int)Enums.OrderStatus.Canceled
                         && order.Status == (int)Enums.Status.Normal
                         //orderby order.CreateDate descending
                         select new Models.Order
                         {
                             ID = order.ID,
                             Type = (Enums.OrderType)order.Type,
                             AdminID = order.AdminID,
                             UserID = order.UserID,
                             ClientID = order.ClientID,
                             ClientAgreementID = order.ClientAgreementID,
                             Currency = order.Currency,
                             CustomsExchangeRate = order.CustomsExchangeRate,
                             RealExchangeRate = order.RealExchangeRate,
                             IsFullVehicle = order.IsFullVehicle,
                             IsLoan = order.IsLoan,
                             PackNo = order.PackNo,
                             WarpType = order.WarpType,
                             DeclarePrice = order.DeclarePrice,
                             InvoiceStatus = (Enums.InvoiceStatus)order.InvoiceStatus,
                             PaidExchangeAmount = order.PaidExchangeAmount,
                             IsHangUp = order.IsHangUp,
                             OrderStatus = (Enums.OrderStatus)order.OrderStatus,
                             Status = (Enums.Status)order.Status,
                             CreateDate = order.CreateDate,
                             UpdateDate = order.UpdateDate,
                             Summary = order.Summary,
                             MainOrderID = order.MainOrderId,
                             //DeclareFlag = order.DeclareFlag == null ? Enums.DeclareFlagEnums.Unable : (Enums.DeclareFlagEnums)order.DeclareFlag,
                             //CollectStatus = order.CollectStatus == null ? Enums.CollectStatusEnums.UnCollected : (Enums.CollectStatusEnums)order.CollectStatus,
                             CollectedAmount = order.CollectedAmount,
                             EnterCode = order.EnterCode,
                             Client = client,
                             AdvanceAmount = advance != null ? advance.AdvanceAmount : 0
                         };
            return iQuery;
        }

        //public List<Models.Order> ToSum()
        //{
        //    IQueryable<Models.Order> iquery = this.IQueryable.Cast<Models.Order>().OrderByDescending(item => item.CreateDate);
        //    int total = iquery.Count();

        //    //获取数据
        //    var ienum_myOrders = iquery.ToArray();

        //    //var ordersView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
        //    //var advanceRecordView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdvanceRecords>();

        //    //var ienums_linq = from order in ordersView
                              
        //    //                  group 
        //    //                  select new Models.Order { 
                              
                              
        //    //                  }



        //    return ienum_myOrders.ToList();

        //}

        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Models.Order> iquery = this.IQueryable.Cast<Models.Order>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_myOrders = iquery.ToArray();

            //获取订单的ID
            var ordersID = ienum_myOrders.Select(item => item.ID);


            var orderVoyagesView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderVoyages>();
            //var advanceRecordView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdvanceRecords>();
            //var clientView = new ClientsView(this.Reponsitory); //this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            //var companyView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();

            #region 客户资料

            //var linq_Client = from client in clientView
            //                  where clientID.Contains(client.ID)
            //                  select client;

            //var ienums_Client = linq_Client.ToArray();

            #endregion

            #region 特殊类型

            var linq_OrderVoyage = from voyage in orderVoyagesView
                                   where ordersID.Contains(voyage.OrderID)
                                   && voyage.Status == (int)Enums.Status.Normal
                                   //group voyage by voyage.OrderID into g_voyage
                                   select new OrderVoyage
                                   {
                                       ID = voyage.OrderID,
                                       Type = (Enums.OrderSpecialType)voyage.Type
                                   };

            var ienums_OrderVoyage = linq_OrderVoyage.ToArray();

            #endregion

            #region 垫资金额

            //var linq_Advance = from advance in advanceRecordView
            //                   where ordersID.Contains(advance.OrderID)
            //                       && advance.Status == (int)Enums.Status.Normal
            //                   group advance by advance.OrderID into g_advance
            //                   select new
            //                   {
            //                       OrderID = g_advance.Key,
            //                       AdvanceAmount = g_advance.Sum(t => t.Amount - t.PaidAmount)
            //                   };

            //var ienums_Advance = linq_Advance.ToArray();

            #endregion

            var ienums_linq = from order in ienum_myOrders
                              //join client in linq_Client on order.ClientID equals client.ID

                              //join advance in linq_Advance on order.ID equals advance.OrderID
                              //into ienums_advance
                              //from advance in ienums_advance.DefaultIfEmpty()

                              let voyage = linq_OrderVoyage.Where(t=>t.ID == order.ID).ToList()

                              select new Models.Order
                              {
                                  ID = order.ID,
                                  Type = (Enums.OrderType)order.Type,
                                  AdminID = order.AdminID,
                                  UserID = order.UserID,
                                  ClientID = order.ClientID,
                                  ClientAgreementID = order.ClientAgreementID,
                                  Currency = order.Currency,
                                  CustomsExchangeRate = order.CustomsExchangeRate,
                                  RealExchangeRate = order.RealExchangeRate,
                                  IsFullVehicle = order.IsFullVehicle,
                                  IsLoan = order.IsLoan,
                                  PackNo = order.PackNo,
                                  WarpType = order.WarpType,
                                  DeclarePrice = order.DeclarePrice,
                                  InvoiceStatus = (Enums.InvoiceStatus)order.InvoiceStatus,
                                  PaidExchangeAmount = order.PaidExchangeAmount,
                                  IsHangUp = order.IsHangUp,
                                  OrderStatus = (Enums.OrderStatus)order.OrderStatus,
                                  Status = (Enums.Status)order.Status,
                                  CreateDate = order.CreateDate,
                                  UpdateDate = order.UpdateDate,
                                  Summary = order.Summary,
                                  MainOrderID = order.MainOrderID,
                                  DeclareFlag = order.DeclareFlag == null ? Enums.DeclareFlagEnums.Unable : (Enums.DeclareFlagEnums)order.DeclareFlag,
                                  CollectStatus = order.CollectStatus == null ? Enums.CollectStatusEnums.UnCollected : (Enums.CollectStatusEnums)order.CollectStatus,
                                  CollectedAmount = order.CollectedAmount,
                                  EnterCode = order.EnterCode,
                                  Client = order.Client,
                                  OrderVoyages = voyage,
                                  AdvanceAmount = order.AdvanceAmount
                              };

            var results = ienums_linq;

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return results.Select(item =>
                {
                    Models.Order o = item;
                    return o;
                }).ToArray();
            }


            Func<Needs.Ccs.Services.Models.Order, object> convert = order => new
            {
                order.MainOrderID,
                order.ID,
                order.Client.ClientCode,
                ClientName = order.Client.Company.Name,
                DeclarePrice = order.DeclarePrice.ToRound(2).ToString("0.00"),
                order.Currency,
                CreateDate = order.CreateDate.ToShortDateString(),
                InvoiceStatus = order.InvoiceStatus.GetDescription(),
                PaidAmount = order.PaidExchangeAmount.ToRound(2).ToString("0.00"),
                UnpaidAmount = (order.DeclarePrice - order.PaidExchangeAmount).ToString("0.00"),
                PayExchangeStatus = order.PayExchangeStatus.GetDescription(),
                OrderStatusValue = order.OrderStatus,
                OrderStatus = order.OrderStatus.GetDescription(),
                SpecialType = GenerateOrderSpecialType(order.OrderVoyages),
                AdvanceAmount = order.AdvanceAmount
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
        /// 订单特殊类型
        /// </summary>
        /// <param name="orderVoyages"></param>
        /// <returns></returns>
        public string GenerateOrderSpecialType(List<Needs.Ccs.Services.Models.OrderVoyage> orderVoyages)
        {
            var result = string.Empty;
            orderVoyages.ForEach(t => {
                result += t.Type.GetDescription() + "|";
            });

            result = result.TrimEnd('|');

            return string.IsNullOrEmpty(result) ? "-" : result;
        }

        /// <summary>
        /// 查询订单号
        /// </summary>
        /// <param name="orderID">订单ID</param>
        /// <returns>视图</returns>
        public RiskOrderViewRJ SearchByOrderID(string orderID)
        {
            var linq = from query in this.IQueryable
                       where query.ID.Contains(orderID)
                       select query;

            var view = new RiskOrderViewRJ(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 查询客户编号
        /// </summary>
        /// <param name="ClientCode"></param>
        /// <returns></returns>
        public RiskOrderViewRJ SearchByClientCode(string ClientCode)
        {
            var linq = from query in this.IQueryable
                       where query.EnterCode.Contains(ClientCode)
                       select query;

            var view = new RiskOrderViewRJ(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 查询订单状态
        /// </summary>
        /// <param name="ClientCode"></param>
        /// <returns></returns>
        public RiskOrderViewRJ SearchByOrderStatus(int OrderStatus)
        {
            var linq = from query in this.IQueryable
                       where query.OrderStatus == (Enums.OrderStatus)OrderStatus
                       select query;

            var view = new RiskOrderViewRJ(this.Reponsitory, linq);
            return view;
        }

        public RiskOrderViewRJ SearchByFrom(DateTime fromtime)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate >= fromtime
                       select query;

            var view = new RiskOrderViewRJ(this.Reponsitory, linq);
            return view;
        }

        public RiskOrderViewRJ SearchByTo(DateTime totime)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate <= totime
                       select query;

            var view = new RiskOrderViewRJ(this.Reponsitory, linq);
            return view;
        }

        public RiskOrderViewRJ SearchByClientType(int ClientType)
        {
            var linq = from query in this.IQueryable
                       where query.Client.ClientType == (Enums.ClientType)ClientType
                       select query;

            var view = new RiskOrderViewRJ(this.Reponsitory, linq);
            return view;
        }

        public RiskOrderViewRJ SearchByAdvance(bool isAdvance)
        {
            var linq = from query in this.IQueryable
                       where query.AdvanceAmount > 0
                       select query;

            var view = new RiskOrderViewRJ(this.Reponsitory, linq);
            return view;
        }
    }

}