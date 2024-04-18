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
    /// 已报价，待客户确认订单的视图扩展
    /// </summary>
    public class QuotedOrdersViewExtends : OrdersViewBase<Models.QuotedOrder>
    {
        protected override IQueryable<Models.QuotedOrder> GetIQueryable()
        {
            var usersView = new UsersView(this.Reponsitory);
            var orderConsignorsView = new OrderConsignorsView(this.Reponsitory);

            return from order in base.GetIQueryable()
                   join orderConsignor in orderConsignorsView on order.ID equals orderConsignor.OrderID
                   join user in usersView on order.UserID equals user.ID into users
                   from user in users.DefaultIfEmpty()
                   where order.OrderStatus == Enums.OrderStatus.Quoted && !order.IsHangUp
                   select new Models.QuotedOrder
                   {
                       ID = order.ID,
                       Type = order.Type,
                       AdminID = order.AdminID,
                       UserID = order.UserID,
                       OrderMaker = (user == null ? "跟单员" : user.RealName),
                       Client = order.Client,
                       ClientAgreement = order.ClientAgreement,
                       OrderConsignee = order.OrderConsignee,
                       OrderConsignor = orderConsignor,
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
                       CreateDate = order.CreateDate,
                       Status=order.Status,
                       OrderStatus=order.OrderStatus,
                       Summary=order.Summary
                   };
        }
    }

    /// <summary>
    /// 已报价，待客户确认订单的视图扩展1
    /// </summary>
    public class QuotedOrdersViewExtends1 : Orders1ViewBase<Models.QuotedOrder>
    {
        public QuotedOrdersViewExtends1() : base()
        {
        }

        internal QuotedOrdersViewExtends1(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.QuotedOrder> GetIQueryable(Expression<Func<Models.QuotedOrder, bool>> expression, params LambdaExpression[] expressions)
        {
            return base.GetIQueryable(expression, expressions).Where(item => item.OrderStatus == Enums.OrderStatus.Quoted && !item.IsHangUp);
        }

        virtual protected Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            var items = new OrderItemsView(this.Reponsitory);
            return items.Where(item => item.OrderID == orderid).ToArray();
        }
    }

    /// <summary>
    /// 已报价，待客户确认订单的视图扩展2
    /// </summary>
    public class QuotedOrdersViewExtends2 : Orders1ViewBase<Models.QuotedOrder>
    {
        public QuotedOrdersViewExtends2() : base()
        {
        }

        internal QuotedOrdersViewExtends2(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.QuotedOrder> GetIQueryable(Expression<Func<Models.QuotedOrder, bool>> expression, params LambdaExpression[] expressions)
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
    /// 待归类订单
    /// </summary>
    public class UnclassfiedOrdersViewExtends : OrdersViewBase<Models.UnClassfiedOrder>
    {
        protected override IQueryable<Models.UnClassfiedOrder> GetIQueryable()
        {
            var usersView = new UsersView(this.Reponsitory);
            var orderConsignorsView = new OrderConsignorsView(this.Reponsitory);

            return from order in base.GetIQueryable()
                   join orderConsignor in orderConsignorsView on order.ID equals orderConsignor.OrderID
                   join user in usersView on order.UserID equals user.ID into users
                   from user in users.DefaultIfEmpty()
                   where order.OrderStatus == Enums.OrderStatus.Confirmed
                   select new Models.UnClassfiedOrder
                   {
                       ID = order.ID,
                       Type = order.Type,
                       AdminID = order.AdminID,
                       UserID = order.UserID,
                       OrderMaker = (user == null ? "跟单员" : user.RealName),
                       Client = order.Client,
                       ClientAgreement = order.ClientAgreement,
                       OrderConsignee = order.OrderConsignee,
                       OrderConsignor = orderConsignor,
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
                       CreateDate = order.CreateDate,
                       Status = order.Status,
                       OrderStatus = order.OrderStatus,
                   };
        }
    }

    /// <summary>
    /// 待归类订单1
    /// </summary>
    public class UnclassfiedOrdersViewExtends1 : Orders1ViewBase<Models.UnClassfiedOrder>
    {
        public UnclassfiedOrdersViewExtends1() : base()
        {
        }

        internal UnclassfiedOrdersViewExtends1(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.UnClassfiedOrder> GetIQueryable(Expression<Func<Models.UnClassfiedOrder, bool>> expression, params LambdaExpression[] expressions)
        {
            return base.GetIQueryable(expression, expressions).Where(item => item.OrderStatus == Enums.OrderStatus.Confirmed);
        }

        virtual protected Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            var items = new OrderItemsView(this.Reponsitory);
            return items.Where(item => item.OrderID == orderid).ToArray();
        }
    }
}
