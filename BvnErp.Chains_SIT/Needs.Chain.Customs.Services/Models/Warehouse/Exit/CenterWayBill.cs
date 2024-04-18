using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class CenterWayBill : IUnique
    {
        public string ID { get; set; }
        public WaybillType WayBillType { get; set; }
        public string InitExType { get; set; }
        public PayType InitExPayType { get; set; }
        public string ConsigneeCompany { get; set; }
        public string ConsigneeAddress { get; set; }
        public string ConsigneeContact { get; set; }
        public string ConsigneePhone { get; set; }
        public string ConsignorContact { get; set; }
        public string ConsignorPhone { get; set; }
        public decimal Quantity { get; set; }
        public string ShelveID { get; set; }
        public string BoxCode { get; set; }
        public string PartNumber { get; set; }
        public string Manufacturer { get; set; }
        public string DeclareName { get; set; }
        public string OrderID { get; set; }
        public string ItemID { get; set; }
        public string CarNumber { get; set; }
        public bool? IsModify { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime BoxingDate { get; set; }

    }
}
