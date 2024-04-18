using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    public class Payment
    {
        public string ID { get; set; }
        public string PayableID { get; set; }
        public AccountType AccountType { get; set; }
        public decimal Price { get; set; }
        public Currency Currency1 { get; set; }
        public decimal Price1 { get; set; }
        public decimal Rate1 { get; set; }
        public DateTime CreateDate { get; set; }
        public string AdminID { get; set; }
        public string AdminName { get; set; }
        public string Summay { get; set; }
        public string OrderID { get; set; }
        public string WaybillID { get; set; }
        public string AccountCode { get; set; }
        public string FlowID { get; set; }

        public string FormCode { get; set; }
    }
}
