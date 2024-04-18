using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.WL.ConsoleToolApp
{
    public class ClearingDataVo
    {
        public string WaybillCode { get; set; }
        public string CarrierID { get; set; }
        public DateTime ArriveDate { get; set; }
        public string OrderID { get; set; }
        public string OrderItemID { get; set; }
    }
}
