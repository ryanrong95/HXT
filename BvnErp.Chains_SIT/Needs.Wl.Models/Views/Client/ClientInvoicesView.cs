using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    public class ClientInvoicesView : View<Models.ClientInvoice, ScCustomsReponsitory>
    {
        private string ClientID;

        public ClientInvoicesView(string clientID)
        {
            this.ClientID = clientID;
        }

        internal ClientInvoicesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ClientInvoice> GetIQueryable()
        {
            return from invoice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientInvoices>()
                   where invoice.Status == (int)Enums.Status.Normal && invoice.ClientID == this.ClientID
                   select new Models.ClientInvoice
                   {
                       ID = invoice.ID,
                       ClientID = invoice.ClientID,
                       Title = invoice.Title,
                       TaxCode = invoice.TaxCode,
                       Address = invoice.Address,
                       Tel = invoice.Tel,
                       BankName = invoice.BankName,
                       BankAccount = invoice.BankAccount,
                       DeliveryType = (Enums.InvoiceDeliveryType)invoice.DeliveryType,
                       InvoiceStatus = (Enums.ClientInvoiceStatus)invoice.InvoiceStatus,
                       Status = invoice.Status,
                       CreateDate = invoice.CreateDate,
                       UpdateDate = invoice.UpdateDate,
                       Summary = invoice.Summary
                   };
        }

    }
}
