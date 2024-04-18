using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class PartNoReceive
    {
        public string ccode { get; set; }
        public int deliveryWay { get; set; }
        public string currency { get; set; }
        public string refNo { get; set; }
        public List<PartNoReceiveItem> info { get; set; }
        public string warehouse { get; set; }
        /// <summary>
        /// 付汇供应商
        /// </summary>
        public string PayExchangeSupplier { get; set; }

    }
}
