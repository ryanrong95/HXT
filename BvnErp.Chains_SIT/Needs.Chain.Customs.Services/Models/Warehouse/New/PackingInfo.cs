using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class PackingInfo
    {
        public string OrderID { get; set; }
        public string EntryNoticeID { get; set; }
        public string PackingDate { get; set; }
        public string BoxIndex { get; set; }
        public decimal Weight { get; set; }
        public bool IsExpress { get; set; }
        public string CarrierID { get; set; }
        public string ExpressCompany { get; set; }
        public string WaybillNo { get; set; }
        public List<SortingModel> PackingItems { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string AdminID { get; set; }
    }
}
