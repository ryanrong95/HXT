using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yahv.PvWsOrder.Services.Models;
using Yahv.PvWsOrder.Services.Views;
using Yahv.Services.Views;
using Yahv.Underly.Erps;
using Yahv.Underly.Logs;

namespace Yahv.Systematic
{
    public class WsOrder : IAction
    {
        IErpAdmin admin;
        public WsOrder(IErpAdmin admin)
        {
            this.admin = admin;
        }

        public void Logs_Error(Logs_Error log)
        {
            throw new NotImplementedException();
        }

        public Yahv.PvWsOrder.Services.Views.CompanysAll Companys
        {
            get
            {
                return new PvWsOrder.Services.Views.CompanysAll();
            }
        }
        public Yahv.PvWsOrder.Services.Views.WsClientsAlls WsClients
        {
            get
            {
                return new PvWsOrder.Services.Views.WsClientsAlls();
            }
        }
        public Yahv.PvWsOrder.Services.Views.MyWsClientsAlls MyWsClients
        {
            get
            {
                return new PvWsOrder.Services.Views.MyWsClientsAlls(this.admin);
            }
        }
        public Yahv.PvWsOrder.Services.Views.WsSupplierAlls Suppliers
        {
            get
            {
                return new PvWsOrder.Services.Views.WsSupplierAlls();
            }
        }
        //客户是个人的发票
        public Yahv.PvWsOrder.Services.Views.Origins.vInvoicesTopView vInvoices
        {
            get
            {
                return new Yahv.PvWsOrder.Services.Views.Origins.vInvoicesTopView();
            }
        }


        public Yahv.PvWsOrder.Services.Views.StoragesAlls Storages
        {
            get
            {
                return new PvWsOrder.Services.Views.StoragesAlls();
            }
        }
        public Yahv.PvWsOrder.Services.Views.HKStoragesAlls HKStorages
        {
            get
            {
                return new PvWsOrder.Services.Views.HKStoragesAlls();
            }
        }
        public Yahv.PvWsOrder.Services.Views.Alls.ApplicationsAlls Applications
        {
            get
            {
                return new PvWsOrder.Services.Views.Alls.ApplicationsAlls();
            }
        }
        public Yahv.PvWsOrder.Services.Views.Alls.MyApplicationView MyApplication(string companyID)
        {
            if (string.IsNullOrEmpty(companyID))
            {
                companyID = "DBAEAB43B47EB4299DD1D62F764E6B6A";
            }
            return new PvWsOrder.Services.Views.Alls.MyApplicationView(this.admin, companyID);
        }

        #region 租赁订单
        public Yahv.PvWsOrder.Services.Views.LsOrdersAll LsOrderAll
        {
            get
            {
                return new Yahv.PvWsOrder.Services.Views.LsOrdersAll();
            }
        }
        /// <summary>
        /// 租赁订单分页
        /// </summary>
        /// <param name="companyID"></param>
        /// <returns></returns>
        public Yahv.PvWsOrder.Services.Views.BaseLsOrderView LsOrder(string companyID)
        {
            if (string.IsNullOrEmpty(companyID))
            {
                companyID = "DBAEAB43B47EB4299DD1D62F764E6B6A";
            }
            return new Yahv.PvWsOrder.Services.Views.BaseLsOrderView(this.admin, companyID);
        }
        public Yahv.PvWsOrder.Services.Views.LsOrderItemsAll LsOrderItem
        {
            get
            {
                return new Yahv.PvWsOrder.Services.Views.LsOrderItemsAll();
            }
        }
        #endregion

        #region 代仓储订单
        public Yahv.PvWsOrder.Services.Views.OrderAlls Orders
        {
            get
            {
                return new PvWsOrder.Services.Views.OrderAlls();
            }
        }
        /// <summary>
        /// 所有代仓储订单(包含挂起订单)
        /// </summary>
        public Yahv.PvWsOrder.Services.Views.BaseOrderView BaseOrders
        {
            get
            {
                return new PvWsOrder.Services.Views.BaseOrderView();
            }
        }
        public Yahv.PvWsOrder.Services.Views.OrderItemsAlls OrderItems
        {
            get
            {
                return new PvWsOrder.Services.Views.OrderItemsAlls();
            }
        }
        public Yahv.PvWsOrder.Services.ClientViews.OrderItemAlls DeclareOrderItems
        {
            get
            {
                using (var Reponsitory = new Layers.Data.Sqls.PvWsOrderReponsitory())
                {
                    return new Yahv.PvWsOrder.Services.ClientViews.OrderItemAlls(Reponsitory);
                }
            }
        }
        #endregion

        #region 我的代仓储订单
        public Yahv.PvWsOrder.Services.Views.MyBaseOrderView MyBaseOrders(string companyID)
        {
            if (string.IsNullOrEmpty(companyID))
            {
                companyID = "DBAEAB43B47EB4299DD1D62F764E6B6A";
            }
            return new PvWsOrder.Services.Views.MyBaseOrderView(this.admin, companyID);
        }
        public Yahv.PvWsOrder.Services.Views.MyRecievedOrderView MyRecievedOrders(string companyID)
        {
            if (string.IsNullOrEmpty(companyID))
            {
                companyID = "DBAEAB43B47EB4299DD1D62F764E6B6A";
            }
            return new PvWsOrder.Services.Views.MyRecievedOrderView(this.admin, companyID);
        }
        public Yahv.PvWsOrder.Services.Views.MyTransportOrderView MyTransportOrders(string companyID)
        {
            if (string.IsNullOrEmpty(companyID))
            {
                companyID = "DBAEAB43B47EB4299DD1D62F764E6B6A";
            }
            return new PvWsOrder.Services.Views.MyTransportOrderView(this.admin, companyID);
        }
        public Yahv.PvWsOrder.Services.Views.MyDeliveryOrderView MyDeliveryOrders(string companyID)
        {
            if (string.IsNullOrEmpty(companyID))
            {
                companyID = "DBAEAB43B47EB4299DD1D62F764E6B6A";
            }
            return new PvWsOrder.Services.Views.MyDeliveryOrderView(this.admin, companyID);
        }
        public Yahv.PvWsOrder.Services.Views.MyDeclareOrderView MyDeclareOrders(string companyID)
        {
            if (string.IsNullOrEmpty(companyID))
            {
                companyID = "DBAEAB43B47EB4299DD1D62F764E6B6A";
            }
            return new PvWsOrder.Services.Views.MyDeclareOrderView(this.admin, companyID);
        }
        public Yahv.PvWsOrder.Services.Views.MyTurnDeclareOrderView MyTurnDeclareOrders(string companyID)
        {
            if (string.IsNullOrEmpty(companyID))
            {
                companyID = "DBAEAB43B47EB4299DD1D62F764E6B6A";
            }
            return new PvWsOrder.Services.Views.MyTurnDeclareOrderView(this.admin, companyID);
        }
        public Yahv.PvWsOrder.Services.Views.MyCnyOrderView MyCnyOrders(string companyID)
        {
            if (string.IsNullOrEmpty(companyID))
            {
                companyID = "DBAEAB43B47EB4299DD1D62F764E6B6A";
            }
            return new PvWsOrder.Services.Views.MyCnyOrderView(this.admin, companyID);
        }
        #endregion

        #region 其它通用视图
        //公司发票
        public Yahv.Services.Views.WsInvoicesTopView<Layers.Data.Sqls.PvWsOrderReponsitory> Invoices
        {
            get
            {
                return new Yahv.Services.Views.WsInvoicesTopView<Layers.Data.Sqls.PvWsOrderReponsitory>();
            }
        }
        public Yahv.Services.Views.CarriersTopView<Layers.Data.Sqls.PvWsOrderReponsitory> Carriers
        {
            get
            {
                return new Services.Views.CarriersTopView<Layers.Data.Sqls.PvWsOrderReponsitory>();
            }
        }
        public Yahv.Services.Views.WsContactsTopView<Layers.Data.Sqls.PvWsOrderReponsitory> Contacts
        {
            get
            {
                return new Yahv.Services.Views.WsContactsTopView<Layers.Data.Sqls.PvWsOrderReponsitory>();
            }
        }
        public Yahv.Services.Views.WsConsigneesTopView<Layers.Data.Sqls.PvWsOrderReponsitory> Consignees
        {
            get
            {
                return new Yahv.Services.Views.WsConsigneesTopView<Layers.Data.Sqls.PvWsOrderReponsitory>();
            }
        }
        public Yahv.Services.Views.LsProductTopView<Layers.Data.Sqls.PvLsOrderReponsitory> LsProduct
        {
            get
            {
                return new Yahv.Services.Views.LsProductTopView<Layers.Data.Sqls.PvLsOrderReponsitory>();
            }
        }
        public Yahv.Services.Views.LsProductPriceTopView<Layers.Data.Sqls.PvLsOrderReponsitory> LsProductPrice
        {
            get
            {
                return new Yahv.Services.Views.LsProductPriceTopView<Layers.Data.Sqls.PvLsOrderReponsitory>();
            }
        }
        public Yahv.Services.Views.ReceivedsTopView<Layers.Data.Sqls.PvbCrmReponsitory> Receiveds
        {
            get
            {
                return new Yahv.Services.Views.ReceivedsTopView<Layers.Data.Sqls.PvbCrmReponsitory>();
            }
        }
        public Yahv.Services.Views.PaymentsTopView<Layers.Data.Sqls.PvbCrmReponsitory> Payments
        {
            get
            {
                return new Yahv.Services.Views.PaymentsTopView<Layers.Data.Sqls.PvbCrmReponsitory>();
            }
        }
        public Yahv.Services.Views.CgTempStoragesTopView<Layers.Data.Sqls.PvWsOrderReponsitory> TempStorages
        {
            get
            {
                return new Yahv.Services.Views.CgTempStoragesTopView<Layers.Data.Sqls.PvWsOrderReponsitory>();
            }
        }
        /// <summary>
        /// 公有付款人：客户付款人
        /// </summary>
        public Yahv.Services.Views.wsPayersTopView<Layers.Data.Sqls.PvWsOrderReponsitory> Payers
        {
            get
            {
                return new Yahv.Services.Views.wsPayersTopView<Layers.Data.Sqls.PvWsOrderReponsitory>();
            }
        }
        /// <summary>
        /// 内部公司的收款人
        /// </summary>
        public Yahv.Services.Views.wsPayeesTopView<Layers.Data.Sqls.PvWsOrderReponsitory> Payees
        {
            get
            {
                return new Yahv.Services.Views.wsPayeesTopView<Layers.Data.Sqls.PvWsOrderReponsitory>();
            }
        }
        public Yahv.Services.Views.wsnSupplierPayeesTopView<Layers.Data.Sqls.PvWsOrderReponsitory> SupplierPayees
        {
            get
            {
                return new Yahv.Services.Views.wsnSupplierPayeesTopView<Layers.Data.Sqls.PvWsOrderReponsitory>();
            }
        }
        /// <summary>
        /// 所有的供应商受益人
        /// </summary>
        public wsnSupplierConsignorsTopView<PvWsOrderReponsitory> SupplierConsignors
        {
            get
            {
                return new wsnSupplierConsignorsTopView<PvWsOrderReponsitory>();
            }
        }
        public VouchersCnyStatisticsView<PvbCrmReponsitory> VouchersCnyStatistics
        {
            get
            {
                return new VouchersCnyStatisticsView<PvbCrmReponsitory>();
            }
        }

        /// <summary>
        /// 资金中心
        /// </summary>
        public Yahv.Services.Views.PvFinance.AccountWorksStatisticsView<PvWsOrderReponsitory> AccountWorks
        {
            get
            {
                return new Yahv.Services.Views.PvFinance.AccountWorksStatisticsView<PvWsOrderReponsitory>();
            }
        }
        #endregion

        #region 深圳代仓储
        public Yahv.PvWsOrder.Services.Views.MyTempStoragesAlls MyTempStorage
        {
            get
            {
                return new Yahv.PvWsOrder.Services.Views.MyTempStoragesAlls(this.admin);
            }
        }

        public Yahv.PvWsOrder.Services.Views.MyHandledTempStoragesAlls MyHandledTempStorage
        {
            get
            {
                return new Yahv.PvWsOrder.Services.Views.MyHandledTempStoragesAlls(this.admin);
            }
        }
        #endregion

    }
}
