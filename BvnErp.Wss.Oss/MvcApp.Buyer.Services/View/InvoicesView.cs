using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcApp.Buyer.Services.View
{
    public class InvoicesView : UniqueView<Models.Invoice, BvSsoReponsitory>
    {
        public InvoicesView()
        {

        }
        internal InvoicesView(BvSsoReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Invoice> GetIQueryable()
        {
            var linq = from entity in Reponsitory.ReadTable<Layer.Data.Sqls.BvSso.Invoices>()
                       select new Models.Invoice
                       {
                           ID = entity.ID,
                           UserID = entity.UserID,

                           Tel = entity.Tel,
                           BankAccount = entity.BankAccount,
                           BankName = entity.BankName,
                           CompanyName = entity.CompanyName,
                           RegAddress = entity.RegAddress,
                           SCC = entity.SCC,
                           Type = (Models.InvoiceType)entity.Type,
                       };

            return linq;
        }

    }

}
