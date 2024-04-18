using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wl.HistoryImport
{
    public class HistoryUseOnly
    {
        public string OrderID { get; set; }
        public string OrderNo { get; set; }
        public int IsLocal { get; set; }
        public string Currency { get; set; }
        public Decimal CustomsExchangeRate { get; set; }
        public decimal RealExchangeRate { get; set; }
        public Needs.Ccs.Services.Models.Client Client { get; set; }

        public string DeclarationDate { get; set; }
        public string ContrNO { get; set; }
        public string OwnerCusCode { get; set; }
        public string VoyNo { get; set; }
        public string BillNo { get; set; }
        public int days { get; set; }
        public string Port { get; set; }
        public bool IsInspection { get; set; }
        public DateTime OrderCreateDate { get; set; }
        public DateTime OrderUpdateDate { get; set; }
        public int TotalPacks { get; set; }
    }
}
