
using NtErp.Wss.Sales.Services.Underly.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NtErp.Wss.Sales.Services.Model.Orders
{
    public class Receipts : CumulateList<Receipt>
    {
        public Receipts() : base()
        {

        }

        public Receipts(IEnumerable<Receipt> t) : base(t)
        {

        }

        [XmlIgnore]
        public decimal Total
        {
            get
            {
                return this.Sum(item => item.Amount);
            }
        }
    }
}
