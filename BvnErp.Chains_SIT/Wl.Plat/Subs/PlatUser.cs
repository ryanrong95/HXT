using Needs.Wl.Client.Services.Views;

namespace Needs.Wl.User.Plat.Models
{
    public partial class PlatUser
    {
        /// <summary>
        /// 即将作废，参考说明文档
        /// </summary>
        public WebSites WebSite
        {
            get { return new WebSites(this); }
        }

        /// <summary>
        /// 订单路径，不进行订单的查询，用于订单子项的调用。
        /// </summary>
        public OrderContext OrderContext
        {
            get { return new OrderContext(this); }
        }

        /// <summary>
        /// 付款记录路径，不进行付款记录的查询，用于子项的调用。
        /// </summary>
        public PaymentRecordContext PaymentRecordContext
        {
            get { return new PaymentRecordContext(this); }
        }

        /// <summary>
        /// 我的供应商
        /// </summary>
        public ClientSuppliersView MySuppliers
        {
            get { return new ClientSuppliersView(this.Client.ID); }
        }

        /// <summary>
        /// 我的收件地址
        /// </summary>
        public ClientConsigneesView MyConsignees
        {
            get { return new ClientConsigneesView(this.Client.ID); }
        }

        /// <summary>
        /// 我的产品
        /// </summary>
        public ClientProductsView MyProducts
        {
            get { return new ClientProductsView(this.Client.ID); }
        }

        /// <summary>
        /// 我的预归类产品
        /// </summary>
        public ClientPreProductsView MyPreProducts
        {
            get { return new ClientPreProductsView(this.Client.ID); }
        }

        /// <summary>
        /// 我的预归类产品
        /// </summary>
        public ClientClassifiedPreProductsView MyClassifiedPreProducts
        {
            get { return new ClientClassifiedPreProductsView(this.Client.ID); }
        }

        /// <summary>
        /// 我的自定义产品税务归类
        /// </summary>
        public ClientProductTaxCategoriesView MyProductTaxCategories
        {
            get { return new ClientProductTaxCategoriesView(this.Client.ID); }
        }

        /// <summary>
        /// 我的付汇申请（订单）
        /// </summary>
        public Needs.Wl.User.Plat.Views.MyPayExchangeAppliesView MyPayExchangeApplies
        {
            get { return new Needs.Wl.User.Plat.Views.MyPayExchangeAppliesView(this); }
        }

        /// <summary>
        /// 我的订单
        /// </summary>
        public Needs.Wl.User.Plat.Views.MyOrdersView MyOrders
        {
            get { return new Needs.Wl.User.Plat.Views.MyOrdersView(this); }
        }


        public Needs.Wl.User.Plat.Views.MyMainOrdersView MyMianOrders
        {
            get { return new Needs.Wl.User.Plat.Views.MyMainOrdersView(this); }
        }

        /// <summary>
        /// 我的全部订单
        /// </summary>
        public Needs.Wl.User.Plat.Views.MyAllOrdersView MyAllOrders
        {
            get { return new Needs.Wl.User.Plat.Views.MyAllOrdersView(this); }
        }


        /// <summary>
        /// 我的待付汇订单
        /// </summary>
        public Needs.Wl.User.Plat.Views.MyUnPayExchangeOrdersView MyUnPayExchangeOrders
        {
            get { return new Needs.Wl.User.Plat.Views.MyUnPayExchangeOrdersView(this); }
        }

        /// <summary>
        /// 我的未开票的订单
        /// </summary>
        public Needs.Wl.User.Plat.Views.MyUnInvocieOrdersView MyUnInvocieOrders
        {
            get { return new Needs.Wl.User.Plat.Views.MyUnInvocieOrdersView(this); }
        }

        /// <summary>
        /// 我的退回的订单
        /// </summary>
        public Needs.Wl.User.Plat.Views.MyRejectedOrdersView MyRejectedOrders
        {
            get { return new Needs.Wl.User.Plat.Views.MyRejectedOrdersView(this); }
        }

        /// <summary>
        /// 我的待收货的订单
        /// </summary>
        public Needs.Wl.User.Plat.Views.MyUnReceivedOrdersView MyUnReceivedOrders
        {
            get { return new Needs.Wl.User.Plat.Views.MyUnReceivedOrdersView(this); }
        }

        /// <summary>
        /// 我的草稿订单
        /// </summary>
        public Needs.Wl.User.Plat.Views.MyDraftOrdersView MyDraftOrders
        {
            get { return new Needs.Wl.User.Plat.Views.MyDraftOrdersView(this); }
        }

        /// <summary>
        /// 我的报关单
        /// </summary>
        public Needs.Wl.User.Plat.Views.MyDecHeadsView MyDecHeads
        {
            get { return new Needs.Wl.User.Plat.Views.MyDecHeadsView(this); }
        }

        /// <summary>
        /// 我的付款记录
        /// </summary>
        public Needs.Wl.User.Plat.Views.MyPaymentRecordsView MyPaymentRecords
        {
            get { return new Needs.Wl.User.Plat.Views.MyPaymentRecordsView(this); }
        }

        /// <summary>
        /// 我的海关缴税记录（报表）
        /// </summary>
        public Needs.Wl.User.Plat.Views.MyCustomsTaxReportsView MyCustomsTaxReports
        {
            get { return new Needs.Wl.User.Plat.Views.MyCustomsTaxReportsView(this); }
        }

        public Needs.Wl.User.Plat.Views.MainOrdersView MainOrders
        {
            get { return new Needs.Wl.User.Plat.Views.MainOrdersView(this); }
        }

        public Needs.Wl.User.Plat.Views.MyUnReceivedExitNoticeView MyUnReceivedExitNoticeView
        {
            get { return new Needs.Wl.User.Plat.Views.MyUnReceivedExitNoticeView(this); }
        }
    }
}