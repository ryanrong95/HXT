using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Erp.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Admin.Plat.Views
{
    /// <summary>
    /// 代理订单的视图（Admin过滤）
    /// </summary>
    public class MyOrdersView : OrdersView
    {
        IGenericAdmin Admin;

        public MyOrdersView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.Order> GetIQueryable()
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable();
            }

            return from order in base.GetIQueryable()
                   where order.Client.Merchandiser.ID == this.Admin.ID || order.Client.ServiceManager.ID == this.Admin.ID
                   select order;
        }
    }

    /// <summary>
    /// 代理订单的视图（Admin过滤）
    /// </summary>
    public class MyOrders1View : Orders1View
    {
        IGenericAdmin Admin;

        public MyOrders1View(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.Order> GetIQueryable(Expression<Func<Needs.Ccs.Services.Models.Order, bool>> expression, params LambdaExpression[] expressions)
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable(expression, expressions).OrderByDescending(item => item.CreateDate);
            }

            var clientIds = new Views.MyClientsView(this.Admin).Select(c => c.ID).ToArray();
            return base.GetIQueryable(expression, expressions).Where(item => clientIds.Contains(item.ClientID)).OrderByDescending(item => item.CreateDate);
        }

        protected override Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            return base.GetItems(orderid);
        }
    }

    /// <summary>
    /// 代理订单的视图（风控，可以看到所有订单）
    /// </summary>
    public class RiskOrderView : Orders1View
    {
        IGenericAdmin Admin;

        public RiskOrderView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.Order> GetIQueryable(Expression<Func<Needs.Ccs.Services.Models.Order, bool>> expression, params LambdaExpression[] expressions)
        {
            return base.GetIQueryable(expression, expressions).OrderByDescending(item => item.CreateDate);
        }

        protected override Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            return base.GetItems(orderid);
        }
    }

    /// <summary>
    /// 草稿订单的视图（Admin过滤）
    /// </summary>
    public class MyDraftOrdersView : DraftOrdersView
    {
        IGenericAdmin Admin;

        public MyDraftOrdersView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.DraftOrder> GetIQueryable()
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable();
            }

            return from order in base.GetIQueryable()
                   where order.Client.Merchandiser.ID == this.Admin.ID
                   select order;
        }
    }

    /// <summary>
    /// 草稿订单的视图（Admin过滤）
    /// </summary>
    public class MyDraftOrdersView1 : DraftOrdersView1
    {
        IGenericAdmin Admin;

        public MyDraftOrdersView1(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.DraftOrder> GetIQueryable(Expression<Func<Needs.Ccs.Services.Models.DraftOrder, bool>> expression, params LambdaExpression[] expressions)
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable(expression, expressions).OrderByDescending(item => item.CreateDate);
            }

            var clientIds = new Views.MyClientsView(this.Admin).Select(c => c.ID).ToArray();
            return base.GetIQueryable(expression, expressions).Where(item => clientIds.Contains(item.ClientID)).OrderByDescending(item => item.CreateDate);
        }

        protected override Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            return base.GetItems(orderid);
        }
    }

    /// <summary>
    /// 待归类
    /// </summary>
    public class MyUnClassifyOrdersView1 : UnClassifyOrdersView1
    {
        IGenericAdmin Admin;

        public MyUnClassifyOrdersView1(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.UnClassifyOrder> GetIQueryable(Expression<Func<Needs.Ccs.Services.Models.UnClassifyOrder, bool>> expression, params LambdaExpression[] expressions)
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable(expression, expressions).OrderByDescending(c => c.CreateDate);
            }

            var clientIds = new Views.MyClientsView(this.Admin).Select(c => c.ID).ToArray();
            return base.GetIQueryable(expression, expressions).Where(item => clientIds.Contains(item.ClientID)).OrderByDescending(c => c.CreateDate);
        }

        protected override Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            return base.GetItems(orderid);
        }
    }

    /// <summary>
    /// 已归类，待报价订单的视图（Admin过滤）
    /// </summary>
    public class MyClassifiedOrdersView : ClassifiedOrdersView
    {
        IGenericAdmin Admin;

        public MyClassifiedOrdersView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.ClassifiedOrder> GetIQueryable()
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable();
            }

            return from order in base.GetIQueryable()
                   where order.Client.Merchandiser.ID == this.Admin.ID
                   select order;
        }
    }

    /// <summary>
    /// 已归类，待报价订单的视图（Admin过滤）
    /// </summary>
    public class MyClassifiedOrdersView1 : ClassifiedOrdersView1
    {
        IGenericAdmin Admin;

        public MyClassifiedOrdersView1(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.Order> GetIQueryable(Expression<Func<Needs.Ccs.Services.Models.Order, bool>> expression, params LambdaExpression[] expressions)
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable(expression, expressions).OrderByDescending(item => item.CreateDate);
            }

            var clientIds = new Views.MyClientsView(this.Admin).Select(c => c.ID).ToArray();
            return base.GetIQueryable(expression, expressions).Where(item => clientIds.Contains(item.ClientID)).OrderByDescending(item => item.CreateDate);
        }

        protected override Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            return base.GetItems(orderid);
        }
    }

    /// <summary>
    /// 未匹配到货信息的订单（Admin过滤）
    /// </summary>
    public class MyUnMatchedOrdersView : UnMatchedOrdersView
    {
        IGenericAdmin Admin;

        public MyUnMatchedOrdersView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.Order> GetIQueryable(Expression<Func<Needs.Ccs.Services.Models.Order, bool>> expression, params LambdaExpression[] expressions)
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable(expression, expressions).OrderByDescending(item => item.CreateDate);
            }

            var clientIds = new Views.MyClientsView(this.Admin).Select(c => c.ID).ToArray();
            return base.GetIQueryable(expression, expressions).Where(item => clientIds.Contains(item.ClientID)).OrderByDescending(item => item.CreateDate);
        }

        protected override Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            return base.GetItems(orderid);
        }
    }

    /// <summary>
    /// 报价完成，待客户确认订单的视图（Admin过滤）
    /// </summary>
    public class MyQuotedOrdersView : QuotedOrdersView
    {
        IGenericAdmin Admin;

        public MyQuotedOrdersView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.QuotedOrder> GetIQueryable()
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable();
            }

            return from order in base.GetIQueryable()
                   where order.Client.Merchandiser.ID == this.Admin.ID
                   select order;
        }
    }

    /// <summary>
    /// 报价完成，待客户确认订单的视图（Admin过滤）
    /// </summary>
    public class MyQuotedOrdersView1 : QuotedOrdersView1
    {
        IGenericAdmin Admin;

        public MyQuotedOrdersView1(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.Order> GetIQueryable(Expression<Func<Needs.Ccs.Services.Models.Order, bool>> expression, params LambdaExpression[] expressions)
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable(expression, expressions).OrderByDescending(item => item.CreateDate);
            }

            var clientIds = new Views.MyClientsView(this.Admin).Select(c => c.ID).ToArray();
            return base.GetIQueryable(expression, expressions).Where(item => clientIds.Contains(item.ClientID)).OrderByDescending(item => item.CreateDate);
        }

        protected override Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            return base.GetItems(orderid);
        }
    }

    /// <summary>
    /// 已报关，待出库订单的视图（Admin过滤）
    /// </summary>
    public class MyDeclaredOrdersView : DeclaredOrdersView
    {
        IGenericAdmin Admin;

        public MyDeclaredOrdersView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.DeclaredOrder> GetIQueryable()
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable();
            }

            return from order in base.GetIQueryable()
                   where order.Client.Merchandiser.ID == this.Admin.ID
                   select order;
        }
    }

    /// <summary>
    /// 已报关，待出库订单的视图（Admin过滤）
    /// </summary>
    public class MyDeclaredOrdersView1 : DeclaredOrdersView1
    {
        IGenericAdmin Admin;

        public MyDeclaredOrdersView1(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.DeclaredOrder> GetIQueryable(Expression<Func<Needs.Ccs.Services.Models.DeclaredOrder, bool>> expression, params LambdaExpression[] expressions)
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable(expression, expressions).OrderByDescending(item => item.CreateDate);
            }

            var clientIds = new Views.MyClientsView(this.Admin).Select(c => c.ID).ToArray();
            return base.GetIQueryable(expression, expressions).Where(item => clientIds.Contains(item.ClientID)).OrderByDescending(item => item.CreateDate);
        }

        protected override Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            return base.GetItems(orderid);
        }
    }

    /// <summary>
    /// 管理端 待发货页面 过滤跟单员用
    /// </summary>
    public class MyMainOrderPendingDeliveryView : CenterOrderPendingDeliveryView
    {
        IGenericAdmin Admin;

        public MyMainOrderPendingDeliveryView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<OrderPendingDelieveryViewModel> GetIQueryable()
        {
            //return base.GetIQueryable();
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable().Where(item => item.AdminID == this.Admin.ID);
            }

            var clientIds = new Views.MyClientsView(this.Admin).Select(c => c.ID).ToArray();
            return base.GetIQueryable().Where(item => clientIds.Contains(item.ClientID));
        }
    }

    /// <summary>
    /// 已退回订单的视图（Admin过滤）
    /// </summary>
    public class MyReturnedOrdersView : ReturnedOrdersView
    {
        IGenericAdmin Admin;

        public MyReturnedOrdersView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.ReturnedOrder> GetIQueryable()
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable();
            }

            return from order in base.GetIQueryable()
                   where order.Client.Merchandiser.ID == this.Admin.ID
                   select order;
        }
    }

    /// <summary>
    /// 已退回订单的视图（Admin过滤）
    /// </summary>
    public class MyReturnedOrdersView1 : ReturnedOrdersView1
    {
        IGenericAdmin Admin;

        public MyReturnedOrdersView1(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.ReturnedOrder> GetIQueryable(Expression<Func<Needs.Ccs.Services.Models.ReturnedOrder, bool>> expression, params LambdaExpression[] expressions)
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable(expression, expressions).OrderByDescending(c => c.CreateDate);
            }

            var clientIds = new Views.MyClientsView(this.Admin).Select(c => c.ID).ToArray();
            return base.GetIQueryable(expression, expressions).Where(item => clientIds.Contains(item.ClientID)).OrderByDescending(c => c.CreateDate);
        }

        protected override Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            return base.GetItems(orderid);
        }
    }

    /// <summary>
    /// 已取消订单的视图（Admin过滤）
    /// </summary>
    public class MyCanceledOrdersView : CanceledOrdersView
    {
        IGenericAdmin Admin;

        public MyCanceledOrdersView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.CanceledOrder> GetIQueryable()
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable();
            }

            return from order in base.GetIQueryable()
                   where order.Client.Merchandiser.ID == this.Admin.ID
                   select order;
        }
    }

    /// <summary>
    /// 已取消订单的视图（Admin过滤）
    /// </summary>
    public class MyCanceledOrdersView1 : CanceledOrdersView1
    {
        IGenericAdmin Admin;

        public MyCanceledOrdersView1(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.CanceledOrder> GetIQueryable(Expression<Func<Needs.Ccs.Services.Models.CanceledOrder, bool>> expression, params LambdaExpression[] expressions)
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable(expression, expressions).OrderByDescending(c => c.CreateDate);
            }

            var clientIds = new Views.MyClientsView(this.Admin).Select(c => c.ID).ToArray();
            return base.GetIQueryable(expression, expressions).Where(item => clientIds.Contains(item.ClientID)).OrderByDescending(c => c.CreateDate);
        }

        protected override Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            return base.GetItems(orderid);
        }
    }

    /// <summary>
    /// 已挂起订单的视图（Admin过滤）
    /// </summary>
    public class MyHangUpOrdersView : HangUpOrdersView
    {
        IGenericAdmin Admin;

        public MyHangUpOrdersView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.HangUpOrder> GetIQueryable()
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable();
            }

            return from order in base.GetIQueryable()
                   where order.Client.Merchandiser.ID == this.Admin.ID
                   select order;
        }
    }

    /// <summary>
    /// 已取消订单的视图（Admin过滤）
    /// </summary>
    public class MyHangUpOrdersView1 : HangUpOrdersView1
    {
        IGenericAdmin Admin;

        public MyHangUpOrdersView1(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.HangUpOrder> GetIQueryable(Expression<Func<Needs.Ccs.Services.Models.HangUpOrder, bool>> expression, params LambdaExpression[] expressions)
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable(expression, expressions).OrderByDescending(c => c.CreateDate);
            }

            var clientIds = new Views.MyClientsView(this.Admin).Select(c => c.ID).ToArray();
            return base.GetIQueryable(expression, expressions).Where(item => clientIds.Contains(item.ClientID)).OrderByDescending(c => c.CreateDate);
        }

        protected override Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            return base.GetItems(orderid);
        }
    }

    /// <summary>
    /// 待付汇订单的视图（Admin过滤）
    /// </summary>
    public class MyUnPayExchangeOrdersView : UnPayExchangeOrdersView
    {
        IGenericAdmin Admin;

        public MyUnPayExchangeOrdersView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.UnPayExchangeOrder> GetIQueryable()
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable();
            }

            return from order in base.GetIQueryable()
                   where order.Client.Merchandiser.ID == this.Admin.ID
                   select order;
        }
    }

    /// <summary>
    /// 待付汇订单的视图（Admin过滤）
    /// </summary>
    public class MyUnPayExchangeOrdersView1 : UnPayExchangeOrdersView1
    {
        IGenericAdmin Admin;

        public MyUnPayExchangeOrdersView1(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.UnPayExchangeOrder> GetIQueryable(Expression<Func<Needs.Ccs.Services.Models.UnPayExchangeOrder, bool>> expression, params LambdaExpression[] expressions)
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable(expression, expressions).OrderByDescending(c => c.CreateDate);
            }

            var clientIds = new Views.MyClientsView(this.Admin).Select(c => c.ID).ToArray();
            return base.GetIQueryable(expression, expressions).Where(item => clientIds.Contains(item.ClientID)).OrderByDescending(c => c.CreateDate);
        }

        protected override Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            return base.GetItems(orderid);
        }
    }



    /// <summary>
    /// 待开票订单的视图（Admin过滤）
    /// </summary>
    public class MyUnInvoicedOrdersView : UnInvoicedOrdersView
    {
        IGenericAdmin Admin;

        public MyUnInvoicedOrdersView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.UnInvoicedOrder> GetIQueryable()
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable();
            }

            return from order in base.GetIQueryable()
                   where order.Client.Merchandiser.ID == this.Admin.ID
                   select order;
        }
    }

    /// <summary>
    /// 待开票订单的视图（Admin过滤）
    /// </summary>
    public class MyUnInvoicedOrdersView1 : UnInvoicedOrdersView1
    {
        IGenericAdmin Admin;

        public MyUnInvoicedOrdersView1(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.UnInvoicedOrder> GetIQueryable(Expression<Func<Needs.Ccs.Services.Models.UnInvoicedOrder, bool>> expression, params LambdaExpression[] expressions)
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable(expression, expressions).OrderByDescending(c => c.CreateDate);
            }

            var clientIds = new Views.MyClientsView(this.Admin).Select(c => c.ID).ToArray();
            return base.GetIQueryable(expression, expressions).Where(item => clientIds.Contains(item.ClientID)).OrderByDescending(c => c.CreateDate);
        }

        protected override Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            return base.GetItems(orderid);
        }
    }

    /// <summary>
    /// 可维护费用订单的视图（Admin过滤）
    /// </summary>
    public class MyFeeMaintenanceOrdersView : FeeMaintenanceOrdersView
    {
        IGenericAdmin Admin;

        public MyFeeMaintenanceOrdersView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.FeeMaintenanceOrder> GetIQueryable()
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable();
            }

            return from order in base.GetIQueryable()
                   where order.Client.Merchandiser.ID == this.Admin.ID
                   select order;
        }
    }

    public class MyOrderChangeView : OrderChangeView
    {
        IGenericAdmin Admin;

        public MyOrderChangeView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.OrderChangeNotice> GetIQueryable(Expression<Func<Needs.Ccs.Services.Models.OrderChangeNotice, bool>> expression, params LambdaExpression[] expressions)
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable(expression, expressions).OrderByDescending(c => c.CreateDate);
            }
            // var clientIds = new Views.MyClientsView(this.Admin).Select(c => c.ID).ToArray();
            var orderIds = new Views.MyOrdersView(this.Admin).Select(c => c.ID).ToArray();


            var linq = base.GetIQueryable(expression, expressions);

            string[] orderIDInLinq = linq.Select(t => t.OrderID).ToArray();
            orderIds = orderIds.Distinct().ToArray();

            string[] ids2 = (from a in orderIDInLinq
                             join b in orderIds on a equals b
                             select a).ToArray();


            return linq.Where(item => ids2.Contains(item.OrderID)).OrderByDescending(c => c.CreateDate);
        }
    }

    /// <summary>
    /// 待付汇订单的视图（Admin过滤）
    /// </summary>
    public class MyUnCollectedOrdersView : UnCollectedOrdersView
    {
        IGenericAdmin Admin;

        public MyUnCollectedOrdersView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.UnCollectedOrder> GetIQueryable(Expression<Func<Needs.Ccs.Services.Models.UnCollectedOrder, bool>> expression, params LambdaExpression[] expressions)
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable(expression, expressions).OrderByDescending(c => c.CreateDate);
            }

            var clientIds = new Views.MyClientsView(this.Admin).Select(c => c.ID).ToArray();
            return base.GetIQueryable(expression, expressions).Where(item => clientIds.Contains(item.ClientID)).OrderByDescending(c => c.CreateDate);
        }

        protected override Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            return base.GetItems(orderid);
        }
    }

}
