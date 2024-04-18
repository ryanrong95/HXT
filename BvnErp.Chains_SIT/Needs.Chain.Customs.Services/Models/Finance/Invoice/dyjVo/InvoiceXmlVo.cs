using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class InvoiceXmlVo : IUnique
    {
        public string ID { get; set; }
        public string InvoiceNoticeID { get; set; }
        public decimal InvoiceNoticeAmount { get; set; }
        public decimal InvoiceNoticeDiff { get; set; }
        public decimal InvoicePrice
        {
            get
            {
                return this.InvoiceNoticeAmount + this.InvoiceNoticeDiff;
            }
        }
        public int InvoiceType { get; set; }
        public decimal TaxRate { get; set; }
        public decimal XmlAmount { get; set; }
        public decimal XmlTax { get; set; }
        public decimal XmlPrice
        {
            get
            {
                return this.XmlAmount + this.XmlTax;
            }
        }
        public DateTime CreateDate { get; set; }
    }
}
