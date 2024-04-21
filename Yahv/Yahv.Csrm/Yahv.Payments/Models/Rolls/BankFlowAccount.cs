using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Payments.Models
{
    public class BankFlowAccount
    {
        /// <summary>
        /// 银行收款流水ID
        /// </summary>
        public string FlowAccountID { get; set; }
        public string Payer { get; set; }
        public string Payee { get; set; }
        //public string Business { get; set; }
        public string FormCode { get; set; }
        public Currency Currency { get; set; }
        public decimal Price { get; set; }
        public string Bank { get; set; }
        public string Account { get; set; }
    }
}
