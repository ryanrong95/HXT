using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsPortal.Services.Models
{
    public class BankChinaRateForClientVO
    {
        public string currencyId { get; set; }
        public string currencyName { get; set; }
        public DateTime publishDate { get; set; }
        public string publishDateString { get; set; }
        public decimal buyingRate { get; set; }
        public decimal cashPurchase { get; set; }
        public decimal spotSelling { get; set; }
        public decimal cashSelling { get; set; }
        public decimal bocConversion { get; set; }
    }
}
