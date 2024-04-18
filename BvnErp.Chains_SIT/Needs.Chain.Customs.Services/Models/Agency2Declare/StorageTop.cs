using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class StorageTop : IUnique
    {
        public string ID { get; set; }

        public StoragesType Type { get; set; }
        public string WareHouseID { get; set; }
        public string InputID { get; set; }
        public decimal? Total { get; set; }
        public decimal? Quantity { get; set; }
        public string Origin { get; set; }
        public bool IsLock { get; set; }
        public string ShelveID { get; set; }
        public string Supplier { get; set; }
        public string DateCode { get; set; }
        public string Summary { get; set; }
        public string CustomsName { get; set; }
        public string OrderID { get; set; }
        public string TinyOrderID { get; set; }
        public string ItemID { get; set; }
        public string TrackerID { get; set; }
        public ClientCurrency? Currency { get; set; }
        public decimal? UnitPrice { get; set; }
        public string PartNumber { get; set; }
        public string Manufacturer { get; set; }
        public string PackageCase { get; set; }
        public string Packaging { get; set; }
        public string ProductID { get; set; }

        /// <summary>
        /// 客户ID    
        /// </summary>
        public string ClientID { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }
        /// <summary>
        /// 入库时间
        /// </summary>
        public DateTime EnterDate { get; set; }
        public Client  MyClient{ get; set; }
    }
}
