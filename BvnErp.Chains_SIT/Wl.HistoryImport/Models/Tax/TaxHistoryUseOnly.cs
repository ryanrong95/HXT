using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wl.HistoryImport
{
    public class TaxHistoryUseOnly
    {
        public Needs.Ccs.Services.Enums.InvoiceType InvoiceType { get; set; }
        public decimal InvoiceTaxRate { get; set; }
        public string Adress { get; set; }
        public string Tel { get; set; }
        public string BankName { get; set; }
        public string BankAccount { get; set; }       
        public string InvoiceNo { get; set; }
        public DateTime CreateDate { get; set; }
        public List<TaxItemHistoryUseOnly> InvoiceItems { get; set; }
    }
}
