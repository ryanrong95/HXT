using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Services.Enums;
using Yahv.Underly;

namespace Yahv.PvWsClient.Model
{
    /// <summary>
    /// 会员端统一调用路径
    /// </summary>
    public partial class ClientUser
    {
        /// <summary>
        /// 日志调用
        /// </summary>
        public Yahv.Services.OperatingLogger OrderOperateLog
        {
            get
            {
                return new CenterLog(this)[LogType.WsOrder];
            }
        }

        public Yahv.Services.OperatingLogger Errorlog
        {
            get
            {
                return new CenterLog(this)[LogType.Error];
            }
        }

        #region 订单
        /// <summary>
        /// 我的所有订单
        /// </summary>
        public PvWsOrder.Services.ClientViews.MyOrders MyOrder
        {
            get
            {
                return new PvWsOrder.Services.ClientViews.MyOrders(this);
            }
        }


        /// <summary>
        /// 租赁订单列表
        /// </summary>
        public PvWsOrder.Services.ClientViews.MyLsOrdersView MyLsOrdersList
        {
            get
            {
                return new PvWsOrder.Services.ClientViews.MyLsOrdersView(this);
            }
        }

        /// <summary>
        /// 报关订单
        /// </summary>
        public PvWsOrder.Services.ClientViews.DeclareOrders MyDeclareOrders
        {
            get
            {
                return new PvWsOrder.Services.ClientViews.DeclareOrders(this);
            }
        }

        /// <summary>
        /// 带客户确认订单
        /// </summary>
        public PvWsOrder.Services.ClientViews.UnConfirmedOrders MyUnConfirmedOrders
        {
            get
            {
                return new PvWsOrder.Services.ClientViews.UnConfirmedOrders(this);
            }
        }

        /// <summary>
        /// 待确认收货订单
        /// </summary>
        public PvWsOrder.Services.ClientViews.UnReceivedOrderView MyUnReceiptedOrders
        {
            get
            {
                return new PvWsOrder.Services.ClientViews.UnReceivedOrderView(this);
            }
        }

        /// <summary>
        /// 发货订单
        /// </summary>
        public PvWsOrder.Services.ClientViews.DeliveryOrders MyDeliveryOrders
        {
            get
            {
                return new PvWsOrder.Services.ClientViews.DeliveryOrders(this);
            }
        }

        /// <summary>
        /// 收货订单
        /// </summary>
        public PvWsOrder.Services.ClientViews.ReceivedOrders MyReceivedOrders
        {
            get
            {
                return new PvWsOrder.Services.ClientViews.ReceivedOrders(this);
            }
        }

        /// <summary>
        /// 代收货款订单
        /// </summary>
        public PvWsOrder.Services.ClientViews.ReceiveAmountOrder UnReceiveAmountOrdes
        {
            get
            {
                return new PvWsOrder.Services.ClientViews.ReceiveAmountOrder(this);
            }
        }
        #endregion


        #region 财务管理
        /// <summary>
        /// 我的订单账单明细
        /// </summary>
        public PvWsOrder.Services.ClientViews.MyOrderBillView MyOrderBills
        {
            get
            {
                return new PvWsOrder.Services.ClientViews.MyOrderBillView(this);
            }
        }

        /// <summary>
        /// 我的付汇申请
        /// </summary>
        public PvWsOrder.Services.XDTClientView.MyPayExchangeApplies MyPayExchangeApplies
        {
            get
            {
                return new PvWsOrder.Services.XDTClientView.MyPayExchangeApplies(this);
            }

        }

        /// <summary>
        /// 我的代付货款申请
        /// </summary>
        public PvWsOrder.Services.ClientViews.MyApplicationView MyApplictions
        {
            get
            {
                return new PvWsOrder.Services.ClientViews.MyApplicationView(this);
            }
        }
        #endregion


        #region 客户基础信息
        /// <summary>
        /// 我的公司信息
        /// </summary>
        public Yahv.Services.Models.WsClient MyClients
        {
            get
            {
                return new Yahv.Services.Views.WsClientsTopView<PvWsOrderReponsitory>().SingleOrDefault(item => item.Name == this.XDTClientName);
            }
        }

        /// <summary>
        /// 我的供应商信息
        /// </summary>
        public Yahv.PvWsOrder.Services.ClientViews.MySuppliers MySupplier
        {
            get
            {
                return new PvWsOrder.Services.ClientViews.MySuppliers(this.EnterpriseID);
            }
        }

        /// <summary>
        /// 我的收货地址
        /// </summary>
        public Yahv.PvWsOrder.Services.ClientViews.MyConsigneesView MyConsignees
        {
            get
            {
                return new PvWsOrder.Services.ClientViews.MyConsigneesView(this.EnterpriseID);
            }
        }

        /// <summary>
        /// 我的客户联系人
        /// </summary>
        public Yahv.PvWsOrder.Services.ClientViews.MyContactsView MyContacts
        {
            get
            {
                return new PvWsOrder.Services.ClientViews.MyContactsView(this.EnterpriseID);
            }
        }

        /// <summary>
        /// 条款协议
        /// </summary>
        public Yahv.PvWsOrder.Services.XDTClientView.ClientAgreementView<ScCustomReponsitory> MyAgreement
        {
            get
            {
                return new PvWsOrder.Services.XDTClientView.ClientAgreementView<ScCustomReponsitory>(this);
            }
        }

        /// <summary>
        /// 我的发票信息
        /// </summary>
        public PvWsOrder.Services.ClientViews.MyInvoicesView MyInvoice
        {
            get
            {
                return new PvWsOrder.Services.ClientViews.MyInvoicesView(this.EnterpriseID);
            }
        }

        /// <summary>
        /// 我的付款人
        /// </summary>
        public PvWsOrder.Services.ClientViews.MyPayerView MyPayers
        {
            get
            {
                return new PvWsOrder.Services.ClientViews.MyPayerView(this.EnterpriseID);
            }
        }

        /// <summary>
        /// 我的个人发票信息
        /// </summary>
        public Yahv.PvWsOrder.Services.ClientViews.MyPersonInvoicesView MyPersonInvoices
        {
            get
            {
                return new PvWsOrder.Services.ClientViews.MyPersonInvoicesView(this.EnterpriseID);
            }
        }

        #endregion


        #region 库存管理
        /// <summary>
        /// 入库单
        /// </summary>
        public PvWsOrder.Services.ClientViews.InputReportTopView MyInStorage
        {
            get
            {
                return new PvWsOrder.Services.ClientViews.InputReportTopView(this.EnterpriseID);
            }
        }

        /// <summary>
        /// 入库单
        /// </summary>
        public PvWsOrder.Services.ClientViews.OutputReportTopView MyOutStorage
        {
            get
            {
                return new PvWsOrder.Services.ClientViews.OutputReportTopView(this.EnterpriseID);
            }
        }

        /// <summary>
        /// 我的库存页面
        /// </summary>
        public PvWsOrder.Services.ClientViews.MyStorageView MyStorages
        {
            get
            {
                return new PvWsOrder.Services.ClientViews.MyStorageView(this.EnterpriseID);
            }
        }
        #endregion


        #region 芯达通视图
        /// <summary>
        /// 芯达通所有订单视图
        /// </summary>
        public PvWsOrder.Services.XDTClientView.XDTOrderView MyXDTOrder
        {
            get
            {
                return new PvWsOrder.Services.XDTClientView.XDTOrderView(this);
            }
        }

        /// <summary>
        /// 芯达通发票
        /// </summary>
        public PvWsOrder.Services.XDTClientView.XDTOrderInvoiceTopView MyInvoices
        {
            get
            {
                return new PvWsOrder.Services.XDTClientView.XDTOrderInvoiceTopView(this.XDTClientID);
            }
        }

        /// <summary>
        /// 芯达通海关发票
        /// </summary>
        public PvWsOrder.Services.XDTClientView.XDTCustomsInvoiceView MyCusInvoices
        {
            get
            {
                return new PvWsOrder.Services.XDTClientView.XDTCustomsInvoiceView(this.XDTClientID);
            }
        }

        /// <summary>
        /// 待付汇
        /// </summary>
        public PvWsOrder.Services.XDTClientView.PayExchangeOrderView MyUnPayExchangeOrder
        {
            get
            {
                return new PvWsOrder.Services.XDTClientView.PayExchangeOrderView(this);
            }
        }

        ///// <summary>
        ///// 报关单+转报关
        ///// </summary>
        //public PvWsOrder.Services.ClientViews.MyXDTDecOrders MyXDTDecOrders
        //{
        //    get
        //    {
        //        return new PvWsOrder.Services.ClientViews.MyXDTDecOrders(this);
        //    }
        //}

        /// <summary>
        /// 报关单
        /// </summary>
        public PvWsOrder.Services.XDTClientView.ClientDecHeadsView MyDecHeads
        {
            get
            {
                return new PvWsOrder.Services.XDTClientView.ClientDecHeadsView(this);
            }
        }

        /// <summary>
        /// 报关单数据
        /// </summary>
        public PvWsOrder.Services.XDTClientView.ClientDecHeadDataView MyDecHeadData
        {
            get
            {
                return new PvWsOrder.Services.XDTClientView.ClientDecHeadDataView(this);
            }
        }

        /// <summary>
        /// 缴税记录
        /// </summary>
        public PvWsOrder.Services.XDTClientView.ClientCustomsTaxReportView MyTaxReports
        {
            get
            {
                return new PvWsOrder.Services.XDTClientView.ClientCustomsTaxReportView(this);
            }
        }

        /// <summary>
        /// 税单记录
        /// </summary>
        public PvWsOrder.Services.XDTClientView.ClientTaxRecordsView MytaxRecords
        {
            get
            {
                return new PvWsOrder.Services.XDTClientView.ClientTaxRecordsView(this);
            }
        }

        /// <summary>
        /// 报关未付汇订单
        /// </summary>
        public PvWsOrder.Services.XDTClientView.ClientUnPayexchangeView UnPayexchangeOrders
        {
            get
            {
                return new PvWsOrder.Services.XDTClientView.ClientUnPayexchangeView(this);
            }
        }
        #endregion

        #region 欠款额度
        /// <summary>
        /// 我的欠款额度
        /// </summary>
        public Yahv.PvWsOrder.Services.ClientViews.MyCreditsStatisticsView MyCredits
        {
            get
            {
                return new PvWsOrder.Services.ClientViews.MyCreditsStatisticsView(this.EnterpriseID);
            }
        }

        /// <summary>
        /// 已使用欠款额度
        /// </summary>
        public Yahv.PvWsOrder.Services.ClientViews.MyCreditsUsdStatisticsView MyUsdCredits
        {
            get
            {
                return new PvWsOrder.Services.ClientViews.MyCreditsUsdStatisticsView(this.EnterpriseID);
            }
        }
        #endregion

        /// <summary>
        /// 客户应付款统计
        /// </summary>
        public Yahv.PvWsOrder.Services.XDTClientView.ClientAccountPayablesView ClientAccountPayables
        {
            get
            {
                return new Yahv.PvWsOrder.Services.XDTClientView.ClientAccountPayablesView();
            }
        }

        /// <summary>
        /// 客户付款记录
        /// </summary>
        public Yahv.PvWsOrder.Services.ClientViews.PaymentRecordsView MyPaymentRecords
        {
            get
            {
                return new Yahv.PvWsOrder.Services.ClientViews.PaymentRecordsView(this);
            }
        }

        /// <summary>
        /// 消息设置
        /// </summary>
        public Yahv.PvWsOrder.Services.ClientViews.MyMsgConfigView MyMsgConfig
        {
            get
            {
                return new Yahv.PvWsOrder.Services.ClientViews.MyMsgConfigView(this);
            }
        }

        #region 暂时不用，原来使用

        #region 首页统计数据(咱不用)
        ///// <summary>
        ///// 我的账户应付款
        ///// </summary>
        //public PvWsOrder.Services.ClientViews.MyAccountPayableView MyPayable
        //{
        //    get
        //    {
        //        return new PvWsOrder.Services.ClientViews.MyAccountPayableView(this.EnterpriseID);
        //    }
        //}

        ///// <summary>
        ///// 我的余额视图
        ///// </summary>
        //public PvWsOrder.Services.ClientViews.MyBalanceView MyBalance
        //{
        //    get
        //    {
        //        return new PvWsOrder.Services.ClientViews.MyBalanceView(this.EnterpriseID);
        //    }
        //}



        ///// <summary>
        ///// 我的账单
        ///// </summary>
        //public PvWsOrder.Services.ClientViews.MyVouchersStatisticsView MyVouchers
        //{
        //    get
        //    {
        //        return new PvWsOrder.Services.ClientViews.MyVouchersStatisticsView(this.EnterpriseID);
        //    }
        //}

        /// <summary>
        /// 客户的业务员
        /// </summary>
        public Yahv.Services.Models.AdminInfo ServiceManager
        {
            get
            {
                return new Yahv.PvWsOrder.Services.ClientViews.TrackerAdminsView(MapsType.ServiceManager, this.EnterpriseID).FirstOrDefault();
            }
        }

        /// <summary>
        /// 客户的跟单员
        /// </summary>
        public Yahv.Services.Models.AdminInfo Merchandiser
        {
            get
            {
                return new Yahv.PvWsOrder.Services.ClientViews.TrackerAdminsView(MapsType.Merchandiser, this.EnterpriseID).FirstOrDefault();
            }
        }

        /// <summary>
        /// 经理，暂时直接默认张庆永
        /// </summary>
        public Yahv.Services.Models.AdminInfo Manager
        {
            get
            {
                return new Yahv.Services.Views.AdminsInfoTopView<PvWsOrderReponsitory>()["Admin00526"];
            }
        }
        #endregion
        ///// <summary>
        ///// 我的信用额度
        ///// </summary>
        //public Yahv.PvWsOrder.Services.ClientViews.MyCreditsStatisticsView MyCredits
        //{
        //    get
        //    {
        //        return new PvWsOrder.Services.ClientViews.MyCreditsStatisticsView(this.EnterpriseID);
        //    }
        //}

        ///// <summary>
        ///// 我的订单操作进展
        ///// </summary>
        //public Yahv.PvWsOrder.Services.ClientViews.MyOrderOperationLogView MyOrderOperations
        //{
        //    get
        //    {
        //        return new PvWsOrder.Services.ClientViews.MyOrderOperationLogView(this);
        //    }
        //}

        ///// <summary>
        ///// 代发货订单
        ///// </summary>
        //public PvWsOrder.Services.ClientViews.DeliveryOrderView MyDeliveryOrder
        //{
        //    get
        //    {
        //        return new PvWsOrder.Services.ClientViews.DeliveryOrderView(this);
        //    }
        //}

        ///// <summary>
        ///// 代入仓订单
        ///// </summary>
        //public PvWsOrder.Services.ClientViews.RecievedOrderView MyRecievedOrder
        //{
        //    get
        //    {
        //        return new PvWsOrder.Services.ClientViews.RecievedOrderView(this);
        //    }
        //}

        ///// <summary>
        ///// 代转运订单
        ///// </summary>
        //public PvWsOrder.Services.ClientViews.TransportOrderView MyTransOrder
        //{
        //    get
        //    {
        //        return new PvWsOrder.Services.ClientViews.TransportOrderView(this);
        //    }
        //}

        ///// <summary>
        ///// 转报关订单
        ///// </summary>
        //public PvWsOrder.Services.ClientViews.TransDeclareOrderView MyTransDeclareOrder
        //{
        //    get
        //    {
        //        return new PvWsOrder.Services.ClientViews.TransDeclareOrderView(this);
        //    }
        //}

        ///// <summary>
        ///// 报关订单
        ///// </summary>
        //public PvWsOrder.Services.ClientViews.DeclareOrderView MyDeclareOrder
        //{
        //    get
        //    {
        //        return new PvWsOrder.Services.ClientViews.DeclareOrderView(this);
        //    }
        //}        

        ///// <summary>
        ///// 待收货订单
        ///// </summary>
        //public PvWsOrder.Services.ClientViews.UnReceivedOrderView UnReceivedOrders
        //{
        //    get
        //    {
        //        return new PvWsOrder.Services.ClientViews.UnReceivedOrderView(this);
        //    }
        //}

        ///// <summary>
        ///// 我的租赁订单
        ///// </summary>
        //public PvWsOrder.Services.ClientViews.MyLsOrders MyLsOrders
        //{
        //    get
        //    {
        //        return new PvWsOrder.Services.ClientViews.MyLsOrders(this);
        //    }
        //}

        ///// <summary>
        ///// 获取当前客户所有优惠券
        ///// </summary>
        //public Yahv.Payments.Views.MyCouponsView MyCoupons
        //{
        //    get
        //    {
        //        return new Payments.Views.MyCouponsView(PvWsOrder.Services.PvClientConfig.CompanyID, this.EnterpriseID);
        //    }
        //}

        /// <summary>
        /// 我的产品
        /// </summary>
        public Yahv.PvWsOrder.Services.ClientViews.MyClientProductsView MyClientProducts
        {
            get
            {
                return new PvWsOrder.Services.ClientViews.MyClientProductsView(this.XDTClientID);
            }
        }

        /// <summary>
        /// 产品预归类
        /// </summary>
        public PvWsOrder.Services.XDTClientView.ClientClassifiedPreProductsView MyClassifiedPreProducts
        {
            get
            {
                return new PvWsOrder.Services.XDTClientView.ClientClassifiedPreProductsView(this);
            }
        }

        /// <summary>
        /// 预归类产品
        /// </summary>
        public PvWsOrder.Services.XDTClientView.ClientPreProductsView MyPreProducts
        {
            get
            {
                return new PvWsOrder.Services.XDTClientView.ClientPreProductsView(this);
            }
        }

        ///// <summary>
        ///// 现金流水
        ///// </summary>
        //public PvWsOrder.Services.ClientViews.MyCashRecordsView MyCashRecords
        //{
        //    get
        //    {
        //        return new PvWsOrder.Services.ClientViews.MyCashRecordsView(this.EnterpriseID);
        //    }
        //}

        ///// <summary>
        ///// 信用流水
        ///// </summary>
        //public PvWsOrder.Services.ClientViews.MyCreditRecordsView MyCreditRecords
        //{
        //    get
        //    {
        //        return new PvWsOrder.Services.ClientViews.MyCreditRecordsView(this.EnterpriseID);
        //    }
        //}


        ///// <summary>
        ///// 我的待确认订单
        ///// </summary>
        //public PvWsOrder.Services.ClientViews.ConfirmOrderView MyConfirmOrder
        //{
        //    get
        //    {
        //        return new PvWsOrder.Services.ClientViews.ConfirmOrderView(this);
        //    }
        //}
        #endregion
    }
}
