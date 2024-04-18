using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Enums;

namespace Yahv.PvWsOrder.Services.Models
{
    public class TWaybills : IUnique
    {
        public string ID { get; set; }
        public string WaybillCode { get; set; }
        public string WareHouseID { get; set; }
        public string CarrierID { get; set; }
        public string EnterCode { get; set; }
        public string ShelveID { get; set; }
        public string Supplier { get; set; }
        public string Summary { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public string AdminID { get; set; }
        public TempStorageStatus Status { get; set; }
        public string ForOrderID { get; set; }
        public DateTime? CompleteDate { get; set; }
    }
}
