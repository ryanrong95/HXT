using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layer.Data.Sqls;
using Layer.Data.Sqls.ScCustoms;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 待收款统计
    /// </summary>
    public class OrderUnReceiveStatsView : UniqueView<Models.OrderReceiptStats, ScCustomsReponsitory>
    {
        public OrderUnReceiveStatsView()
        {
        }

        public OrderUnReceiveStatsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<OrderReceiptStats> GetIQueryable()
        {
            var ordersView = new OrdersView(this.Reponsitory);
            var decHeadsView = new DecHeadsView(this.Reponsitory);
            var receiptsView = new OrderReceiptsAllsView(this.Reponsitory).Where(item => item.FeeType != OrderFeeType.Product);

            var linq = from order in ordersView
                       join receipt in receiptsView on order.ID equals receipt.OrderID into receipts
                       join decHead in decHeadsView on order.ID equals decHead.OrderID into decHeads
                       where order.OrderStatus >= OrderStatus.Declared && order.OrderStatus <= OrderStatus.Completed &&
                             receipts.Any() && receipts.Sum(r => r.Amount) >= 5
                             && order.Type == OrderType.Outside
                       select new Models.OrderReceiptStats
                       {
                           ID = order.ID,
                           Client = order.Client,
                           Agreement = order.ClientAgreement,
                           DeclarePrice = order.DeclarePrice,
                           CustomsExchangeRate = order.CustomsExchangeRate.GetValueOrDefault(),
                           RealExchangeRate = order.RealExchangeRate.GetValueOrDefault(),
                           OrderStatus = order.OrderStatus,
                           DDate = decHeads.OrderBy(d => d.CreateTime).FirstOrDefault().DDate,

                           //应收款
                           Receivable = receipts.Where(r => r.Type == OrderReceiptType.Receivable).Sum(r => (decimal?)r.Amount).GetValueOrDefault(),
                           //已收款
                           Received = receipts.Where(r => r.Type == OrderReceiptType.Received).Sum(r => -(decimal?)r.Amount).GetValueOrDefault(),
                           //欠款
                           Overdraft = receipts.Sum(r => (decimal?)r.Amount).GetValueOrDefault(),
                       };

            return linq;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class OrderUnReceiveByAgreementView : UniqueView<Models.OrderReceiptStats, ScCustomsReponsitory>
    {
        public OrderUnReceiveByAgreementView()
        {
        }

        public OrderUnReceiveByAgreementView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<OrderReceiptStats> GetIQueryable()
        {
            //var ordersView = new OrdersView(this.Reponsitory);
            var decHeadsView = new DecHeadsView(this.Reponsitory);
            var receiptsView = new OrderReceiptsAllsView(this.Reponsitory).Where(item => item.FeeType != OrderFeeType.Product);

            //var linq = from order in ordersView
            //           join receipt in receiptsView on order.ID equals receipt.OrderID into receipts
            //           join decHead in decHeadsView on order.ID equals decHead.OrderID into decHeads
            //           where order.OrderStatus >= OrderStatus.Declared && order.OrderStatus <= OrderStatus.Completed &&
            //                 receipts.Any() && receipts.Sum(r => r.Amount) >= 5
            //                 && order.Type == OrderType.Outside
            //           select new Models.OrderReceiptStats
            //           {
            //               ID = order.ID,
            //               Client = order.Client,
            //               Agreement = order.ClientAgreement,
            //               DeclarePrice = order.DeclarePrice,
            //               CustomsExchangeRate = order.CustomsExchangeRate.GetValueOrDefault(),
            //               RealExchangeRate = order.RealExchangeRate.GetValueOrDefault(),
            //               OrderStatus = order.OrderStatus,
            //               DDate = decHeads.OrderBy(d => d.CreateTime).FirstOrDefault().DDate,

            //               //应收款
            //               Receivable = receipts.Where(r => r.Type == OrderReceiptType.Receivable).Sum(r => (decimal?)r.Amount).GetValueOrDefault(),
            //               //已收款
            //               Received = receipts.Where(r => r.Type == OrderReceiptType.Received).Sum(r => -(decimal?)r.Amount).GetValueOrDefault(),
            //               //欠款
            //               Overdraft = receipts.Sum(r => (decimal?)r.Amount).GetValueOrDefault(),
            //           };

            //return linq;


            var clientViews = new ClientsView(this.Reponsitory);
            var linq1 = from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                        join client in clientViews on order.ClientID equals client.ID
                        join decHead in decHeadsView on order.ID equals decHead.OrderID into decHeads
                        join agreement in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>() on order.ClientID equals agreement.ClientID
                        join settlement in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientFeeSettlements>() on agreement.ID equals settlement.AgreementID
                        join receipt in receiptsView on order.ID equals receipt.OrderID into receipts
                        where order.OrderStatus >= (int)OrderStatus.Declared && order.OrderStatus <= (int)OrderStatus.Completed &&
                             receipts.Any() && receipts.Sum(r => r.Amount) >= 5
                             && order.Type == (int)OrderType.Outside
                             && settlement.FeeType == (int)FeeType.AgencyFee
                             && agreement.Status == (int)Enums.Status.Normal
                             && settlement.Status == (int)Enums.Status.Normal
                        select new Models.OrderReceiptStats
                        {
                            ID = order.ID,
                            Client = client,
                            //Agreement = Agreement,
                            DeclarePrice = order.DeclarePrice,
                            CustomsExchangeRate = order.CustomsExchangeRate.GetValueOrDefault(),
                            RealExchangeRate = order.RealExchangeRate.GetValueOrDefault(),
                            OrderStatus = (Enums.OrderStatus)order.OrderStatus,
                            DDate = decHeads.OrderBy(d => d.CreateTime).FirstOrDefault().DDate,

                            //应收款
                            Receivable = receipts.Where(r => r.Type == OrderReceiptType.Receivable).Sum(r => (decimal?)r.Amount).GetValueOrDefault(),
                            //已收款
                            Received = receipts.Where(r => r.Type == OrderReceiptType.Received).Sum(r => -(decimal?)r.Amount).GetValueOrDefault(),
                            //欠款
                            Overdraft = receipts.Sum(r => (decimal?)r.Amount).GetValueOrDefault(),
                            PeriodType = settlement.PeriodType.ToString(),
                            MonthlyDay = settlement.MonthlyDay.ToString(),
                            DaysLimit = settlement.DaysLimit.ToString(),
                        };
            return linq1;
        }
    }

    /// <summary>
    /// 已收款统计
    /// </summary>
    public class OrderReceivedStatsView : UniqueView<Models.OrderReceiptStats, ScCustomsReponsitory>
    {
        public OrderReceivedStatsView()
        {
        }

        public OrderReceivedStatsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<OrderReceiptStats> GetIQueryable()
        {
            var ordersView = new OrdersView(this.Reponsitory);
            var decHeadsView = new DecHeadsView(this.Reponsitory);
            //订单收款
            var receiptsView = new OrderReceiptsAllsView(this.Reponsitory).Where(item => item.FeeType != OrderFeeType.Product);
            //香港实收现金费用
            var hkFeesView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderWhesPremiums>().Where(item => item.PaymentType == (int)Enums.WhsePaymentType.Cash &&
                                                                                                                     item.PremiumsStatus == (int)Enums.WarehousePremiumsStatus.Payed);
            //订单实付杂费的金额、汇率
            var paymentItemsView = from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PaymentNoticeItems>()
                                   join payment in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PaymentNotices>() on item.PaymentNoticeID equals payment.ID
                                   where item.FeeType == (int)Enums.FeeType.Incidental && item.Status == (int)Enums.PaymentNoticeStatus.Paid
                                   select new
                                   {
                                       item.ID,
                                       item.OrderID,
                                       item.Amount,
                                       payment.ExchangeRate
                                   };

            var result = from order in ordersView
                         join receipt in receiptsView on order.ID equals receipt.OrderID into receipts
                         join paymentItem in paymentItemsView on order.ID equals paymentItem.OrderID into paymentItems
                         join hkFee in hkFeesView on order.ID equals hkFee.OrderID into hkFees
                         join decHead in decHeadsView on order.ID equals decHead.OrderID into decHeads
                         where order.OrderStatus >= OrderStatus.Declared && order.OrderStatus <= OrderStatus.Completed &&
                               receipts.Any() && receipts.Sum(r => r.Amount) < 5
                         select new Models.OrderReceiptStats
                         {
                             ID = order.ID,
                             Client = order.Client,
                             Agreement = order.ClientAgreement,
                             DeclarePrice = order.DeclarePrice,
                             CustomsExchangeRate = order.CustomsExchangeRate.GetValueOrDefault(),
                             RealExchangeRate = order.RealExchangeRate.GetValueOrDefault(),
                             OrderStatus = order.OrderStatus,
                             DDate = decHeads.OrderBy(d => d.CreateTime).FirstOrDefault().DDate,

                             //已收款
                             Received = receipts.Where(r => r.Type == OrderReceiptType.Received).Sum(r => -(decimal?)r.Amount).GetValueOrDefault(),
                             //实收
                             TariffReceived = receipts.Where(r => r.FeeType == Enums.OrderFeeType.Tariff && r.Type == OrderReceiptType.Received).Sum(r => -(decimal?)r.Amount).GetValueOrDefault(),
                             AVTReceived = receipts.Where(r => r.FeeType == Enums.OrderFeeType.AddedValueTax && r.Type == OrderReceiptType.Received).Sum(r => -(decimal?)r.Amount).GetValueOrDefault(),
                             AgencyReceived = receipts.Where(r => r.FeeType == Enums.OrderFeeType.AgencyFee && r.Type == OrderReceiptType.Received).Sum(r => -(decimal?)r.Amount).GetValueOrDefault(),
                             IncidentalReceived = receipts.Where(r => r.FeeType == Enums.OrderFeeType.Incidental && r.Type == OrderReceiptType.Received).Sum(r => -(decimal?)r.Amount).GetValueOrDefault(),
                             HKFeeReceived = hkFees.Sum(fee => (decimal?)(fee.Count * fee.UnitPrice * fee.ExchangeRate)).GetValueOrDefault(),
                             //实付
                             TariffReceivable = receipts.Where(r => r.FeeType == Enums.OrderFeeType.Tariff && r.Type == OrderReceiptType.Receivable).Sum(r => (decimal?)r.Amount).GetValueOrDefault(),
                             AVTReceivable = receipts.Where(r => r.FeeType == Enums.OrderFeeType.AddedValueTax && r.Type == OrderReceiptType.Receivable).Sum(r => (decimal?)r.Amount).GetValueOrDefault(),
                             IncidentalPaid = paymentItems.Sum(pay => (decimal?)(pay.Amount * pay.ExchangeRate)).GetValueOrDefault()
                         };

            return result;
        }
    }


    /// <summary>
    /// 展示已收款列表
    /// </summary>
    public class OrderReceivedNewView : UniqueView<Models.OrderReceiptInfo, ScCustomsReponsitory>
    {
        public OrderReceivedNewView()
        {
        }

        public OrderReceivedNewView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<OrderReceiptInfo> GetIQueryable()
        {
            var clientViews = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var orderViews = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>().Where(t=>t.Type == (int)Enums.OrderType.Outside);
            var decheadViews = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
            var orderReceiptViews = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>()
                .Where(t => t.FeeType != (int)OrderFeeType.Product && t.Status == (int)Enums.Status.Normal);
            var clientAdminView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAdmins>();
            var adminsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>();

            //订单实付杂费的金额、汇率
            //var paymentItemsView = from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PaymentNoticeItems>()
            //                       join payment in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PaymentNotices>() on item.PaymentNoticeID equals payment.ID
            //                       where item.FeeType == (int)Enums.FeeType.Incidental && item.Status == (int)Enums.PaymentNoticeStatus.Paid
            //                       select new
            //                       {
            //                           item.ID,
            //                           item.OrderID,
            //                           item.Amount,
            //                           payment.ExchangeRate
            //                       };

            var shoukuanView = from receipt in orderReceiptViews
                               group receipt by receipt.OrderID into rec
                               select new
                               {
                                   OrderID = rec.Key,
                                   Receipt = rec.Where(t => t.Type == (int)OrderReceiptType.Receivable).Sum(t => t.Amount),
                                   Receipted = rec.Where(t => t.Type == (int)OrderReceiptType.Received).Sum(t => -t.Amount),
                                   daili = rec.Where(t => t.Type == (int)OrderReceiptType.Received &&
                                   (t.FeeType == (int)OrderFeeType.AgencyFee || t.FeeType == (int)OrderFeeType.Incidental)).Sum(t => -t.Amount),
                               };

            var linq = from order in orderViews
                       join dechead in decheadViews on order.ID equals dechead.OrderID
                       join client in clientViews on order.ClientID equals client.ID
                       join admin in clientAdminView on client.ID equals admin.ClientID
                       join adminview in adminsView on admin.AdminID equals adminview.ID
                       join shoukuan in shoukuanView on order.ID equals shoukuan.OrderID
                       //join paymentItem in paymentItemsView on order.ID equals paymentItem.OrderID into paymentItems
                       where dechead.IsSuccess == true
                       && admin.Type == (int)Enums.ClientAdminType.ServiceManager
                       && (shoukuan.Receipted - shoukuan.Receipt) > -5
                       select new OrderReceiptInfo
                       {
                           ID = order.ID,
                           ClientCode = client.ClientCode,
                           CompanyName = dechead.OwnerName,
                           OrderStatus = (Enums.OrderStatus)order.OrderStatus,
                           DeclareDate = dechead.DDate,
                           DeclarePrice = (order.DeclarePrice * order.RealExchangeRate).Value.ToRound(2),
                           Received = shoukuan.Receipted,
                           //Profit = shoukuan.daili - paymentItems.Sum(pay => (decimal?)(pay.Amount * pay.ExchangeRate)).GetValueOrDefault(),
                           Salesman = adminview.RealName
                       };


            return linq;
        }
    }

    /// <summary>
    /// 订单收款统计（不区分待收和已收）
    /// </summary>

    public class OrderAllStatsView : UniqueView<Models.OrderReceiptStats, ScCustomsReponsitory>
    {
        public OrderAllStatsView()
        {
        }

        public OrderAllStatsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<OrderReceiptStats> GetIQueryable()
        {
            var ordersView = new OrdersView(this.Reponsitory);
            var decHeadsView = new DecHeadsView(this.Reponsitory);
            //订单收款
            var receiptsView = new OrderReceiptsAllsView(this.Reponsitory);
            //香港实收现金费用
            var hkFeesView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderWhesPremiums>().Where(item => item.PaymentType == (int)Enums.WhsePaymentType.Cash &&
                                                                                                                     item.PremiumsStatus == (int)Enums.WarehousePremiumsStatus.Payed);
            //订单实付 金额、汇率、类型 、付款日期
            var paymentItemsView = from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PaymentNoticeItems>()
                                   join payment in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PaymentNotices>() on item.PaymentNoticeID equals payment.ID
                                   where item.Status == (int)Enums.PaymentNoticeStatus.Paid
                                   select new
                                   {
                                       item.ID,
                                       item.OrderID,
                                       item.Amount,
                                       payment.ExchangeRate,
                                       item.FeeType,
                                       payment.PayDate
                                   };
            //付款 关税、增值税 付款日期
            //var decTaxItemsView = from tax in this.Reponsitory.ReadTable<DecTaxFlows>()
            //    join decHead in decHeadsView on tax.DecTaxID equals decHead.ID
            //    select new
            //    {
            //        tax.TaxType,
            //        tax.PayDate,
            //        decHead.OrderID,
            //        decHead.DDate
            //    };

            var result = from order in ordersView
                         join receipt in receiptsView on order.ID equals receipt.OrderID into receipts
                         join paymentItem in paymentItemsView on order.ID equals paymentItem.OrderID into paymentItems
                         //  join tax in decTaxItemsView on order.ID equals tax.OrderID into taxItems
                         join decHead in decHeadsView on order.ID equals decHead.OrderID into decHeads
                         join hkFee in hkFeesView on order.ID equals hkFee.OrderID into hkFees

                         where order.OrderStatus >= OrderStatus.Declared && order.OrderStatus <= OrderStatus.Completed &&
                               receipts.Any()
                         select new Models.OrderReceiptStats
                         {
                             ID = order.ID,
                             CreateDate = order.CreateDate,
                             Agreement = order.ClientAgreement,
                             //应收
                             Product = receipts.Where(r => r.Type == OrderReceiptType.Receivable && r.FeeType == OrderFeeType.Product).Sum(r => (decimal?)r.Amount).GetValueOrDefault(),
                             TariffReceivable = receipts.Where(r => r.FeeType == Enums.OrderFeeType.Tariff && r.Type == OrderReceiptType.Receivable).Sum(r => (decimal?)r.Amount).GetValueOrDefault(),
                             AVTReceivable = receipts.Where(r => r.FeeType == Enums.OrderFeeType.AddedValueTax && r.Type == OrderReceiptType.Receivable).Sum(r => (decimal?)r.Amount).GetValueOrDefault(),
                             AgencyReceivable = receipts.Where(r => r.Type == OrderReceiptType.Receivable && r.FeeType == OrderFeeType.AgencyFee).Sum(r => (decimal?)r.Amount).GetValueOrDefault(),
                             IncidentalReceivable = receipts.Where(r => r.Type == OrderReceiptType.Receivable && r.FeeType == OrderFeeType.Incidental).Sum(r => (decimal?)r.Amount).GetValueOrDefault(),

                             //实收
                             ProductReceived = receipts.Where(r => r.FeeType == Enums.OrderFeeType.Product && r.Type == OrderReceiptType.Received).Sum(r => -(decimal?)r.Amount).GetValueOrDefault(),
                             TariffReceived = receipts.Where(r => r.FeeType == Enums.OrderFeeType.Tariff && r.Type == OrderReceiptType.Received).Sum(r => -(decimal?)r.Amount).GetValueOrDefault(),
                             AVTReceived = receipts.Where(r => r.FeeType == Enums.OrderFeeType.AddedValueTax && r.Type == OrderReceiptType.Received).Sum(r => -(decimal?)r.Amount).GetValueOrDefault(),
                             AgencyReceived = receipts.Where(r => r.FeeType == Enums.OrderFeeType.AgencyFee && r.Type == OrderReceiptType.Received).Sum(r => -(decimal?)r.Amount).GetValueOrDefault(),
                             IncidentalReceived = receipts.Where(r => r.FeeType == Enums.OrderFeeType.Incidental && r.Type == OrderReceiptType.Received).Sum(r => -(decimal?)r.Amount).GetValueOrDefault(),
                             HKFeeReceived = hkFees.Sum(fee => (decimal?)(fee.Count * fee.UnitPrice * fee.ExchangeRate)).GetValueOrDefault(),

                             //实付
                             //关税 增值税==订单的关税和增值税   货款 外币*汇率
                             ProductPaid = paymentItems.Where(pay => pay.FeeType == (int)FeeType.Product).Sum(pay => (decimal?)(pay.Amount * pay.ExchangeRate)).GetValueOrDefault(),
                             IncidentalPaid = paymentItems.Where(pay => pay.FeeType == (int)Enums.FeeType.Incidental).Sum(pay => (decimal?)(pay.Amount * pay.ExchangeRate)).GetValueOrDefault(),

                             //付款日期///////////////////////////////////////
                             ProductPayDate = paymentItems.Where(pay => pay.FeeType == (int)FeeType.Product).Select(pay => pay.PayDate).FirstOrDefault(),
                             IncidentalPayDate = paymentItems.Where(pay => pay.FeeType == (int)FeeType.Incidental).Select(pay => pay.PayDate).FirstOrDefault(),

                             //关税 增值税付款日期  报关日期
                             //TariffPayDate = taxItems.FirstOrDefault(pay => pay.TaxType==(int)DecTaxType.Tariff).PayDate,
                             //AVTPayDate = taxItems.FirstOrDefault(pay => pay.TaxType == (int)DecTaxType.AddedValueTax).PayDate,
                             TariffPayDate = decHeads.Select(d => d.DDate).FirstOrDefault(),
                             AVTPayDate = decHeads.Select(d => d.DDate).FirstOrDefault(),
                             DDate = decHeads.Select(d => d.DDate).FirstOrDefault()
                         };

            return result;
        }
    }
}
