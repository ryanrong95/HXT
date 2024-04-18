using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Erp.Generic;

namespace Needs.Wl.Admin.Plat.Models
{
    public partial class Admin
    {
        public Order Order
        {
            get
            {
                return new Order(this);
            }
        }
    }

    public class Order
    {
        IGenericAdmin Admin;

        public Order(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        /// <summary>
        /// 代理订单
        /// </summary>
        public Needs.Ccs.Services.Views.OrdersView Orders
        {
            get
            {
                return new Needs.Ccs.Services.Views.OrdersView();
            }
        }

        /// <summary>
        /// 代理订单
        /// </summary>
        public Needs.Ccs.Services.Views.Orders1View Orders1
        {
            get
            {
                return new Needs.Ccs.Services.Views.Orders1View();
            }
        }

        /// <summary>
        /// 代理订单-纯净
        /// </summary>
        public Needs.Ccs.Services.Views.Orders2View Orders2
        {
            get
            {
                return new Needs.Ccs.Services.Views.Orders2View();
            }
        }

        /// <summary>
        /// 代理订单
        /// </summary>
        public Views.MyOrdersView MyOrders
        {
            get
            {
                return new Views.MyOrdersView(this.Admin);
            }
        }

        /// <summary>
        /// 代理订单1
        /// </summary>
        public Views.MyOrders1View MyOrders1
        {
            get
            {
                return new Views.MyOrders1View(this.Admin);
            }
        }

        /// <summary>
        /// 风控看到的订单
        /// </summary>
        public Views.RiskOrderView RiskOrders
        {
            get
            {
                return new Views.RiskOrderView(this.Admin);
            }
        }

        

        /// <summary>
        /// 草稿订单
        /// </summary>
        public Views.MyDraftOrdersView MyDraftOrders
        {
            get
            {
                return new Views.MyDraftOrdersView(this.Admin);
            }
        }

        /// <summary>
        /// 草稿订单1
        /// </summary>
        public Views.MyDraftOrdersView1 MyDraftOrders1
        {
            get
            {
                return new Views.MyDraftOrdersView1(this.Admin);
            }
        }

        /// <summary>
        /// 已归类，待报价订单
        /// </summary>
        public Views.MyClassifiedOrdersView MyClassifiedOrders
        {
            get
            {
                return new Views.MyClassifiedOrdersView(this.Admin);
            }
        }

        /// <summary>
        /// 已归类，待报价订单
        /// </summary>
        public Views.MyUnClassifyOrdersView1 MyUnClassifyOrdersView1
        {
            get
            {
                return new Views.MyUnClassifyOrdersView1(this.Admin);
            }
        }

        /// <summary>
        /// 草稿订单1
        /// </summary>
        public Views.MyClassifiedOrdersView1 MyClassifiedOrders1
        {
            get
            {
                return new Views.MyClassifiedOrdersView1(this.Admin);
            }
        }

        /// <summary>
        /// 未匹配到货信息的订单
        /// </summary>
        public Views.MyUnMatchedOrdersView MyUnMatchedOrders
        {
            get
            {
                return new Views.MyUnMatchedOrdersView(this.Admin);
            }
        }

        /// <summary>
        /// 已报价，待客户确认订单
        /// </summary>
        public Views.MyQuotedOrdersView MyQuotedOrders
        {
            get
            {
                return new Views.MyQuotedOrdersView(this.Admin);
            }
        }

        /// <summary>
        /// 已报价，待客户确认订单1
        /// </summary>
        public Views.MyQuotedOrdersView1 MyQuotedOrders1
        {
            get
            {
                return new Views.MyQuotedOrdersView1(this.Admin);
            }
        }

        /// <summary>
        /// 已报关，待出库订单的视图
        /// </summary>
        public Views.MyDeclaredOrdersView MyDeclaredOrders
        {
            get
            {
                return new Views.MyDeclaredOrdersView(this.Admin);
            }
        }

        /// <summary>
        /// 已报关，待出库订单的视图1
        /// </summary>
        public Views.MyDeclaredOrdersView1 MyDeclaredOrders1
        {
            get
            {
                return new Views.MyDeclaredOrdersView1(this.Admin);
            }
        }

        /// <summary>
        ///  管理端 待发货页面 过滤跟单员用
        /// </summary>
        public Views.MyMainOrderPendingDeliveryView MyMainOrderPendingDeliveryView
        {
            get
            {
                return new Views.MyMainOrderPendingDeliveryView(this.Admin);
            }
        }

        /// <summary>
        ///  管理端 待发货页面 风控用
        /// </summary>
        public Ccs.Services.Views.DeliveryOrderAdvavceMoneyView DeliveryOrderAdvavceMoneyView
        {
            get
            {
                return new Ccs.Services.Views.DeliveryOrderAdvavceMoneyView();
            }
        }

        /// <summary>
        /// 已退回订单
        /// </summary>
        public Views.MyReturnedOrdersView MyReturnedOrders
        {
            get
            {
                return new Views.MyReturnedOrdersView(this.Admin);
            }
        }

        /// <summary>
        /// 已退回订单
        /// </summary>
        public Views.MyReturnedOrdersView1 MyReturnedOrders1
        {
            get
            {
                return new Views.MyReturnedOrdersView1(this.Admin);
            }
        }

        /// <summary>
        /// 已取消订单
        /// </summary>
        public Views.MyCanceledOrdersView MyCanceledOrders
        {
            get
            {
                return new Views.MyCanceledOrdersView(this.Admin);
            }
        }

        /// <summary>
        /// 已取消订单
        /// </summary>
        public Views.MyCanceledOrdersView1 MyCanceledOrders1
        {
            get
            {
                return new Views.MyCanceledOrdersView1(this.Admin);
            }
        }

        /// <summary>
        /// 已挂起订单
        /// </summary>
        public Views.MyHangUpOrdersView MyHangUpOrders
        {
            get
            {
                return new Views.MyHangUpOrdersView(this.Admin);
            }
        }

        /// <summary>
        /// 已挂起订单1
        /// </summary>
        public Views.MyHangUpOrdersView1 MyHangUpOrders1
        {
            get
            {
                return new Views.MyHangUpOrdersView1(this.Admin);
            }
        }

        /// <summary>
        /// 待付汇订单
        /// </summary>
        public Views.MyUnPayExchangeOrdersView MyUnPayExchangeOrders
        {
            get
            {
                return new Views.MyUnPayExchangeOrdersView(this.Admin);
            }
        }

        /// <summary>
        /// 待付汇订单
        /// </summary>
        public Views.MyUnPayExchangeOrdersView1 MyUnPayExchangeOrders1
        {
            get
            {
                return new Views.MyUnPayExchangeOrdersView1(this.Admin);
            }
        }

        /// <summary>
        /// 待申请开票订单
        /// </summary>
        public Views.MyUnInvoicedOrdersView MyUnInvoicedOrders
        {
            get
            {
                return new Views.MyUnInvoicedOrdersView(this.Admin);
            }
        }

        /// <summary>
        /// 待申请开票订单
        /// </summary>
        public Views.MyUnInvoicedOrdersView1 MyUnInvoicedOrders1
        {
            get
            {
                return new Views.MyUnInvoicedOrdersView1(this.Admin);
            }
        }

        /// <summary>
        /// 可维护费用订单
        /// </summary>
        public Views.MyFeeMaintenanceOrdersView MyFeeMaintenanceOrders
        {
            get
            {
                return new Views.MyFeeMaintenanceOrdersView(this.Admin);
            }
        }

        /// <summary>
        /// 到货订单
        /// </summary>
        public Views.MyOrderArrivalsView MyOrderArrivals
        {
            get
            {
                return new Views.MyOrderArrivalsView(this.Admin);
            }
        }

        /// <summary>
        /// 到货订单
        /// </summary>
        public Views.MyOrderArrivalsView1 MyOrderArrivals1
        {
            get
            {
                return new Views.MyOrderArrivalsView1(this.Admin);
            }
        }

        /// <summary>
        /// 装箱信息
        /// </summary>
        public Views.MyOrderPackingsView MyOrderPackings
        {
            get
            {
                return new Views.MyOrderPackingsView(this.Admin);
            }
        }

        /// <summary>
        /// 装箱信息
        /// </summary>
        public Views.MyOrderPackingsView1 MyOrderPackings1
        {
            get
            {
                return new Views.MyOrderPackingsView1(this.Admin);
            }
        }

        /// <summary>
        /// 待封箱订单
        /// </summary>
        public Views.MyOrderUnSealedsView MyOrderUnSealeds
        {
            get
            {
                return new Views.MyOrderUnSealedsView(this.Admin);
            }
        }

        /// <summary>
        /// 待封箱订单
        /// </summary>
        public Views.MyOrderUnSealedsView1 MyOrderUnSealeds1
        {
            get
            {
                return new Views.MyOrderUnSealedsView1(this.Admin);
            }
        }

        /// <summary>
        /// 代理订单的装箱单
        /// </summary>
        public Needs.Ccs.Services.Views.SortingsView SortingPackings
        {
            get
            {
                return new Needs.Ccs.Services.Views.SortingsView();
            }
        }

        /// <summary>
        /// 待发货订单
        /// </summary>
        public Views.MyOrderDeliveriesView MyOrderDeliveries
        {
            get
            {
                return new Views.MyOrderDeliveriesView(this.Admin);
            }
        }

        /// <summary>
        /// 订单的国际快递
        /// </summary>
        public Needs.Ccs.Services.Views.OrderWaybillView OrderWaybill
        {
            get
            {
                return new Needs.Ccs.Services.Views.OrderWaybillView();
            }
        }

        /// <summary>
        /// 订单的国际快递项
        /// </summary>
        public Needs.Ccs.Services.Views.OrderWaybillItemView OrderWaybillItem
        {
            get
            {
                return new Needs.Ccs.Services.Views.OrderWaybillItemView();
            }
        }

        /// <summary>
        /// 订单项
        /// </summary>
        public Needs.Ccs.Services.Views.OrderItemsView OrderItems
        {
            get
            {
                return new Needs.Ccs.Services.Views.OrderItemsView();
            }
        }

        /// <summary>
        /// 订单费用
        /// </summary>
        public Needs.Ccs.Services.Views.OrderPremiumsView OrderPremiums
        {
            get
            {
                return new Needs.Ccs.Services.Views.OrderPremiumsView();
            }
        }

        /// <summary>
        /// 主订单附件
        /// </summary>
        public Needs.Ccs.Services.Views.MainOrderFilesView MainOrderFiles
        {
            get
            {
                return new Ccs.Services.Views.MainOrderFilesView();
            }
        }

        /// <summary>
        /// 订单附件
        /// </summary>
        public Needs.Ccs.Services.Views.OrderFilesView OrderFiles
        {
            get
            {
                return new Needs.Ccs.Services.Views.OrderFilesView();
            }
        }

        /// <summary>
        /// 代理报关委托书
        /// </summary>
        public Needs.Ccs.Services.Views.OrderAgentProxiesView OrderAgentProxies
        {
            get
            {
                return new Needs.Ccs.Services.Views.OrderAgentProxiesView();
            }
        }

        /// <summary>
        /// 主订单代理报关委托书
        /// </summary>
        public Needs.Ccs.Services.Views.MainOrderAgentProxiesView MainOrderAgentProxies
        {
            get
            {
                return new Needs.Ccs.Services.Views.MainOrderAgentProxiesView();
            }
        }

        /// <summary>
        /// 代理报关委托书
        /// </summary>
        public Views.MyOrderAgentProxiesView MyOrderAgentProxies
        {
            get
            {
                return new Views.MyOrderAgentProxiesView(this.Admin);
            }
        }

        /// <summary>
        /// 对账单
        /// </summary>
        public Needs.Ccs.Services.Views.OrderBillsView OrderBills
        {
            get
            {
                return new Needs.Ccs.Services.Views.OrderBillsView();
            }
        }


        /// <summary>
        /// 对账单 主订单用 新的对账单
        /// </summary>
        //public Needs.Ccs.Services.Views.OrderBillsView2 OrderBills2
        //{
        //    get
        //    {
        //        return new Needs.Ccs.Services.Views.OrderBillsView2();
        //    }
        //}

        /// <summary>
        /// 对账单
        /// </summary>
        public Views.MyOrderBillsView MyOrderBills
        {
            get
            {
                return new Views.MyOrderBillsView(this.Admin);
            }
        }

        /// <summary>
        /// 我的未处理对账单视图  未上传，待上传 对账单用
        /// </summary>
        public Views.MyUnHandleMainOrderBillsView MyUnHandleMainOrderBill
        {
            get
            {
                return new Views.MyUnHandleMainOrderBillsView(this.Admin);
            }
        }

        public Views.MyUnHandleMainOrderAgentView MyUnHandleMainOrderAgent
        {
            get
            {
                return new Views.MyUnHandleMainOrderAgentView(this.Admin);
            }
        }

        /// <summary>
        /// 主订单对账单
        /// </summary>
        public Views.MyMainOrderBillsView MyMainOrderBills
        {
            get
            {
                return new Views.MyMainOrderBillsView(this.Admin);
            }
        }



        public Needs.Ccs.Services.Views.OrderChangeView OrderChanges
        {
            get
            {
                return new Ccs.Services.Views.OrderChangeView();
            }
        }

        /// <summary>
        /// 订单变更
        /// </summary>
        public Views.MyOrderChangeView MyOrderChanges
        {
            get
            {
                return new Views.MyOrderChangeView(this.Admin);
            }
        }


        /// <summary>
        /// 订单管控原因查询
        /// </summary>
        public Needs.Ccs.Services.Views.OrderControlsView OrderControls
        {
            get
            {
                return new Needs.Ccs.Services.Views.OrderControlsView();
            }
        }

        /// <summary>
        /// 税费修改记录
        /// </summary>
        public Needs.Ccs.Services.Views.OrderChangeNoticeLogsListView OrderChangeNoticeLogsList
        {
            get
            {
                return new Needs.Ccs.Services.Views.OrderChangeNoticeLogsListView();
            }
        }

        /// <summary>
        /// 产品变更日志
        /// </summary>
        public Needs.Ccs.Services.Views.Alls.OrderItemChangeLogsAll OrderItemChangeLogs
        {
            get
            {
                return new Ccs.Services.Views.Alls.OrderItemChangeLogsAll();
            }
        }

        /// <summary>
        /// 未处理的的订单项的变更通知
        /// </summary>
        public Needs.Ccs.Services.Views.UnProcessedOrderItemChangeNoticeView UnProcessedOrderItemChangeNoticeView
        {
            get
            {
                return new Ccs.Services.Views.UnProcessedOrderItemChangeNoticeView();
            }
        }

        /// <summary>
        /// （报关通知待制单列表中使用）未处理的管控视图
        /// </summary>
        public Needs.Ccs.Services.Views.UnAuditControlViewForDecNotice UnAuditControlViewForDecNotice
        {
            get
            {
                return new Ccs.Services.Views.UnAuditControlViewForDecNotice();
            }
        }

        /// <summary>
        /// 销售合同订单
        /// </summary>
        public Ccs.Services.Views.SalesContractOrdersView SalesContractOrderView
        {
            get
            {
                return new Ccs.Services.Views.SalesContractOrdersView();
            }
        }

        /// <summary>
        /// 待收款订单
        /// </summary>
        public Views.MyUnCollectedOrdersView MyUnCollectedOrders
        {
            get
            {
                return new Views.MyUnCollectedOrdersView(this.Admin);
            }
        }
    }
}
