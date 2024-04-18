using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Yahv.Underly;

namespace Yahv.PsWms.DappApi
{
    public class NoticeModel
    {
        public string ID { get; set; }
        public string WarehouseID { get; set; }
        public string ClientID { get; set; }
        public string CompanyID { get; set; }
        public string NoticeType { get; set; }
        public string FormID { get; set; }
        public string TrackerID { get; set; }
        public string Summary { get; set; }
        public List<NoticeItemModel> Items { get; set; }
        public List<Services.Models.Require> Requires { get; set; }
        public Services.Models.NoticeTransport Consignor { get; set; }
        public Services.Models.NoticeTransport Consignee { get; set; }
    }

    public class NoticeItemModel
    {
        public string ID { get; set; }
        public string Source { get; set; }
        public string InputID { get; set; }
        public string CustomCode { get; set; }
        public string StocktakingType { get; set; }
        public int Mpq { get; set; }
        public int PackageNumber { get; set; }
        public int Total { get; set; }
        public Currency Currency { get; set; }
        public decimal UnitPrice { get; set; }
        public string Supplier { get; set; }
        public string ClientID { get; set; }
        public string FormID { get; set; }
        public string FormItemID { get; set; }
        public string StorageID { get; set; }
        public string ShelveID { get; set; }
        public Services.Models.Product Product { get; set; }
    }
}