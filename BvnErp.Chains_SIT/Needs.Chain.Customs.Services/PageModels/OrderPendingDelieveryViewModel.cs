using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class OrderPendingDelieveryViewModel : IUnique
    {
        public string ID { get; set; }
        public string MainOrderID { get; set; }
        public string ClientID { get; set; }
        public string AdminID { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public decimal DeclarePrice { get; set; }
        public string Currency { get; set; }
        public DateTime CreateDate { get; set; }
        public bool HasNotified { get; set; }
        public ClientType ClientType { get; set; }

        public bool? HasExited { get; set; }

        public decimal Amount { get; set; }//垫资金额

       public string AdvanceRecordID { get; set; }//垫资记录ID
    }
}
