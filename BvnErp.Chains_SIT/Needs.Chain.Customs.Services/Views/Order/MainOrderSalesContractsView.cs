using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class MainOrderSalesContractsView : UniqueView<Models.SalesContract, ScCustomsReponsitory>
    {
        /// <summary>
        /// 合同ID，主订单号
        /// </summary>
        public string ID { get; set; }

        public MainOrderSalesContractsView()
        {

        }

        public MainOrderSalesContractsView(string ID)
        {
            this.ID = ID;
        }

        protected override IQueryable<Models.SalesContract> GetIQueryable()
        {

            var orderView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var agreementView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>();
            var clientInvoiceView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientInvoices>();

            var result = from order in orderView
                         join agreement in agreementView on order.ClientAgreementID equals agreement.ID
                         join invoice in clientInvoiceView on order.ClientID equals invoice.ClientID
                         where order.MainOrderId == this.ID
                         select new Models.SalesContract {
                             ID = order.MainOrderId,
                             SalesDate = order.CreateDate,
                             Buyer = new Models.InvoiceBaseInfo {
                                 Title = invoice.Title,
                                 Address = invoice.Address,
                                 Tel = invoice.Tel,
                                 BankName = invoice.BankName,
                                 BankAccount = invoice.BankAccount
                             },
                             //Seller = new Models.InvoiceBaseInfo { },
                             InvoiceType = (Enums.InvoiceType)agreement.InvoiceType
                         };

            return result;
        }

        public Models.SalesContract GetSales()
        {

            var model = this.GetIQueryable().FirstOrDefault();



            var order = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>().Where(t => t.MainOrderId == this.ID).FirstOrDefault();
            var agreement = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>().Where(t => t.ID == order.ClientAgreementID).FirstOrDefault();
            var clientInvoice = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientInvoices>().Where(t => t.ClientID == order.ClientID).FirstOrDefault();
            var items = new InvoiceOrderItemView().Where(t => t.MainOrderID == this.ID).ToList().Select(t => new {
                OrderItemID = t.ID,
                ProductName = t.Name,
                Model = t.Model,
                Quantity = t.Quantity,
                Unit = t.Unit,
                UnitPrice = t.SalesUnitPriceRat
            });

            return null;
        }
    }
}
