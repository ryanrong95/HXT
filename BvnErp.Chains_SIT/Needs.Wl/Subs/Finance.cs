using Needs.Erp.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Admin.Plat.Models
{
    public partial class Admin
    {
        public Finance Finance
        {
            get
            {
                return new Finance(this);
            }
        }
    }
    public class Finance
    {
        IGenericAdmin Admin;

        public Finance(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        public Ccs.Services.Views.InvoiceOrderItemView InvoiceOrderItem
        {
            get
            {
                return new Ccs.Services.Views.InvoiceOrderItemView();
            }
        }

        /// <summary>
        /// 开票通知
        /// </summary>
        public Needs.Ccs.Services.Views.InvoiceNoticeView InvoiceNotice
        {
            get
            {
                return new Needs.Ccs.Services.Views.InvoiceNoticeView();
            }
        }

        /// <summary>
        /// 开票通知明细
        /// </summary>
        public Needs.Ccs.Services.Views.InvoiceNoticeItemView InvoiceNoticeItem
        {
            get
            {
                return new Needs.Ccs.Services.Views.InvoiceNoticeItemView();
            }
        }
        /// <summary>
        /// 开票明细
        /// </summary>
        public Needs.Ccs.Services.Views.InvoiceDetaiView InvoiceDetail
        {
            get
            {
                return new Needs.Ccs.Services.Views.InvoiceDetaiView();
            }
        }

        /// <summary>
        /// 开票日志
        /// </summary>
        public Needs.Ccs.Services.Views.InvoiceNoticeLogView InvoiceNoticeLog
        {
            get
            {
                return new Needs.Ccs.Services.Views.InvoiceNoticeLogView();
            }
        }

        /// <summary>
        /// 开票运单
        /// </summary>
        public Needs.Ccs.Services.Views.InvoiceWaybillView InvoiceWaybill
        {
            get
            {
                return new Needs.Ccs.Services.Views.InvoiceWaybillView();
            }
        }

        /// <summary>
        /// 金库
        /// </summary>
        public Needs.Ccs.Services.Views.FinanceVaultsView FinanceVault
        {
            get
            {
                return new Needs.Ccs.Services.Views.FinanceVaultsView();
            }
        }

        /// <summary>
        /// 金库账户
        /// </summary>
        public Needs.Ccs.Services.Views.FinanceAccountsView FinanceAccounts
        {
            get
            {
                return new Needs.Ccs.Services.Views.FinanceAccountsView();
            }
        }

        /// <summary>
        /// 换汇银行
        /// </summary>
        public Needs.Ccs.Services.Views.SwapBanksView SwapBanks
        {
            get
            {
                return new Needs.Ccs.Services.Views.SwapBanksView();
            }
        }

        /// <summary>
        /// 受限日志
        /// </summary>
        public Needs.Ccs.Services.Views.SwapLimitCountryLogsView SwapLimitCountryLogs
        {
            get
            {
                return new Needs.Ccs.Services.Views.SwapLimitCountryLogsView();
            }
        }

        /// <summary>
        /// 受限国家
        /// </summary>
        public Needs.Ccs.Services.Views.SwapLimitCountriesView SwapLimitCountries
        {
            get
            {
                return new Needs.Ccs.Services.Views.SwapLimitCountriesView();
            }
        }

        /// <summary>
        /// 账户流水
        /// </summary>
        public Needs.Ccs.Services.Views.FinanceAccountFlowsView FinanceAccountFlows
        {
            get
            {
                return new Needs.Ccs.Services.Views.FinanceAccountFlowsView();
            }
        }

        /// <summary>
        /// 换汇通知
        /// </summary>
        public Ccs.Services.Views.SwapNoticeView SwapNotice
        {
            get
            {
                return new Ccs.Services.Views.SwapNoticeView();
            }
        }

        public Ccs.Services.Views.SwapDecHeadView SwapDecHead
        {
            get
            {
                return new Ccs.Services.Views.SwapDecHeadView();
            }
        }

        public Ccs.Services.Views.UnSwapDecHeadListView UnSwapDecHeadListView
        {
            get
            {
                return new Ccs.Services.Views.UnSwapDecHeadListView();
            }
        }

        public Ccs.Services.Views.SwapNoticeItemView SwapNoticeItem
        {
            get
            {
                return new Ccs.Services.Views.SwapNoticeItemView();
            }
        }

        /// <summary>
        /// 换汇日志
        /// </summary>
        public Ccs.Services.Views.SwapNoticelogsView SwapNoticelogsView
        {
            get
            {
                return new Ccs.Services.Views.SwapNoticelogsView();
            }
        }
        /// <summary>
        /// 付款申请
        /// </summary>
        public Ccs.Services.Views.PaymentApplyView PaymentApply
        {
            get
            {
                return new Ccs.Services.Views.PaymentApplyView();
            }
        }

        /// <summary>
        /// 我的付款通知
        /// </summary>
        public Views.MyPaymentNoticesView MyPaymentNotice
        {
            get
            {
                return new Views.MyPaymentNoticesView(this.Admin);
            }
        }

        /// <summary>
        /// 付款通知
        /// </summary>
        public Ccs.Services.Views.PaymentNoticesView PaymentNotice
        {
            get
            {
                return new Ccs.Services.Views.PaymentNoticesView();
            }
        }

        /// <summary>
        /// 订单付款通知
        /// </summary>
        public Ccs.Services.Views.OrderPaymentNoticesView OrderPaymentNotice
        {
            get
            {
                return new Ccs.Services.Views.OrderPaymentNoticesView();
            }
        }

        /// <summary>
        /// 付款通知明细
        /// </summary>
        public Ccs.Services.Views.PaymentNoticeItemView PaymentNoticeItem
        {
            get
            {
                return new Ccs.Services.Views.PaymentNoticeItemView();
            }
        }

        /// <summary>
        /// 付款申请
        /// </summary>
        public Ccs.Services.Views.FinancePaymentView FinancePayment
        {
            get
            {
                return new Ccs.Services.Views.FinancePaymentView();
            }
        }

        /// <summary>
        /// 财务收款
        /// </summary>
        public Ccs.Services.Views.FinanceReceiptsView FinanceReceipts
        {
            get
            {
                return new Ccs.Services.Views.FinanceReceiptsView();
            }
        }

        /// <summary>
        /// 收款通知
        /// </summary>
        public Ccs.Services.Views.ReceiptNoticesView ReceiptNotices
        {
            get
            {
                return new Ccs.Services.Views.ReceiptNoticesView();
            }
        }

        /// <summary>
        /// 当前跟单员的收款通知
        /// </summary>
        public Views.MyReceiptNoticesView MyReceiptNotices
        {
            get
            {
                return new Views.MyReceiptNoticesView(this.Admin);
            }
        }

        /// <summary>
        /// 订单收款明细
        /// </summary>
        public Ccs.Services.Views.OrderReceiptsAllsView OrderReceipts
        {
            get
            {
                return new Ccs.Services.Views.OrderReceiptsAllsView();
            }
        }

        /// <summary>
        /// 所有状态的（不排除非200状态的）订单收款明细
        /// </summary>
        public Ccs.Services.Views.OrderReceiptsAllStatusView OrderReceiptsAllStatus
        {
            get
            {
                return new Ccs.Services.Views.OrderReceiptsAllStatusView();
            }
        }

        /// <summary>
        /// 订单实收款明细
        /// </summary>
        public Ccs.Services.Views.OrderReceivedsView OrderReceiveds
        {
            get
            {
                return new Ccs.Services.Views.OrderReceivedsView();
            }
        }

        /// <summary>
        /// 订单实收费用统计
        /// </summary>
        public Ccs.Services.Views.OrderReceivedFeesView OrderReceivedFees
        {
            get
            {
                return new Ccs.Services.Views.OrderReceivedFeesView();
            }
        }

        /// <summary>
        /// 订单费用明细
        /// </summary>
        public Ccs.Services.Views.OrderReceivedDetailsView OrderReceivedDetails
        {
            get
            {
                return new Ccs.Services.Views.OrderReceivedDetailsView();
            }
        }

        /// <summary>
        /// 提成信息
        /// </summary>
        public Ccs.Services.Views.CommissionProportionsView CommissionProportions
        {
            get
            {
                return new Ccs.Services.Views.CommissionProportionsView();
            }
        }

        /// <summary>
        /// 订单待收款统计
        /// </summary>
        public Ccs.Services.Views.OrderUnReceiveStatsView OrderUnReceiveStats
        {
            get
            {
                return new Ccs.Services.Views.OrderUnReceiveStatsView();
            }
        }
        /// <summary>
        /// 订单已收款统计
        /// </summary>
        public Ccs.Services.Views.OrderReceivedStatsView OrderReceived
        {
            get
            {
                return new Ccs.Services.Views.OrderReceivedStatsView();
            }
        }

        /// <summary>
        /// 订单已收款统计new
        /// </summary>
        public Ccs.Services.Views.OrderReceivedNewView OrderReceivedNew
        {
            get
            {
                return new Ccs.Services.Views.OrderReceivedNewView();
            }
        }

        /// <summary>
        /// 订单收款统计（不区分待收和已收）
        /// </summary>
        public Ccs.Services.Views.OrderAllStatsView OrderAllStats
        {
            get
            {
                return new Ccs.Services.Views.OrderAllStatsView();
            }
        }

        /// <summary>
        /// 利润提成
        /// </summary>
        public Ccs.Services.Views.ProfitsView Profits
        {
            get
            {
                return new Ccs.Services.Views.ProfitsView();
            }
        }

        /// <summary>
        /// 跟单利润提成
        /// </summary>
        public Ccs.Services.Views.MerchandiserProfitsView MerchandiserProfits
        {
            get
            {
                return new Ccs.Services.Views.MerchandiserProfitsView();
            }
        }
        /// <summary>
        /// 利润明细
        /// </summary>
        public Ccs.Services.Views.ProfitsDetailsView ProfitsDetails
        {
            get
            {
                return new Ccs.Services.Views.ProfitsDetailsView();
            }
        }

        #region 待收款中弹框使用

        /// <summary>
        /// 可收款的 OrderReceipt
        /// </summary>
        public Ccs.Services.Views.ReceivableOrderReceiptView ReceivableOrderReceipt
        {
            get
            {
                return new Ccs.Services.Views.ReceivableOrderReceiptView();
            }
        }

        #endregion

        /// <summary>
        /// 辅助功能-财务打印运单
        /// </summary>
        public Ccs.Services.Views.ExpressKddView ExpressKdds
        {
            get
            {
                return new Ccs.Services.Views.ExpressKddView();
            }
        }

        /// <summary>
        /// 金库账户
        /// </summary>
        public Needs.Ccs.Services.Views.FundTransferAppliesView FundTransferApplies
        {
            get
            {
                return new Needs.Ccs.Services.Views.FundTransferAppliesView();
            }
        }


        /// <summary>
        /// 当前跟单员的资金调拨
        /// </summary>
        public Views.MyFundTransferApplyView MyFundTransferApply
        {
            get
            {
                return new Views.MyFundTransferApplyView(this.Admin);
            }
        }

        /// <summary>
        /// 财务入库
        /// </summary>

        public Needs.Ccs.Services.Views.FinanceStockInView FinanceStockIn
        {
            get
            {
                return new Needs.Ccs.Services.Views.FinanceStockInView();
            }
        }

        /// <summary>
        /// 财务入库-合计数
        /// </summary>

        public Needs.Ccs.Services.Views.FinanceStockInStatisticView FinanceStockInStatistic
        {
            get
            {
                return new Needs.Ccs.Services.Views.FinanceStockInStatisticView();
            }
        }

        
    }
}
