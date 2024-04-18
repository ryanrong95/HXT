using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PvWsClient.WebAppNew.Models
{
    public class StorageListViewModel
    {
        public string ID { get; set; }

        public string CreateDate { get; set; }

        public string CreateDateDateString { get; set; }

        public string PartNumber { get; set; }

        public string CustomsName { get; set; }

        public string Manufacturer { get; set; }

        public string Supplier { get; set; }

        public string Currency { get; set; }

        public string CurrencyShortName { get; set; }

        public string CurrencyInt { get; set; }

        public string UnitPrice { get; set; }

        public string Quantity { get; set; }

        public string WareHouseName { get; set; }

        public string TotalPrice { get; set; }

        public string WareHouseID { get; set; }

        public bool IsCheck { get; set; }

        public string InputID { get; set; }

        public string Origin { get; set; }

        public decimal Num { get; set; }

        public decimal SaleQuantity { get; set; }

        public string PackageCase { get; set; }

        public string DateCode { get; set; }
    }
}