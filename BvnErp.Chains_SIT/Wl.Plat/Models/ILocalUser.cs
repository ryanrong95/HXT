using Needs.Wl.Client.Services.Views;

namespace Needs.Wl.User.Plat.Models
{
    public partial interface ILocalUser
    {
        WebSites WebSite { get; }

        /// <summary>
        /// 订单路径
        /// 不进行订单的查询，用于订单子项的调用。
        /// </summary>
        OrderContext OrderContext { get; }

        /// <summary>
        /// 付款记录
        /// 不进行付款记录的查询，用于子项的调用。
        /// </summary>
        PaymentRecordContext PaymentRecordContext { get; }

        /// <summary>
        /// 我的供应商
        /// </summary>
        ClientSuppliersView MySuppliers { get; }

        /// <summary>
        /// 我的收件地址
        /// </summary>
        ClientConsigneesView MyConsignees { get; }

        /// <summary>
        /// 我的产品
        /// </summary>
        ClientProductsView MyProducts { get; }

        /// <summary>
        /// 我的预归类产品
        /// </summary>
        ClientPreProductsView MyPreProducts { get; }

        /// <summary>
        /// 我的预归类产品
        /// </summary>
        ClientClassifiedPreProductsView MyClassifiedPreProducts { get; }

        /// <summary>
        /// 我的自定义产品税务归类
        /// </summary>
        ClientProductTaxCategoriesView MyProductTaxCategories { get; }

        /// <summary>
        /// 我的付汇申请（订单）
        /// </summary>
        Needs.Wl.User.Plat.Views.MyPayExchangeAppliesView MyPayExchangeApplies { get; }

        /// <summary>
        /// 我的全部订单
        /// </summary>
        Needs.Wl.User.Plat.Views.MyAllOrdersView MyAllOrders { get; }

        /// <summary>
        /// 我的订单
        /// </summary>
        Needs.Wl.User.Plat.Views.MyOrdersView MyOrders { get; }     
        
        /// <summary>
        /// 我的主订单
        /// </summary>
        Needs.Wl.User.Plat.Views.MyMainOrdersView MyMianOrders { get; }

        /// <summary>
        /// 我的待付汇订单
        /// </summary>
        Needs.Wl.User.Plat.Views.MyUnPayExchangeOrdersView MyUnPayExchangeOrders { get; }

        /// <summary>
        /// 我的未开票的订单
        /// </summary>
        Needs.Wl.User.Plat.Views.MyUnInvocieOrdersView MyUnInvocieOrders { get; }

        /// <summary>
        /// 我的退回的订单
        /// </summary>
        Needs.Wl.User.Plat.Views.MyRejectedOrdersView MyRejectedOrders { get; }

        /// <summary>
        /// 我的待收货的订单
        /// </summary>
        Needs.Wl.User.Plat.Views.MyUnReceivedOrdersView MyUnReceivedOrders { get; }

        /// <summary>
        /// 我的草稿订单
        /// </summary>
        Needs.Wl.User.Plat.Views.MyDraftOrdersView MyDraftOrders { get; }

        /// <summary>
        /// 我的报关单
        /// </summary>
        Needs.Wl.User.Plat.Views.MyDecHeadsView MyDecHeads { get; }

        /// <summary>
        /// 我的付款记录
        /// </summary>
        Needs.Wl.User.Plat.Views.MyPaymentRecordsView MyPaymentRecords { get; }

        /// <summary>
        /// 我的海关缴税记录（报表）
        /// </summary>
        Needs.Wl.User.Plat.Views.MyCustomsTaxReportsView MyCustomsTaxReports { get; }

        /// <summary>
        /// 主订单
        /// </summary>
        Needs.Wl.User.Plat.Views.MainOrdersView MainOrders { get; }
        /// <summary>
        /// 我的未收到货的出库通知
        /// </summary>
        Needs.Wl.User.Plat.Views.MyUnReceivedExitNoticeView MyUnReceivedExitNoticeView { get; }
    }
}