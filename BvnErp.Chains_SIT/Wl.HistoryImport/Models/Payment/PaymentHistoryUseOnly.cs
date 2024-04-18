using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wl.HistoryImport
{
    public class PaymentHistoryUseOnly
    {
        public string MemberCode { get; set; }
        public string ClientID { get; set; }
        public string SupplierName { get; set; }
        public string SupplierEnglishName { get; set; }
        public string SupplierAddress { get; set; }
        public string BankAccount { get; set; }
        public string BankName { get; set; }
        public string BankAddress { get; set; }
        public string SwiftCode { get; set; }
        public Needs.Ccs.Services.Enums.ExchangeRateType ExchangeRateType { get; set; }
        public string Currency { get; set; }
        public decimal ExchangeRate { get; set; }
        public Needs.Ccs.Services.Enums.PaymentType PaymentType { get; set; }
        public DateTime? ExceptPayDate { get; set; }
        public DateTime SettlementDate { get; set; }
        public Needs.Ccs.Services.Enums.PayExchangeApplyStatus PayExchangeApplyStatus { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public List<PaymentItemHistoryUseOnly> Lists { get; set; }
    }
}
