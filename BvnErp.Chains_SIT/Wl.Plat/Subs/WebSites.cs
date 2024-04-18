using Needs.Wl.User.Plat.Views;

namespace Needs.Wl.User.Plat.Models
{
    /// <summary>
    /// Wl.net.cn 站点
    /// TODO:禁止在此添加代码了
    /// </summary>
    public class WebSites
    {
        IPlatUser User;

        public WebSites(IPlatUser user)
        {
            this.User = user;
        }

        /// <summary>
        /// 我的草稿订单1
        /// </summary>
        public UserDraftOrdersView1 MyDraftOrdersView1
        {
            get
            {
                return new UserDraftOrdersView1(this.User);
            }
        }

        /// <summary>
        /// 已报价，待客户确认订单的视图1
        /// </summary>
        public UserQuotedOrdersView1 MyQuotedOrdersView1
        {
            get
            {
                return new UserQuotedOrdersView1(this.User);
            }
        }

        /// <summary>
        /// 已报价，待客户确认订单的视图2
        /// </summary>
        public UserQuotedOrdersView2 MyQuotedOrdersView2
        {
            get
            {
                return new UserQuotedOrdersView2(this.User);
            }
        }

        /// <summary>
        /// 已报价，待客户确认订单的视图 UnConfirmedOrdersView
        /// </summary>
        public UnConfirmedOrdersView UnConfirmedOrdersView
        {
            get
            {
                return new UnConfirmedOrdersView(this.User);
            }
        }

        ///// <summary>
        ///// 待付汇订单的视图2
        ///// </summary>
        //public UserUnPayExchangeOrdersView1 MyUnPayExchangeOrdersView1
        //{
        //    get
        //    {
        //        return new UserUnPayExchangeOrdersView1(this.User);
        //    }
        //}

        /// <summary>
        /// 挂起订单
        /// </summary>
        public UserHangUpOrdersView MyHangUpOrdersView
        {
            get
            {
                return new UserHangUpOrdersView(this.User);
            }
        }

        /// <summary>
        /// 退回订单1
        /// </summary>
        public UserReturnedOrdersView1 MyReturnedOrdersView1
        {
            get
            {
                return new UserReturnedOrdersView1(this.User);
            }
        }

        ///// <summary>
        ///// 已取消订单1
        ///// </summary>
        //public UserCanceledOrdersView1 MyCanceledOrdersView1
        //{
        //    get
        //    {
        //        return new UserCanceledOrdersView1(this.User);
        //    }
        //}

        ///// <summary>
        ///// 待开票订单1
        ///// </summary>
        //public UserUnInvoicedOrdersView1 MyUnInvoicedOrdersView1
        //{
        //    get
        //    {
        //        return new UserUnInvoicedOrdersView1(this.User);
        //    }
        //}

        ///// <summary>
        ///// 已完成订单1
        ///// </summary>
        //public UserCompletedOrdersView1 MyCompletedOrdersView1
        //{
        //    get
        //    {
        //        return new UserCompletedOrdersView1(this.User);
        //    }
        //}

        /// <summary>
        /// 待归类订单1
        /// </summary>
        public UserUnClassfiedOrdersView1 MyUnClassfiedOrdersView1
        {
            get
            {
                return new UserUnClassfiedOrdersView1(this.User);
            }
        }

        /// <summary>
        /// 待收货订单1
        /// </summary>
        public UserWarehouseExitedOrdersView1 MyWarehouseExitedOrdersView1
        {
            get
            {
                return new UserWarehouseExitedOrdersView1(this.User);
            }
        }

        /// <summary>
        /// 我的订单（全部）
        /// </summary>
        public UserOrdersView MyOrders
        {
            get
            {
                return new UserOrdersView(this.User);
            }
        }

        /// <summary>
        /// 我的订单（全部）
        /// </summary>
        public UserOrdersView1 MyOrders1
        {
            get
            {
                return new UserOrdersView1(this.User);
            }
        }

        /// <summary>
        /// 我的订单（扩展）
        /// </summary>
        public Views.OrderExtendsView1 MyOrdersExtends1
        {
            get
            {
                return new OrderExtendsView1(this.User);
            }
        }

        /// <summary>
        /// 我的订单（首页）
        /// </summary>
        public IndexView MyIndex
        {
            get
            {
                return new IndexView(this.User);
            }
        }

        /// <summary>
        /// 客户的付汇申请
        /// </summary>
        public Views.ClientPayExchangeAppliesView MyPayExchangeApplies
        {
            get
            {
                return new Views.ClientPayExchangeAppliesView(this.User);
            }
        }

        /// <summary>
        /// 客户的对账单
        /// </summary>
        public UserOrderBillsView MyOrderBills
        {
            get
            {
                return new UserOrderBillsView(this.User);
            }
        }

        ///// <summary>
        ///// 付款记录
        ///// </summary>
        //public ClientReceiptNoticesView MyReceiptNotices
        //{
        //    get
        //    {
        //        return new ClientReceiptNoticesView(this.User);
        //    }
        //}

        ///// <summary>
        ///// 付款明细
        ///// </summary>
        //public ClientOrderReceivedsView MyOrderReceiveds
        //{
        //    get
        //    {
        //        return new ClientOrderReceivedsView();
        //    }
        //}

        /// <summary>
        /// 缴费流水
        /// </summary>
        public UserTaxRecordsView MyTaxRecordsView
        {
            get
            {
                return new UserTaxRecordsView(this.User);
            }
        }

        ///// <summary>
        ///// 报关单
        ///// </summary>
        //public ClientDeclareOrderView MyClientDeclareOrderView
        //{
        //    get
        //    {
        //        return new ClientDeclareOrderView(this.User);
        //    }
        //}

        ///// <summary>
        ///// 报关单数据
        ///// </summary>
        //public ClientOrderDataView MyClientOrderDataView
        //{
        //    get
        //    {
        //        return new ClientOrderDataView(this.User);
        //    }
        //}

        ///// <summary>
        ///// 报关单excel页面导出
        ///// </summary>
        //public ClientOrderDataExportView MyClientOrderDataExportView
        //{
        //    get
        //    {
        //        return new ClientOrderDataExportView(this.User);
        //    }
        //}
    }
}