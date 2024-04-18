using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PsWms.SzMvc.Services.Notice
{
    public class StorageNoticeModelNew
    {
        /// <summary>
        /// 
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string WarehouseID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CompanyID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int NoticeType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TrackerID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Item[] Items { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Require[] Requires { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Transport Consignor { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Transport Consignee { get; set; }




        public class Product
        {
            /// <summary>
            /// 
            /// </summary>
            public string Partnumber { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Brand { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Package { get; set; }


            /// <summary>
            /// 
            /// </summary>
            public string DateCode { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public int Mpq { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public int Moq { get; set; }
        }

        public class Item
        {
            /// <summary>
            /// 
            /// </summary>
            public Product Product { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string OrderItemID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Source { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string InputID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string CustomCode { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public int StocktakingType { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public int Mpq { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public int PackageNumber { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public int Total { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public int Currency { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public decimal UnitPrice { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Supplier { get; set; }
        }

        public class Require
        {
            /// <summary>
            /// 
            /// </summary>
            public string ID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string FormID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Contents { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public int Type { get; set; }
        }

        public class Transport
        {
            /// <summary>
            /// 
            /// </summary>
            public int TransportMode { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Carrier { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string WaybillCode { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public int ExpressPayer { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string ExpressEscrow { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public int ExpressFreight { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string TakingTime { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string TakerName { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string TakerLicense { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string TakerPhone { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string TakerIDCode { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public int TakerIDType { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Address { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Contact { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Phone { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Email { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Summary { get; set; }
        }
    }
}
