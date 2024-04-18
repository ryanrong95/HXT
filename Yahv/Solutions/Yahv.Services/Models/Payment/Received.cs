using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 实收
    /// </summary>
    public class Received : IUnique
    {
        public string ID { get; set; }
        public string ReceivableID { get; set; }
        public AccountType AccountType { get; set; }
        public decimal Price { get; set; }
        public decimal PaidPrice { get; set; }
        public Currency Currency1 { get; set; }
        public decimal Price1 { get; set; }
        public decimal Rate1 { get; set; }
        public string AdminID { get; set; }
        public string AdminName { get; set; }
        public string OrderID { get; set; }
        public string WaybillID { get; set; }
        public DateTime CreateDate { get; set; }
        public string Summay { get; set; }
        public string AccountCode { get; set; }
        public string FlowID { get; set; }

        /// <summary>
        /// 银行流水号
        /// </summary>
        public string FormCode { get; set; }
    }
}
