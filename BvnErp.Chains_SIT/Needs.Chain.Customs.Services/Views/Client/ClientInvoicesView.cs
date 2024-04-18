using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ClientInvoicesView : UniqueView<Models.ClientInvoice, ScCustomsReponsitory>
    {
        public ClientInvoicesView()
        {
        }

        internal ClientInvoicesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ClientInvoice> GetIQueryable()
        {
            return from invoice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientInvoices>()
                   where invoice.Status == (int)Enums.Status.Normal
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
                       Status = (Enums.Status)invoice.Status,
                       CreateDate = invoice.CreateDate,
                       UpdateDate = invoice.UpdateDate,
                       Summary = invoice.Summary
                   };
        }

    }
}
