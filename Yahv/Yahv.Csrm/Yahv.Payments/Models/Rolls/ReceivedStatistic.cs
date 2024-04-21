using System;
using Yahv.Underly;

namespace Yahv.Payments.Models
{
    public class ReceivedStatistic
    {
        public string ReceivedID { get; set; }
        public string ReceivableID { get; set; }
        public string Business { get; set; }
        public string Catalog { get; set; }
        public string Subject { get; set; }
        public string Payer { get; set; }
        public string PayerName { get; set; }
        public string Payee { get; set; }
        public string PayeeName { get; set; }
        public AccountType AccountType { get; set; }
        public Currency SettlementCurrency { get; set; }
        public decimal Price { get; set; }
        public string AdminID { get; set; }
        public DateTime CreateDate { get; set; }
        public string OrderID { get; set; }
        public string TinyID { get; set; }
        public string FlowID { get; set; }

        public Currency Currency1 { get; set; }
        public decimal Price1 { get; set; }
        public decimal Rate1 { get; set; }
    }
}