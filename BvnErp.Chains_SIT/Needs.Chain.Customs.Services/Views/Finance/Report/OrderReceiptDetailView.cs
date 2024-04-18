using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// OrderReceipts 行转列
    /// </summary>
    public class OrderReceiptDetailView : UniqueView<Models.OrderReceiveDetail, ScCustomsReponsitory>
    {
        public OrderReceiptDetailView()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        public OrderReceiptDetailView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.OrderReceiveDetail> GetIQueryable()
        {
            var clientsView = new ClientsView(this.Reponsitory);
            var orderReceiptsC2RView = new OrderReceiptsC2RView(this.Reponsitory);
            var paymentExchangeRateView = new PaymentExchangeRateView(this.Reponsitory);
            var clientAgreements = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>();
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();

            var result = from receiptNotice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ReceiptNotices>()
                         join financeReceipts in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceReceipts>() on receiptNotice.ID equals financeReceipts.ID
                         join orderFee in orderReceiptsC2RView on receiptNotice.ID equals orderFee.FinanceReceiptID into orderFees
                         from orderFee in orderFees.DefaultIfEmpty()
                         join client in clientsView on receiptNotice.ClientID equals client.ID into clients
                         from client in clients.DefaultIfEmpty()
                         where receiptNotice.Status == (int)Enums.Status.Normal
                         select new Models.OrderReceiveDetail
                         {
                             ReceiveDate = receiptNotice.CreateDate,
                             Client = client,
                             ReceiveAmount = financeReceipts.Amount,
                             ClearAmount = receiptNotice.ClearAmount,
                             OrderID = orderFee.OrderID,
                             FinanceReceiptID = receiptNotice.ID,
                             Tariff = orderFee.Tariff,
                             ExciseTax = orderFee.ExciseTax,
                             AddedValueTax = orderFee.AddedValueTax,
                             Incidental = orderFee.Incidental,
                             GoodsAmount = orderFee.GoodsAmount,
                             AgencyFee = orderFee.AgencyFee,
                         };


            var finalResult = from c in result
                              join dechead in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>() on c.OrderID equals dechead.OrderID
                              into decheads
                              from dechead in decheads.DefaultIfEmpty()
                              join paymentExchangeRate in paymentExchangeRateView on c.OrderID equals paymentExchangeRate.OrderID into paymentExchangeRates
                              from paymentExchangeRate in paymentExchangeRates.DefaultIfEmpty()

                              join order in orders on dechead.OrderID equals order.ID into orders2
                              from order in orders2.DefaultIfEmpty()
                              join clientAgreement in clientAgreements on order.ClientAgreementID equals clientAgreement.ID into clientAgreements2
                              from clientAgreement in clientAgreements2.DefaultIfEmpty()
                              where dechead.IsSuccess == true
                              select new Models.OrderReceiveDetail
                              {
                                  ReceiveDate = c.ReceiveDate,
                                  Client = c.Client,
                                  ReceiveAmount = c.ReceiveAmount,
                                  ClearAmount = c.ClearAmount,
                                  OrderID = c.OrderID,
                                  FinanceReceiptID = c.FinanceReceiptID,
                                  Tariff = c.Tariff,
                                  ExciseTax = c.ExciseTax,
                                  AddedValueTax = c.AddedValueTax,
                                  Incidental = c.Incidental,
                                  GoodsAmount = c.GoodsAmount,
                                  AgencyFee = c.AgencyFee,
                                  ContrNo = dechead.ContrNo,
                                  PaymentExchangeRate = paymentExchangeRate.ExchangeRate,
                                  InvoiceTypeInt = clientAgreement != null ? (int?)clientAgreement.InvoiceType : null,
                              };


            return finalResult;

        }
    }
}
