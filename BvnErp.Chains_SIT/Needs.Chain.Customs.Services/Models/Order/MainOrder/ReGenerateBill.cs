using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ReGenerateBill
    {
        public string OrderID { get; set; }
        public decimal CustomsExchangeRate { get; set; }
        public decimal RealExchangeRate { get; set; }
        public Enums.OrderBillType OrderBillType { get; set; }
        public decimal RealAgencyFee { get; set; }
    }
}
