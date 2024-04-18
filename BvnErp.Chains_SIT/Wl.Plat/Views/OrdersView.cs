using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Linq;
using Needs.Wl.User.Plat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Needs.Wl.User.Plat.Views
{
    /// <summary>
    /// 我的订单
    /// </summary>
    public class UserOrdersView : OrdersView
    {
        IPlatUser User;
        internal UserOrdersView(IPlatUser user)
        {
            this.User = user;
        }

        internal UserOrdersView(IPlatUser user, ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
            this.User = user;
        }

        protected override IQueryable<Order> GetIQueryable()
        {
            if (this.User.IsMain)
            {
                return from entity in base.GetIQueryable()
                       where entity.Client.ID == this.User.ClientID
                       select entity;
            }
            else
            {
                return from entity in base.GetIQueryable()
                       where entity.UserID == this.User.ID
                       select entity;
            }
        }
    }
    /// <summary>
    /// 我的订单
    /// </summary>
    public class UserOrdersView1 : Orders1View
    {
        IPlatUser User;
        internal UserOrdersView1(IPlatUser user)
        {
            this.User = user;
        }

        internal UserOrdersView1(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Needs.Ccs.Services.Models.Order> GetIQueryable(Expression<Func<Needs.Ccs.Services.Models.Order, bool>> expression, params LambdaExpression[] expressions)
        {
            if (this.User.IsMain)
            {
                return base.GetIQueryable(expression, expressions).Where(item => item.ClientID == this.User.ClientID).OrderByDescending(item => item.CreateDate);
            }
            return base.GetIQueryable(expression, expressions).Where(item => item.UserID == this.User.ID).OrderByDescending(item => item.CreateDate);
        }

        protected override Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            return base.GetItems(orderid);
        }
    }

    //public class OrderExtendsView : UniqueView<UserOrderExtends, ScCustomsReponsitory>
    //{
    //    IPlatUser User;

    //    public OrderExtendsView(IPlatUser user)
    //    {
    //        this.User = user;
    //    }

    //    protected override IQueryable<UserOrderExtends> GetIQueryable()
    //    {
    //        var exitNotice = new UserExitNoticeView(this.Reponsitory);
    //        var oders = new UserOrdersView(User, this.Reponsitory);
    //        return from order in oders
    //               join notice in exitNotice on order.ID equals notice.OrderId into notices
    //               from _notice in notices.DefaultIfEmpty()
    //               select new UserOrderExtends
    //               {
    //                   ID = order.ID,
    //                   Type = order.Type,
    //                   AdminID = order.AdminID,
    //                   UserID = order.UserID,
    //                   Client = order.Client,
    //                   ClientAgreement = order.ClientAgreement,
    //                   OrderConsignee = order.OrderConsignee,
    //                   Currency = order.Currency,
    //                   CustomsExchangeRate = order.CustomsExchangeRate,
    //                   RealExchangeRate = order.RealExchangeRate,
    //                   IsFullVehicle = order.IsFullVehicle,
    //                   IsLoan = order.IsLoan,
    //                   PackNo = order.PackNo,
    //                   WarpType = order.WarpType,
    //                   DeclarePrice = order.DeclarePrice,
    //                   InvoiceStatus = order.InvoiceStatus,
    //                   PaidExchangeAmount = order.PaidExchangeAmount,
    //                   IsHangUp = order.IsHangUp,
    //                   OrderStatus = order.OrderStatus,
    //                   Status = order.Status,
    //                   CreateDate = order.CreateDate,
    //                   UpdateDate = order.UpdateDate,
    //                   Summary = order.Summary,
    //                   IsShowTihou = notices.Count() == 0 ? false : true
    //               };
    //    }
    //}

    /// <summary>
    /// 订单扩展
    /// </summary>
    public class OrderExtendsView1 : Orders1ViewBase<UserOrderExtends>
    {
        IPlatUser User;

        public OrderExtendsView1(IPlatUser user)
        {
            this.User = user;
        }

        protected override IQueryable<UserOrderExtends> GetIQueryable(Expression<Func<UserOrderExtends, bool>> expression, params LambdaExpression[] expressions)
        {
            if (this.User.IsMain)
            {
                return base.GetIQueryable(expression, expressions).Where(item => item.ClientID == this.User.ClientID).OrderByDescending(item => item.CreateDate);
            }
            else
            {
                return base.GetIQueryable(expression, expressions).Where(item => item.UserID == this.User.ID).OrderByDescending(item => item.CreateDate);
            }
        }

        protected override IEnumerable<UserOrderExtends> OnReadShips(UserOrderExtends[] results)
        {
            var orderIds = results.Select(o => o.ID).ToArray();
            var clientsView = new ClientsView(this.Reponsitory).Where(item => results.Select(a => a.ClientID).ToArray().Contains(item.ID)).ToArray();
            var clientAgreementsView = new ClientAgreementsView(this.Reponsitory).Where(item => results.Select(a => a.ClientAgreementID).ToArray().Contains(item.ID)).ToArray();
            var orderConsigneesView = new OrderConsigneesView(this.Reponsitory).Where(item => orderIds.Contains(item.OrderID)).ToArray();
            var exitNotice = new UserExitNoticeView(this.Reponsitory).Where(item => orderIds.Contains(item.OrderId)).ToArray();
            var orderVoyagesView = new OrderVoyagesOriginView(this.Reponsitory).Where(item => orderIds.Contains(item.Order.ID)).ToArray();
            var isUnAuditedControlView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.IsUnAuditedControlView>().Where(item => orderIds.Contains(item.OrderID)).ToArray();

            return from order in results
                   join client in clientsView on order.ClientID equals client.ID
                   join clientAgreement in clientAgreementsView on order.ClientAgreementID equals clientAgreement.ID
                   join orderConsignee in orderConsigneesView on order.ID equals orderConsignee.OrderID
                   join isUnAuditedControl in isUnAuditedControlView on order.ID equals isUnAuditedControl.OrderID into isUnAuditedControlView2
                   from isUnAuditedControl in isUnAuditedControlView2.DefaultIfEmpty()
                   select new UserOrderExtends
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
                       IsShowTihou = exitNotice.Where(item => item.OrderId == order.ID).Count() == 0 ? false : true,
                       OrderVoyages = orderVoyagesView.Where(t => t.Order.ID == order.ID).ToList(),
                       DeclareDate = (from decHead in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>()
                                      where decHead.OrderID == order.ID
                                      orderby decHead.CreateTime
                                      select decHead.DDate).FirstOrDefault(),
                       IsBecauseModified = isUnAuditedControl != null ?
                                            (order.IsHangUp == true) && (isUnAuditedControl.IsUnAuditedDeleteModel > 0 || isUnAuditedControl.IsUnAuditedChangeQuantity > 0)
                                            : false,
                       MainOrderID = order.MainOrderID,
                   };
        }

        virtual protected Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            var items = new OrderItemsView(this.Reponsitory);
            return items.Where(item => item.OrderID == orderid).ToArray();
        }
    }

    ///// <summary>
    ///// 草稿订单的视图
    ///// </summary>
    //public class UserDraftOrdersView : DraftOrdersView
    //{
    //    IPlatUser User;

    //    internal UserDraftOrdersView(IPlatUser user)
    //    {
    //        this.User = user;
    //    }

    //    protected override IQueryable<Needs.Ccs.Services.Models.DraftOrder> GetIQueryable()
    //    {
    //        if (this.User.IsMain)
    //        {
    //            return from entity in base.GetIQueryable()
    //                   where entity.Client.ID == this.User.ClientID
    //                   select entity;
    //        }
    //        else
    //        {
    //            return from entity in base.GetIQueryable()
    //                   where entity.UserID == this.User.ID
    //                   select entity;
    //        }
    //    }
    //}

    /// <summary>
    /// 草稿订单的视图
    /// </summary>
    public class UserDraftOrdersView1 : DraftOrdersView1
    {
        IPlatUser User;

        public UserDraftOrdersView1(IPlatUser user)
        {
            this.User = user;
        }

        protected override IQueryable<DraftOrder> GetIQueryable(Expression<Func<DraftOrder, bool>> expression, params LambdaExpression[] expressions)
        {
            if (this.User.IsMain)
            {
                return base.GetIQueryable(expression, expressions).Where(item => item.ClientID == this.User.ClientID).OrderByDescending(item => item.CreateDate);
            }
            else
            {
                return base.GetIQueryable(expression, expressions).Where(item => item.UserID == this.User.ID).OrderByDescending(item => item.CreateDate);
            }
        }

        protected override Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            return base.GetItems(orderid);
        }

    }

    ///// <summary>
    ///// 已报价，待客户确认订单的视图
    ///// </summary>
    //public class UserQuotedOrdersView : QuotedOrdersViewExtends
    //{
    //    IPlatUser User;

    //    internal UserQuotedOrdersView(IPlatUser user)
    //    {
    //        this.User = user;
    //    }

    //    protected override IQueryable<Needs.Ccs.Services.Models.QuotedOrder> GetIQueryable()
    //    {
    //        if (this.User.IsMain)
    //        {
    //            var a = base.GetIQueryable();

    //            return from entity in base.GetIQueryable()
    //                   where entity.Client.ID == this.User.ClientID
    //                   select entity;
    //        }
    //        else
    //        {
    //            return from entity in base.GetIQueryable()
    //                   where entity.UserID == this.User.ID
    //                   select entity;
    //        }
    //    }
    //}

    /// <summary>
    /// 已报价，待客户确认订单的视图
    /// </summary>
    public class UserQuotedOrdersView1 : QuotedOrdersViewExtends1
    {
        IPlatUser User;

        internal UserQuotedOrdersView1(IPlatUser user)
        {
            this.User = user;
        }

        protected override IQueryable<QuotedOrder> GetIQueryable(Expression<Func<QuotedOrder, bool>> expression, params LambdaExpression[] expressions)
        {
            if (this.User.IsMain)
            {
                return base.GetIQueryable(expression, expressions).Where(item => item.ClientID == this.User.ClientID).OrderByDescending(item => item.CreateDate);
            }
            else
            {
                return base.GetIQueryable(expression, expressions).Where(item => item.UserID == this.User.ID).OrderByDescending(item => item.CreateDate);
            }
        }

        protected override Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            return base.GetItems(orderid);
        }
    }

    /// <summary>
    /// 已报价，待客户确认订单的视图2
    /// </summary>
    public class UserQuotedOrdersView2 : QuotedOrdersViewExtends2
    {
        IPlatUser User;

        internal UserQuotedOrdersView2(IPlatUser user)
        {
            this.User = user;
        }

        protected override IQueryable<QuotedOrder> GetIQueryable(Expression<Func<QuotedOrder, bool>> expression, params LambdaExpression[] expressions)
        {
            if (this.User.IsMain)
            {
                return base.GetIQueryable(expression, expressions).Where(item => item.ClientID == this.User.ClientID).OrderByDescending(item => item.CreateDate);
            }
            else
            {
                return base.GetIQueryable(expression, expressions).Where(item => item.UserID == this.User.ID).OrderByDescending(item => item.CreateDate);
            }
        }

        protected override Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            return base.GetItems(orderid);
        }
    }

    /// <summary>
    /// 待付汇订单的视图
    /// </summary>
    public class UserUnPayExchangeOrdersView : UnPayExchangeOrdersView
    {
        IPlatUser User;

        internal UserUnPayExchangeOrdersView(IPlatUser user)
        {
            this.User = user;
        }

        internal UserUnPayExchangeOrdersView(IPlatUser user, ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
            this.User = user;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.UnPayExchangeOrder> GetIQueryable()
        {
            if (this.User.IsMain)
            {
                return from entity in base.GetIQueryable()
                       where entity.Client.ID == this.User.ClientID
                       select entity;
            }
            else
            {
                return from entity in base.GetIQueryable()
                       where entity.UserID == this.User.ID
                       select entity;
            }
        }
    }

    /// <summary>
    /// 待付汇订单的视图1
    /// </summary>
    public class UserUnPayExchangeOrdersView1 : UnPayExchangeOrdersView1
    {
        IPlatUser User;

        internal UserUnPayExchangeOrdersView1(IPlatUser user)
        {
            this.User = user;
        }

        internal UserUnPayExchangeOrdersView1(IPlatUser user, ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
            this.User = user;
        }

        protected override IQueryable<UnPayExchangeOrder> GetIQueryable(Expression<Func<UnPayExchangeOrder, bool>> expression, params LambdaExpression[] expressions)
        {
            if (this.User.IsMain)
            {
                return base.GetIQueryable(expression, expressions).Where(item => item.ClientID == this.User.ClientID).OrderByDescending(item => item.CreateDate);
            }
            else
            {
                return base.GetIQueryable(expression, expressions).Where(item => item.UserID == this.User.ID).OrderByDescending(item => item.CreateDate);
            }
        }

        protected override Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            return base.GetItems(orderid);
        }
    }

    ///// <summary>
    ///// 待收货订单的视图
    ///// </summary>
    //public class UserWarehouseExitedOrdersView : WarehouseExitedOrdersView
    //{
    //    IPlatUser User;

    //    internal UserWarehouseExitedOrdersView(IPlatUser user)
    //    {
    //        this.User = user;
    //    }

    //    protected override IQueryable<Needs.Ccs.Services.Models.WarehouseExitedOrder> GetIQueryable()
    //    {
    //        if (this.User.IsMain)
    //        {
    //            return from entity in base.GetIQueryable()
    //                   where entity.Client.ID == this.User.ClientID
    //                   select entity;
    //        }
    //        else
    //        {
    //            return from entity in base.GetIQueryable()
    //                   where entity.UserID == this.User.ID
    //                   select entity;
    //        }
    //    }
    //}

    /// <summary>
    /// 待收货订单的视图1
    /// </summary>
    public class UserWarehouseExitedOrdersView1 : WarehouseExitedOrdersView1
    {
        IPlatUser User;

        internal UserWarehouseExitedOrdersView1(IPlatUser user)
        {
            this.User = user;
        }

        protected override IQueryable<WarehouseExitedOrder> GetIQueryable(Expression<Func<WarehouseExitedOrder, bool>> expression, params LambdaExpression[] expressions)
        {
            if (this.User.IsMain)
            {
                return base.GetIQueryable(expression, expressions).Where(item => item.ClientID == this.User.ClientID).OrderByDescending(item => item.CreateDate);
            }
            else
            {
                return base.GetIQueryable(expression, expressions).Where(item => item.UserID == this.User.ID).OrderByDescending(item => item.CreateDate);
            }
        }

        protected override Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            return base.GetItems(orderid);
        }
    }

    /// <summary>
    /// 已挂起订单
    /// </summary>
    public class UserHangUpOrdersView : HangUpOrdersView
    {
        IPlatUser User;

        internal UserHangUpOrdersView(IPlatUser user)
        {
            this.User = user;
        }


        protected override IQueryable<Needs.Ccs.Services.Models.HangUpOrder> GetIQueryable()
        {
            if (this.User.IsMain)
            {
                return from entity in base.GetIQueryable()
                       where entity.Client.ID == this.User.ClientID
                       select entity;
            }
            else
            {
                return from entity in base.GetIQueryable()
                       where entity.UserID == this.User.ID
                       select entity;
            }
        }
    }

    ///// <summary>
    ///// 已挂起订单1
    ///// </summary>
    //public class UserHangUpOrdersView1 : HangUpOrdersView1
    //{
    //    IPlatUser User;

    //    internal UserHangUpOrdersView1(IPlatUser user)
    //    {
    //        this.User = user;
    //    }

    //    protected override IQueryable<HangUpOrder> GetIQueryable(Expression<Func<HangUpOrder, bool>> expression, params LambdaExpression[] expressions)
    //    {
    //        if (this.User.IsMain)
    //        {
    //            return base.GetIQueryable(expression, expressions).Where(item => item.ClientID == this.User.ClientID).OrderByDescending(item => item.CreateDate);
    //        }
    //        else
    //        {
    //            return base.GetIQueryable(expression, expressions).Where(item => item.UserID == this.User.ID).OrderByDescending(item => item.CreateDate);
    //        }
    //    }

    //    protected override Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
    //    {
    //        return base.GetItems(orderid);
    //    }
    //}

    ///// <summary>
    ///// 已退回订单
    ///// </summary>
    //public class UserReturnedOrdersView : ReturnedOrdersView
    //{
    //    IPlatUser User;

    //    internal UserReturnedOrdersView(IPlatUser user)
    //    {
    //        this.User = user;
    //    }

    //    protected override IQueryable<ReturnedOrder> GetIQueryable()
    //    {
    //        if (this.User.IsMain)
    //        {
    //            return from entity in base.GetIQueryable()
    //                   where entity.Client.ID == this.User.ClientID
    //                   select entity;
    //        }
    //        else
    //        {
    //            return from entity in base.GetIQueryable()
    //                   where entity.UserID == this.User.ID
    //                   select entity;
    //        }
    //    }
    //}

    /// <summary>
    /// 已退回订单
    /// </summary>
    public class UserReturnedOrdersView1 : ReturnedOrdersView1
    {
        IPlatUser User;

        internal UserReturnedOrdersView1(IPlatUser user)
        {
            this.User = user;
        }
        protected override IQueryable<ReturnedOrder> GetIQueryable(Expression<Func<ReturnedOrder, bool>> expression, params LambdaExpression[] expressions)
        {
            if (this.User.IsMain)
            {
                return base.GetIQueryable(expression, expressions).Where(item => item.ClientID == this.User.ClientID).OrderByDescending(item => item.CreateDate);
            }
            else
            {
                return base.GetIQueryable(expression, expressions).Where(item => item.UserID == this.User.ID).OrderByDescending(item => item.CreateDate);
            }
        }

        protected override Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            return base.GetItems(orderid);
        }
    }

    ///// <summary>
    ///// 已取消订单
    ///// </summary>
    //public class UserCanceledOrdersView : CanceledOrdersView
    //{
    //    IPlatUser User;

    //    internal UserCanceledOrdersView(IPlatUser user)
    //    {
    //        this.User = user;
    //    }

    //    protected override IQueryable<CanceledOrder> GetIQueryable()
    //    {
    //        if (this.User.IsMain)
    //        {
    //            return from entity in base.GetIQueryable()
    //                   where entity.Client.ID == this.User.ClientID
    //                   select entity;
    //        }
    //        else
    //        {
    //            return from entity in base.GetIQueryable()
    //                   where entity.UserID == this.User.ID
    //                   select entity;
    //        }
    //    }
    //}

    /// <summary>
    /// 已取消订单1
    /// </summary>
    public class UserCanceledOrdersView1 : CanceledOrdersView1
    {
        IPlatUser User;

        internal UserCanceledOrdersView1(IPlatUser user)
        {
            this.User = user;
        }

        protected override IQueryable<CanceledOrder> GetIQueryable(Expression<Func<CanceledOrder, bool>> expression, params LambdaExpression[] expressions)
        {
            if (this.User.IsMain)
            {
                return base.GetIQueryable(expression, expressions).Where(item => item.ClientID == this.User.ClientID).OrderByDescending(item => item.CreateDate);
            }
            else
            {
                return base.GetIQueryable(expression, expressions).Where(item => item.UserID == this.User.ID).OrderByDescending(item => item.CreateDate);
            }
        }

        protected override Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            return base.GetItems(orderid);
        }
    }

    ///// <summary>
    ///// 待开票订单
    ///// </summary>
    //public class UserUnInvoicedOrdersView : UnInvoicedOrdersView
    //{
    //    IPlatUser User;

    //    internal UserUnInvoicedOrdersView(IPlatUser user)
    //    {
    //        this.User = user;
    //    }

    //    protected override IQueryable<UnInvoicedOrder> GetIQueryable()
    //    {
    //        if (this.User.IsMain)
    //        {
    //            return from entity in base.GetIQueryable()
    //                   where entity.Client.ID == this.User.ClientID
    //                   select entity;
    //        }
    //        else
    //        {
    //            return from entity in base.GetIQueryable()
    //                   where entity.UserID == this.User.ID
    //                   select entity;
    //        }
    //    }
    //}

    /// <summary>
    /// 待开票订单1
    /// </summary>
    public class UserUnInvoicedOrdersView1 : UnInvoicedOrdersView1
    {
        IPlatUser User;

        internal UserUnInvoicedOrdersView1(IPlatUser user)
        {
            this.User = user;
        }

        protected override IQueryable<UnInvoicedOrder> GetIQueryable(Expression<Func<UnInvoicedOrder, bool>> expression, params LambdaExpression[] expressions)
        {
            if (this.User.IsMain)
            {
                return base.GetIQueryable(expression, expressions).Where(item => item.ClientID == this.User.ClientID).OrderByDescending(item => item.CreateDate);
            }
            else
            {
                return base.GetIQueryable(expression, expressions).Where(item => item.UserID == this.User.ID).OrderByDescending(item => item.CreateDate);
            }
        }

        protected override Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            return base.GetItems(orderid);
        }
    }

    ///// <summary>
    ///// 已完成订单
    ///// </summary>
    //public class UserCompletedOrdersView : CompletedOrdersView
    //{
    //    IPlatUser User;

    //    internal UserCompletedOrdersView(IPlatUser user)
    //    {
    //        this.User = user;
    //    }

    //    protected override IQueryable<CompletedOrder> GetIQueryable()
    //    {
    //        if (this.User.IsMain)
    //        {
    //            return from entity in base.GetIQueryable()
    //                   where entity.Client.ID == this.User.ClientID
    //                   select entity;
    //        }
    //        else
    //        {
    //            return from entity in base.GetIQueryable()
    //                   where entity.UserID == this.User.ID
    //                   select entity;
    //        }
    //    }
    //}

    /// <summary>
    /// 已完成订单1
    /// </summary>
    public class UserCompletedOrdersView1 : CompletedOrdersView1
    {
        IPlatUser User;

        internal UserCompletedOrdersView1(IPlatUser user)
        {
            this.User = user;
        }

        protected override IQueryable<CompletedOrder> GetIQueryable(Expression<Func<CompletedOrder, bool>> expression, params LambdaExpression[] expressions)
        {
            if (this.User.IsMain)
            {
                return base.GetIQueryable(expression, expressions).Where(item => item.ClientID == this.User.ClientID).OrderByDescending(item => item.CreateDate);
            }
            else
            {
                return base.GetIQueryable(expression, expressions).Where(item => item.UserID == this.User.ID).OrderByDescending(item => item.CreateDate);
            }
        }

        protected override Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            return base.GetItems(orderid);
        }
    }

    ///// <summary>
    ///// 待归类视图
    ///// </summary>
    //public class UserUnClassfiedOrdersView : UnclassfiedOrdersViewExtends
    //{
    //    IPlatUser User;

    //    internal UserUnClassfiedOrdersView(IPlatUser user)
    //    {
    //        this.User = user;
    //    }

    //    protected override IQueryable<UnClassfiedOrder> GetIQueryable()
    //    {
    //        if (this.User.IsMain)
    //        {
    //            return from entity in base.GetIQueryable()
    //                   where entity.Client.ID == this.User.ClientID
    //                   select entity;
    //        }
    //        else
    //        {
    //            return from entity in base.GetIQueryable()
    //                   where entity.UserID == this.User.ID
    //                   select entity;
    //        }
    //    }
    //}

    /// <summary>
    /// 待归类视图1
    /// </summary>
    public class UserUnClassfiedOrdersView1 : UnclassfiedOrdersViewExtends1
    {
        IPlatUser User;

        internal UserUnClassfiedOrdersView1(IPlatUser user)
        {
            this.User = user;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.UnClassfiedOrder> GetIQueryable(Expression<Func<Needs.Ccs.Services.Models.UnClassfiedOrder, bool>> expression, params LambdaExpression[] expressions)
        {
            if (this.User.IsMain)
            {
                return base.GetIQueryable(expression, expressions).Where(item => item.ClientID == this.User.ClientID).OrderByDescending(item => item.CreateDate);
            }
            return base.GetIQueryable(expression, expressions).Where(item => item.UserID == this.User.ID).OrderByDescending(item => item.CreateDate);
        }

        protected override Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            return base.GetItems(orderid);
        }
    }

    /// <summary>
    /// 首页订单
    /// </summary>
    public class IndexView : UniqueView<Order, ScCustomsReponsitory>
    {
        IPlatUser User;

        internal IndexView(IPlatUser user)
        {
            this.User = user;
        }

        protected override IQueryable<Order> GetIQueryable()
        {
            //TODO:根据条件 this.User.IsMain 完成query的拼接
            //不再使用getData 的方式查询，因为：ClientsView中查询的表太多，首页的订单列表中的订单，不需要太多的信息。

            if (this.User.IsMain)
            {
                var linq = from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                           where order.ClientID == this.User.ClientID && order.Status == (int)Status.Normal && order.OrderStatus != (int)OrderStatus.Draft
                           select new Order
                           {
                               ID = order.ID,
                               Status = (Status)order.Status,
                               CreateDate = order.CreateDate,
                               Summary = (from trace in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderTraces>()
                                          where trace.OrderID == order.ID
                                          orderby trace.CreateDate descending
                                          select trace.Summary).FirstOrDefault(),
                               OrderStatus = (OrderStatus)order.OrderStatus,
                               IsHangUp = order.IsHangUp,
                               InvoiceStatus = (InvoiceStatus)order.InvoiceStatus,
                               UserID = order.UserID,
                           };
                return linq;
            }
            else
            {
                return from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                       where order.UserID == this.User.ID && order.Status == (int)Status.Normal && order.OrderStatus != (int)OrderStatus.Draft
                       select new Order
                       {
                           ID = order.ID,
                           Status = (Status)order.Status,
                           CreateDate = order.CreateDate,
                           Summary = (from log in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderLogs>()
                                      where log.OrderID == order.ID
                                      orderby log.CreateDate descending
                                      select log.Summary).FirstOrDefault(),
                           OrderStatus = (OrderStatus)order.OrderStatus,
                           IsHangUp = order.IsHangUp,
                           InvoiceStatus = (InvoiceStatus)order.InvoiceStatus,
                           UserID = order.UserID,
                       };
            }
        }

        /// <summary>
        /// 获取待确认订单数量
        /// </summary>
        /// <returns></returns>
        public int GetUnConfirmCount()
        {
            return this.GetIQueryable().Count(item => item.OrderStatus == OrderStatus.Quoted && !item.IsHangUp);
        }

        /// <summary>
        /// 获取待开票订单数量
        /// </summary>
        /// <returns></returns>
        public int GetUnInvoiceCount()
        {
            return this.GetIQueryable().Count(item => item.InvoiceStatus == InvoiceStatus.UnInvoiced &&
                         item.OrderStatus >= OrderStatus.Declared && item.OrderStatus <= OrderStatus.Completed);
        }

        /// <summary>
        /// 获取待付汇订单数量
        /// </summary>
        /// <returns></returns>
        public int GetUnPayExchangeCount()
        {
            return new UserUnPayExchangeOrdersView(this.User, this.Reponsitory).Count();
        }

        /// <summary>
        /// 获取待收货订单数量
        /// </summary>
        /// <returns></returns>
        public int GetWarehouseExitedCount()
        {
            return this.GetIQueryable().Count(item => item.OrderStatus == OrderStatus.WarehouseExited);
        }

        /// <summary>
        /// 获取已挂起订单数量
        /// </summary>
        /// <returns></returns>
        public int GetHangUpCount()
        {
            return this.GetIQueryable().Count(item => item.IsHangUp);
        }
    }

    /// <summary>
    /// 缴费流水视图
    /// </summary>
    public class UserTaxRecordsView : TaxRecordsView
    {
        IPlatUser User;

        internal UserTaxRecordsView(IPlatUser user)
        {
            this.User = user;
        }

        protected override IQueryable<DecTaxFlowForUser> GetIQueryable(Expression<Func<DecTaxFlowForUser, bool>> expression, params LambdaExpression[] expressions)
        {
            if (this.User.IsMain)
            {
                return from dectaxflow in base.GetIQueryable(expression, expressions)
                       join dectax in Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxs>() on dectaxflow.DecTaxID equals dectax.ID
                       join dechead in Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>() on dectaxflow.DecTaxID equals dechead.ID
                       join order in Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on dechead.OrderID equals order.ID
                       where order.ClientID == this.User.ClientID && dectax.InvoiceType == (int)InvoiceType.Service
                       orderby dectaxflow.PayDate descending
                       select dectaxflow;
            }
            return from dectaxflow in base.GetIQueryable(expression, expressions)
                   join dectax in Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxs>() on dectaxflow.DecTaxID equals dectax.ID
                   join dechead in Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>() on dectaxflow.DecTaxID equals dechead.ID
                   join order in Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on dechead.OrderID equals order.ID
                   where order.UserID == this.User.ID && dectax.InvoiceType == (int)InvoiceType.Service
                   orderby dectaxflow.PayDate descending
                   select dectaxflow;
        }
    }
}
