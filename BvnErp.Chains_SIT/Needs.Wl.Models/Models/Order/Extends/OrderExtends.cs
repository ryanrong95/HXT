using Needs.Wl.Models.Views;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Needs.Wl.Models
{
    public static class OrderExtends
    {
        /// <summary>
        /// 订单的应收款
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static Views.OrderReceiptsView Receivables(this Order order)
        {
            return new Views.OrderReceiptsView(order.ID);
        }

        /// <summary>
        /// 订单的补充协议
        /// </summary>
        public static ClientAgreement Agreement(this Order order)
        {
            return new OrderAgreementView(order).FirstOrDefault();
        }

        /// <summary>
        /// 订单的补充协议
        /// 异步
        /// </summary>
        public static async Task<ClientAgreement> AgreementAsync(this Order order)
        {
            return await new OrderAgreementView(order).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 订单的附件
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static OrderFilesView Files(this Order order)
        {
            return new Views.OrderFilesView(order.ID);
        }

        /// <summary>
        /// 订单的报关单附件
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static OrderDecHeadFilesView DecHeadFiles(this Order order)
        {
            return new Views.OrderDecHeadFilesView(order.ID);
        }

        /// <summary>
        /// 订单的海关税费
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static OrderDecHeadTaxsView DecHeadTaxs(this Order order)
        {
            return new Views.OrderDecHeadTaxsView(order.ID);
        }

        /// <summary>
        /// 订单的操作日志
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static OrderLogsView Logs(this Order order)
        {
            return new OrderLogsView(order.ID);
        }

        /// <summary>
        /// 订单跟踪轨迹
        /// </summary>
        public static OrderTracesView Traces(this Order order)
        {
            return new OrderTracesView(order.ID);
        }

        /// <summary>
        /// 订单的开票信息
        /// </summary>
        public static OrderInvoicesView Invoices(this Order order)
        {
            return new OrderInvoicesView(order.ID);
        }

        /// <summary>
        /// 订单的发票运单信息
        /// </summary>
        public static OrderInvoiceWaybillsView InvoicesWaybill(this Order order)
        {
            return new OrderInvoiceWaybillsView(order.ID);
        }

        /// <summary>
        /// 订单的付汇供应商
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static OrderPayExchangeSuppliersView PayExchangeSupplier(this Order order)
        {
            return new OrderPayExchangeSuppliersView(order.ID);
        }

        /// <summary>
        /// 订单的香港交货方式
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static OrderConsignee Consignee(this Order order)
        {
            return new OrderConsigneesView(order.ID).FirstOrDefault();
        }

        /// <summary>
        /// 订单的深圳交货方式
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static OrderConsignor Consignor(this Order order)
        {
            return new OrderConsignorsView(order.ID).FirstOrDefault();
        }

        /// <summary>
        /// 订单的代理费汇率
        /// </summary>
        public static decimal AgencyFeeExchangeRate(this Order order)
        {
            decimal agencyFeeExchangeRate = 0;
            var agreement = order.Agreement();
            var exchangeRateType = agreement.AgencyFeeClause.ExchangeRateType;
            switch (exchangeRateType)
            {
                case Enums.ExchangeRateType.RealTime:
                    agencyFeeExchangeRate = order.RealExchangeRate.GetValueOrDefault();
                    break;
                case Enums.ExchangeRateType.Custom:
                    agencyFeeExchangeRate = order.CustomsExchangeRate.GetValueOrDefault();
                    break;
                case Enums.ExchangeRateType.Agreed:
                    agencyFeeExchangeRate = agreement.AgencyFeeClause.ExchangeRateValue.GetValueOrDefault(0);
                    break;
                default:
                    agencyFeeExchangeRate = 0;
                    break;
            }

            return agencyFeeExchangeRate;
        }

        /// <summary>
        /// 订单的代理费汇率
        /// </summary>
        /// <param name="order"></param>
        /// <param name="agreement">客户的补充协议</param>
        /// <returns></returns>
        public static decimal AgencyFeeExchangeRate(this Order order, ClientAgreement agreement)
        {
            decimal agencyFeeExchangeRate = 0;
            var exchangeRateType = agreement.AgencyFeeClause.ExchangeRateType;
            switch (exchangeRateType)
            {
                case Enums.ExchangeRateType.RealTime:
                    agencyFeeExchangeRate = order.RealExchangeRate.GetValueOrDefault();
                    break;
                case Enums.ExchangeRateType.Custom:
                    agencyFeeExchangeRate = order.CustomsExchangeRate.GetValueOrDefault();
                    break;
                case Enums.ExchangeRateType.Agreed:
                    agencyFeeExchangeRate = agreement.AgencyFeeClause.ExchangeRateValue.GetValueOrDefault(0);
                    break;
                default:
                    agencyFeeExchangeRate = 0;
                    break;
            }

            return agencyFeeExchangeRate;
        }

        /// <summary>
        /// 包含归类信息的订单产品明细
        /// </summary>
        public static CategoriedOrderItemsView CategoriedItems(this Order order)
        {
            return new CategoriedOrderItemsView(order.ID);
        }

        /// <summary>
        /// 订单产品明细
        /// </summary>
        public static OrderItemsView Items(this Order order)
        {
            return new OrderItemsView(order.ID);
        }

        /// <summary>
        /// 订单的费用
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static OrderPremiumsView Premiums(this Order order)
        {
            return new OrderPremiumsView(order.ID);
        }

        /// <summary>
        /// 订单的对账单
        /// TODO:完成订单的对账单设计，陆凯
        /// </summary>
        /// <param name="order"></param>
        public static void Bill(this Order order)
        {

        }

        /// <summary>
        /// 订单的管控信息
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static OrderControlsView OrderControls(this Order order)
        {
            return new OrderControlsView(order.ID);
        }

        /// <summary>
        /// 付汇申请页面中，订单本次申请金额
        /// </summary>
        /// <param name="order"></param>
        /// <param name="supplierID"></param>
        /// <returns></returns>
        public static OrderCurrentPayAmountView OrderCurrentPayAmount(this Order order, string supplierID)
        {
            return new OrderCurrentPayAmountView(order.ID, supplierID);
        }
    }
}