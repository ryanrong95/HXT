using System;
using Layers.Data.Sqls;
using Yahv.Payments.Views;
using Yahv.Payments.Views.Rolls;
using Yahv.Underly;
using Yahv.Underly.Erps;
using Yahv.Underly.Logs;

namespace Yahv.Systematic
{
    /// <summary>
    /// 财务模块
    /// </summary>
    public class Pays : IAction
    {
        IErpAdmin admin;
        /// <summary>
        /// 构造函数
        /// </summary>
        public Pays(IErpAdmin admin)
        {
            this.admin = admin;
        }

        #region Views
        /// <summary>
        /// 应收、实收视图
        /// </summary>
        public Yahv.Payments.Views.VouchersStatisticsView VouchersStatistics
        {
            get
            {
                return new Yahv.Payments.Views.VouchersStatisticsView();
            }
        }

        /// <summary>
        /// 财务通知视图
        /// </summary>
        public Yahv.Payments.VouchersView Vouchers
        {
            get
            {
                return new Yahv.Payments.VouchersView();
            }
        }

        /// <summary>
        /// 运单视图
        /// </summary>
        public Yahv.Payments.Views.WaybillsTopView Waybills
        {
            get
            {
                return new Yahv.Payments.Views.WaybillsTopView();
            }
        }

        /// <summary>
        /// 清关费用记录
        /// </summary>
        //public Yahv.Payments.Views.CustomsRecordsTopView CustomsRecords
        //{
        //    get
        //    {
        //        return new Yahv.Payments.Views.CustomsRecordsTopView();
        //    }
        //}

        /// <summary>
        /// 受益人视图
        /// </summary>
        //public Yahv.Payments.Views.BeneficiariesTopView Beneficiaries
        //{
        //    get { return new Yahv.Payments.Views.BeneficiariesTopView(); }
        //}

        /// <summary>
        /// 受益人视图(代仓储)
        /// </summary>
        public Yahv.Payments.Views.BeneficiariesTopView WsBeneficiaries
        {
            get { return new Yahv.Payments.Views.BeneficiariesTopView(Business.WarehouseServicing); }
        }

        /// <summary>
        /// 代仓储客户视图
        /// </summary>
        public Yahv.Payments.Views.WsClientsTopView WsClients
        {
            get { return new Yahv.Payments.Views.WsClientsTopView(); }
        }

        /// <summary>
        /// 内部公司视图
        /// </summary>
        public Yahv.Payments.Views.CompaniesTopView Companies
        {
            get { return new Yahv.Payments.Views.CompaniesTopView(); }
        }

        /// <summary>
        /// 应付实付 视图
        /// </summary>
        public Yahv.Payments.Views.PaymentsStatisticsView PaymentsStatistics
        {
            get
            {
                return new Yahv.Payments.Views.PaymentsStatisticsView();
            }
        }

        /// <summary>
        /// 代仓储供应商 视图
        /// </summary>
        public Yahv.Payments.Views.WsSuppliersTopView WsSuppliers
        {
            get
            {
                return new Yahv.Payments.Views.WsSuppliersTopView();
            }
        }

        /// <summary>
        /// 流水账视图
        /// </summary>
        public Yahv.Payments.Views.Rolls.FlowAccountsRoll FlowAccounts
        {
            get
            {
                return new Yahv.Payments.Views.Rolls.FlowAccountsRoll();
            }
        }
        ///// <summary>
        ///// 月结账单
        ///// </summary>
        //public Yahv.Payments.Views.MonthlyBillView MonthlyBill
        //{
        //    get
        //    {
        //        return new Yahv.Payments.Views.MonthlyBillView();
        //    }
        //}


        /// <summary>
        /// 海关发票附件
        /// </summary>
        public Yahv.Payments.Views.InvoiceFilesTopView InvoiceFiles
        {
            get
            {
                return new Yahv.Payments.Views.InvoiceFilesTopView();
            }
        }

        /// <summary>
        /// 企业视图
        /// </summary>
        public Yahv.Payments.Views.EnterprisesTopView Enterprises
        {
            get { return new Yahv.Payments.Views.EnterprisesTopView(); }
        }

        /// <summary>
        /// 实收视图
        /// </summary>
        public Yahv.Payments.ReceivedsView Receiveds
        {
            get { return new Yahv.Payments.ReceivedsView(); }
        }

        /// <summary>
        /// 信用账单
        /// </summary>
        public Yahv.Payments.Views.CreditsRepayStatisticsView CreditsRepay
        {
            get { return new Yahv.Payments.Views.CreditsRepayStatisticsView(); }
        }

        /// <summary>
        /// 优惠券视图
        /// </summary>
        public Yahv.Payments.Views.CouponsView Coupons
        {
            get { return new Yahv.Payments.Views.CouponsView(); }
        }

        /// <summary>
        /// 应收视图
        /// </summary>
        public Yahv.Payments.Views.ReceivablesView Receivables
        {
            get { return new Yahv.Payments.Views.ReceivablesView(); }
        }

        /// <summary>
        /// 快递鸟请求视图
        /// </summary>
        public Yahv.Payments.Views.KdnRequestsTopView KdnRequests
        {
            get { return new Yahv.Payments.Views.KdnRequestsTopView(); }
        }

        /// <summary>
        /// 承运商通用视图
        /// </summary>
        public Yahv.Payments.Views.CarriersTopView Carriers
        {
            get { return new Yahv.Payments.Views.CarriersTopView(); }
        }

        /// <summary>
        /// 实付视图
        /// </summary>
        public Yahv.Payments.PaymentsView Payments
        {
            get { return new Yahv.Payments.PaymentsView(); }
        }

        /// <summary>
        /// 应付视图
        /// </summary>
        public Yahv.Payments.Views.PayablesView Payables
        {
            get { return new Yahv.Payments.Views.PayablesView(); }
        }

        /// <summary>
        /// 租赁订单通用视图
        /// </summary>
        public Yahv.Payments.Views.LsOrderTopView LsOrders
        {
            get { return new Yahv.Payments.Views.LsOrderTopView(); }
        }

        /// <summary>
        /// 科目管理
        /// </summary>
        public Yahv.Payments.SubjectsView Subjects
        {
            get { return new Yahv.Payments.SubjectsView(); }
        }

        /// <summary>
        /// 流水汇总
        /// </summary>
        public Yahv.Payments.Views.FlowAccountsStatisticsView FlowAccountsStatistics
        {
            get { return new Yahv.Payments.Views.FlowAccountsStatisticsView(); }
        }

        /// <summary>
        /// 银行流水汇总
        /// </summary>
        public Yahv.Payments.Views.BankFlowAccountsView BankFlowAccounts
        {
            get { return new Yahv.Payments.Views.BankFlowAccountsView(); }
        }

        /// <summary>
        /// 收款账户
        /// </summary>
        public Yahv.Payments.Views.PayeesTopView Payees
        {
            get
            {
                return new Yahv.Payments.Views.PayeesTopView();
            }
        }

        /// <summary>
        /// 付款账户
        /// </summary>
        public Yahv.Payments.Views.PayersTopView Payers
        {
            get
            {
                return new Yahv.Payments.Views.PayersTopView();
            }
        }

        /// <summary>
        /// admins
        /// </summary>
        public Yahv.Payments.Views.AdminsTopView Admins
        {
            get
            {
                return new Yahv.Payments.Views.AdminsTopView();
            }
        }
        #endregion

        #region Action
        public void Logs_Error(Logs_Error log)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
