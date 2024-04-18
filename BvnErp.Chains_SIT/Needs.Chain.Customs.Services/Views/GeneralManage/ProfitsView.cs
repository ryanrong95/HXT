using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 业务员利润提成的视图
    /// </summary>
    public class ProfitsView : UniqueView<Models.Profit, ScCustomsReponsitory>
    {
        public ProfitsView()
        {
        }

        protected override IQueryable<Profit> GetIQueryable()
        {
            var saleMansView = new AdminRoleViews(this.Reponsitory).Where(item => item.Role.Name == "业务员")
                                                                   .Select(item => new { item.Admin.ID, item.Admin.RealName }).Distinct();
            var detailsView = new ProfitsDetailsView(this.Reponsitory);
            var linq = from entity in saleMansView
                       join detail in detailsView on entity.ID equals detail.Client.ServiceManager.ID into profitDetails
                       select new Models.Profit
                       {
                           ID = entity.ID,
                           Name = entity.RealName,
                           ProfitDetails = profitDetails
                       };

            return linq;
        }
    }
    /// <summary>
    /// 跟单员利润提成的视图
    /// </summary>
    public class MerchandiserProfitsView : UniqueView<Models.Profit, ScCustomsReponsitory>
    {
        public MerchandiserProfitsView()
        {
        }

        protected override IQueryable<Profit> GetIQueryable()
        {
            var merchandiserView = new AdminRoleViews(this.Reponsitory).Where(item => item.Role.Name == "跟单员")
                                                                   .Select(item => new { item.Admin.ID, item.Admin.RealName }).Distinct();
            var detailsView = new ProfitsDetailsView(this.Reponsitory);
            var linq = from entity in merchandiserView
                       join detail in detailsView on entity.ID equals detail.Client.ServiceManager.ID into profitDetails
                       select new Models.Profit
                       {
                           ID = entity.ID,
                           Name = entity.RealName,
                           ProfitDetails = profitDetails
                       };

            return linq;
        }
    }
    /// <summary>
    /// 订单利润提成明细的视图
    /// </summary>
    public class ProfitsDetailsView : UniqueView<Models.ProfitDetail, ScCustomsReponsitory>
    {
        public ProfitsDetailsView()
        {

        }

        internal ProfitsDetailsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<ProfitDetail> GetIQueryable()
        {
            var ordersView = new OrdersView(this.Reponsitory).Where(t => t.Type == OrderType.Outside);
            var decHeadsView = new DecHeadsView(this.Reponsitory);
            var financeReceiptsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceReceipts>();
            var receiptsView = from receipt in new OrderReceiptsAllsView(this.Reponsitory)
                               join financeReceipt in financeReceiptsView on receipt.FinanceReceiptID equals financeReceipt.ID into financeReceipts
                               from financeReceipt in financeReceipts.DefaultIfEmpty()
                               where receipt.FeeType != OrderFeeType.Product
                               select new
                               {
                                   ID = receipt.ID,
                                   FinanceReceiptID = receipt.FinanceReceiptID,
                                   ClientID = receipt.ClientID,
                                   OrderID = receipt.OrderID,
                                   FeeSourceID = receipt.FeeSourceID,
                                   FeeType = receipt.FeeType,
                                   Type = receipt.Type,
                                   Amount = receipt.Amount,
                                   CreateDate = receipt.CreateDate,
                                   ReceiptDate = financeReceipt.ReceiptDate
                               };

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
            var hkFeesView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderWhesPremiums>().Where(item => item.PaymentType == (int)Enums.WhsePaymentType.Cash &&
                                                                                                                item.PremiumsStatus == (int)Enums.WarehousePremiumsStatus.Payed);
            var commissionView = new CommissionProportionsView(this.Reponsitory).OrderBy(x => x.RegeisterMonth);
            //开票信息
            var invoiceItemsView = from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>()
                                   join invoice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNotices>() on item.InvoiceNoticeID equals invoice.ID
                                   select new
                                   {
                                       item.ID,
                                       item.OrderID,
                                       invoice.InvoiceType,
                                       invoice.InvoiceTaxRate,
                                       invoice.UpdateDate
                                   };


            var linq = from order in ordersView
                       join receipt in receiptsView on order.ID equals receipt.OrderID into receipts
                       join paymentItem in paymentItemsView on order.ID equals paymentItem.OrderID into paymentItems
                       join hkFee in hkFeesView on order.ID equals hkFee.OrderID into hkFees
                       join decHead in decHeadsView on order.ID equals decHead.OrderID into decHeads
                       let regeisterMonth = (DateTime.Now.Year - order.Client.CreateDate.Year) * 12 + (DateTime.Now.Month - order.Client.CreateDate.Month)
                       let invoiceItem = (from item in invoiceItemsView
                                          where item.OrderID.IndexOf(order.ID) > -1// && order.InvoiceStatus == InvoiceStatus.Invoiced
                                          select item).FirstOrDefault()
                       where order.OrderStatus >= OrderStatus.Declared && order.OrderStatus <= OrderStatus.Completed &&
                             receipts.Any() && receipts.Sum(r => r.Amount) > -5
                       select new Models.ProfitDetail
                       {
                           ID = order.ID,
                           Client = order.Client,
                           Agreement = order.ClientAgreement,
                           DeclarePrice = order.DeclarePrice,
                           Currency = order.Currency,
                           CustomsExchangeRate = order.CustomsExchangeRate.GetValueOrDefault(),
                           RealExchangeRate = order.RealExchangeRate.GetValueOrDefault(),
                           OrderDate = order.CreateDate,
                           ReceiveDate = receipts.OrderByDescending(r => r.CreateDate).FirstOrDefault(r => r.Type == OrderReceiptType.Received && r.FeeType == OrderFeeType.AgencyFee) == null ?
                           (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue
                           : receipts.OrderByDescending(r => r.CreateDate).FirstOrDefault(r => r.Type == OrderReceiptType.Received && r.FeeType == OrderFeeType.AgencyFee).ReceiptDate,
                           DDate = decHeads.OrderBy(d => d.CreateTime).FirstOrDefault().DDate.Value,
                           proportion = commissionView.FirstOrDefault(c => c.RegeisterMonth >= regeisterMonth).Proportion,

                           //实收
                           TariffReceived = receipts.Where(r => r.FeeType == Enums.OrderFeeType.Tariff && r.Type == OrderReceiptType.Received).Sum(r => -(decimal?)r.Amount).GetValueOrDefault(),
                           AVTReceived = receipts.Where(r => r.FeeType == Enums.OrderFeeType.AddedValueTax && r.Type == OrderReceiptType.Received).Sum(r => -(decimal?)r.Amount).GetValueOrDefault(),
                           AgencyReceived = receipts.Where(r => r.FeeType == Enums.OrderFeeType.AgencyFee && r.Type == OrderReceiptType.Received).Sum(r => -(decimal?)r.Amount).GetValueOrDefault(),
                           IncidentalReceived = receipts.Where(r => r.FeeType == Enums.OrderFeeType.Incidental && r.Type == OrderReceiptType.Received).Sum(r => -(decimal?)r.Amount).GetValueOrDefault(),
                           HKFeeReceived = hkFees.Sum(fee => (decimal?)(fee.Count * fee.UnitPrice * fee.ExchangeRate)).GetValueOrDefault(),

                           //实付
                           TariffReceivable = receipts.Where(r => r.FeeType == Enums.OrderFeeType.Tariff && r.Type == OrderReceiptType.Receivable).Sum(r => (decimal?)r.Amount).GetValueOrDefault(),
                           AVTReceivable = receipts.Where(r => r.FeeType == Enums.OrderFeeType.AddedValueTax && r.Type == OrderReceiptType.Receivable).Sum(r => (decimal?)r.Amount).GetValueOrDefault(),
                           IncidentalPaid = paymentItems.Sum(pay => (decimal?)(pay.Amount * pay.ExchangeRate)).GetValueOrDefault(),

                           //开票信息
                           InvoiceType = invoiceItem == null ? null : (Enums.InvoiceType?)invoiceItem.InvoiceType,
                           InvoiceTaxRate = invoiceItem == null ? (decimal?)null : invoiceItem.InvoiceTaxRate,
                           InvoiceDate = invoiceItem == null ? (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue : invoiceItem.UpdateDate
                       };

            return linq;
        }
    }


    /// <summary>
    /// 代理订单1
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ProfitsExportDetailsView : Needs.Linq.Generic.Query1Classics<Models.ProfitDetail, ScCustomsReponsitory>
    {
        public ProfitsExportDetailsView()
        {
        }

        internal ProfitsExportDetailsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ProfitDetail> GetIQueryable(Expression<Func<Models.ProfitDetail, bool>> expression, params LambdaExpression[] expressions)
        {
            var ordersView = new OrdersView(this.Reponsitory).Where(t => t.Type == OrderType.Outside);
            var XDTStaffsTopView = new Needs.Ccs.Services.Views.XDTStaffsTopView(this.Reponsitory);
            var clientAdminsView = new ClientAdminsView(this.Reponsitory);
            var separtmentView = new DepartmentView(this.Reponsitory);

            var linq = from order in ordersView
                       join dechead in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>() on order.ID equals dechead.OrderID

                       join saleAdmin in clientAdminsView.Where(t => t.Type == Enums.ClientAdminType.ServiceManager && t.Status == Enums.Status.Normal)
                            on order.Client.ID equals saleAdmin.ClientID into saleAdmins
                       from sale in saleAdmins.DefaultIfEmpty()
                           //join staff in XDTStaffsTopView on sale.Admin.ID equals staff.OriginID into XDTStaffsTopView2
                           //from staff in XDTStaffsTopView2.DefaultIfEmpty()
                       join depart in separtmentView on sale.Admin.DepartmentID equals depart.ID into depart_temp
                       from depart in depart_temp.DefaultIfEmpty()

                       where dechead.IsSuccess == true
                       select new Models.ProfitDetail
                       {
                           ID = order.ID,
                           Client = order.Client,
                           DDate = dechead.DDate.Value,
                           CustomsExchangeRate = order.CustomsExchangeRate.GetValueOrDefault(),
                           RealExchangeRate = order.RealExchangeRate.GetValueOrDefault(),
                           OrderDate = order.CreateDate,
                           Agreement = order.ClientAgreement,
                           Currency = order.Currency,
                           DeclarePrice = order.DeclarePrice,
                           //DepartmentCode = staff != null ? staff.DepartmentCode : null,
                           DepartmentCode = depart != null ? depart.Name : null,
                       };

            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<Models.ProfitDetail, bool>>);
            }

            return linq.Where(expression);
        }

        protected override IEnumerable<Models.ProfitDetail> OnReadShips(Models.ProfitDetail[] results)
        {
            var orderIds = results.Select(o => o.ID).Distinct().ToArray();

            var orderReceipts = new OrderReceiptsAllsView(this.Reponsitory).Where(t => orderIds.Contains(t.OrderID)).ToArray();

            var receiptsView = (from receipt in orderReceipts
                                join financeReceipt in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceReceipts>() on receipt.FinanceReceiptID equals financeReceipt.ID into financeReceipts
                                from financeReceipt in financeReceipts.DefaultIfEmpty()
                                where receipt.FeeType != OrderFeeType.Product
                                select new
                                {
                                    ID = receipt.ID,
                                    FinanceReceiptID = receipt.FinanceReceiptID,
                                    ClientID = receipt.ClientID,
                                    OrderID = receipt.OrderID,
                                    FeeSourceID = receipt.FeeSourceID,
                                    FeeType = receipt.FeeType,
                                    Type = receipt.Type,
                                    Amount = receipt.Amount,
                                    CreateDate = receipt.CreateDate,
                                    ReceiptDate = financeReceipt?.ReceiptDate
                                }).ToArray();

            //订单实付杂费的金额、汇率
            var paymentItemsView = (from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PaymentNoticeItems>()
                                    join payment in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PaymentNotices>() on item.PaymentNoticeID equals payment.ID
                                    where item.FeeType == (int)Enums.FeeType.Incidental && item.Status == (int)Enums.PaymentNoticeStatus.Paid
                                    && orderIds.Contains(item.OrderID)
                                    select new
                                    {
                                        item.ID,
                                        item.OrderID,
                                        item.Amount,
                                        payment.ExchangeRate
                                    }).ToArray();

            var hkFeesView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderWhesPremiums>().Where(item => item.PaymentType == (int)Enums.WhsePaymentType.Cash &&
                                                                                                                item.PremiumsStatus == (int)Enums.WarehousePremiumsStatus.Payed);
            var commissionView = new CommissionProportionsView(this.Reponsitory).OrderBy(x => x.RegeisterMonth);
            //开票信息
            var invoiceItemsView = (from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>()
                                    join invoice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNotices>() on item.InvoiceNoticeID equals invoice.ID
                                    select new
                                    {
                                        item.ID,
                                        item.OrderID,
                                        invoice.InvoiceType,
                                        invoice.InvoiceTaxRate,
                                        invoice.UpdateDate
                                    }).ToArray();

            //var invoices = new List<ProfitInvoiceInfo>();
            //foreach (var id in orderIds)
            //{
            //    var inv = invoiceItemsView.Where(t => t.OrderID.Contains(id)).FirstOrDefault();
            //    if (inv != null)
            //    {
            //        invoices.Add(new ProfitInvoiceInfo
            //        {
            //            OrderID = id,
            //            InvoiceType = (Enums.InvoiceType)inv.InvoiceType,
            //            InvoiceTaxRate = inv.InvoiceTaxRate,
            //            InvoiceDate = inv.UpdateDate
            //        });
            //    }
            //}


            var invoices = new List<ProfitInvoiceInfo>();
            var feeClause = new List<ProfitFeeClause>();
            foreach (var item in results)
            {
                //发票信息
                var inv = invoiceItemsView.Where(t => t.OrderID.Contains(item.ID)).FirstOrDefault();
                if (inv != null)
                {
                    invoices.Add(new ProfitInvoiceInfo
                    {
                        OrderID = item.ID,
                        InvoiceType = (Enums.InvoiceType)inv.InvoiceType,
                        InvoiceTaxRate = inv.InvoiceTaxRate,
                        InvoiceDate = inv.UpdateDate
                    });
                }

                //账期 20211115 by ryan 宋师傅：导出来的提成表要显示账期，使用税费的结算日
                var clauesdate = new DateTime();
                var ddate = item.DDate;
                switch (item.Agreement.TaxFeeClause.PeriodType)
                {
                    case Enums.PeriodType.PrePaid:
                        //预付款
                        clauesdate = ddate;
                        break;
                    case Enums.PeriodType.AgreedPeriod:
                        //约定期限
                        clauesdate = ddate.AddDays(item.Agreement.TaxFeeClause.DaysLimit.Value);
                        break;
                    case Enums.PeriodType.Monthly:
                        //月结
                        var monthlyDay = item.Agreement.TaxFeeClause.MonthlyDay.Value;
                        //当前月的天数
                        var days = DateTime.DaysInMonth(ddate.AddMonths(1).Year, ddate.AddMonths(1).Month);
                        if (days < monthlyDay)
                        {
                            monthlyDay = days;
                        }
                        clauesdate = new DateTime(ddate.AddMonths(1).Year, ddate.AddMonths(1).Month, monthlyDay);
                        break;
                    default:
                        //
                        clauesdate = ddate;
                        break;
                }
                feeClause.Add(new ProfitFeeClause
                {
                    OrderID = item.ID,
                    PeriodDate = clauesdate
                });
            }

            var linq = from order in results
                       join receipt in receiptsView on order.ID equals receipt.OrderID into receipts
                       join paymentItem in paymentItemsView on order.ID equals paymentItem.OrderID into paymentItems
                       join hkFee in hkFeesView on order.ID equals hkFee.OrderID into hkFees
                       join invoice in invoices on order.ID equals invoice.OrderID into invs
                       from invoice in invs.DefaultIfEmpty()
                       join clause in feeClause on order.ID equals clause.OrderID into t_clause
                       from clause in t_clause.DefaultIfEmpty()
                           //join decHead in decHeadsView on order.ID equals decHead.OrderID into decHeads
                       let regeisterMonth = (DateTime.Now.Year - order.Client.CreateDate.Year) * 12 + (DateTime.Now.Month - order.Client.CreateDate.Month)
                       //let invoiceItem = (from item in invoiceItemsView
                       //                   where item.OrderID.IndexOf(order.ID) > -1// && order.InvoiceStatus == InvoiceStatus.Invoiced
                       //                   select item).FirstOrDefault()
                       where receipts.Any() && receipts.Sum(r => r.Amount) <= 5//允许客户少付5块钱
                       select new Models.ProfitDetail
                       {
                           ID = order.ID,
                           Client = order.Client,
                           Agreement = order.Agreement,
                           DeclarePrice = order.DeclarePrice,
                           Currency = order.Currency,
                           CustomsExchangeRate = order.CustomsExchangeRate,
                           RealExchangeRate = order.RealExchangeRate,
                           OrderDate = order.OrderDate,
                           ReceiveDate = receipts.OrderByDescending(r => r.CreateDate).FirstOrDefault(r => r.Type == OrderReceiptType.Received && r.FeeType == OrderFeeType.AgencyFee) == null ?
                           (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue
                           : receipts.OrderByDescending(r => r.CreateDate).FirstOrDefault(r => r.Type == OrderReceiptType.Received && r.FeeType == OrderFeeType.AgencyFee).ReceiptDate.Value,
                           DDate = order.DDate,
                           proportion = commissionView.FirstOrDefault(c => c.RegeisterMonth >= regeisterMonth).Proportion,

                           DepartmentCode = order.DepartmentCode,
                           FeeClauseDate = clause.PeriodDate,
                           //FeeClauseType = order.Agreement.TaxFeeClause.PeriodType,

                           //实收
                           TariffReceived = receipts.Where(r => r.FeeType == Enums.OrderFeeType.Tariff && r.Type == OrderReceiptType.Received).Sum(r => -(decimal?)r.Amount).GetValueOrDefault(),
                           AVTReceived = receipts.Where(r => r.FeeType == Enums.OrderFeeType.AddedValueTax && r.Type == OrderReceiptType.Received).Sum(r => -(decimal?)r.Amount).GetValueOrDefault(),
                           AgencyReceived = receipts.Where(r => r.FeeType == Enums.OrderFeeType.AgencyFee && r.Type == OrderReceiptType.Received).Sum(r => -(decimal?)r.Amount).GetValueOrDefault(),
                           IncidentalReceived = receipts.Where(r => r.FeeType == Enums.OrderFeeType.Incidental && r.Type == OrderReceiptType.Received).Sum(r => -(decimal?)r.Amount).GetValueOrDefault(),
                           HKFeeReceived = hkFees.Sum(fee => (decimal?)(fee.Count * fee.UnitPrice * fee.ExchangeRate)).GetValueOrDefault(),

                           //实付
                           TariffReceivable = receipts.Where(r => r.FeeType == Enums.OrderFeeType.Tariff && r.Type == OrderReceiptType.Receivable).Sum(r => (decimal?)r.Amount).GetValueOrDefault(),
                           AVTReceivable = receipts.Where(r => r.FeeType == Enums.OrderFeeType.AddedValueTax && r.Type == OrderReceiptType.Receivable).Sum(r => (decimal?)r.Amount).GetValueOrDefault(),
                           IncidentalPaid = paymentItems.Sum(pay => (decimal?)(pay.Amount * pay.ExchangeRate)).GetValueOrDefault(),

                           //开票信息
                           ProfitInvoiceInfo = invoice
                           //InvoiceType = invoice == null ? null : (Enums.InvoiceType?)invoice.InvoiceType,
                           //InvoiceTaxRate = invoice == null ? (decimal?)null : invoice.InvoiceTaxRate,
                           //InvoiceDate = invoice == null ? (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue : invoice.UpdateDate
                       };

            return linq;
        }
    }
}
