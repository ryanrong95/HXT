using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PvWsOrder.Services.Models
{
    public class TStorages:IUnique
    {
        public string ID { get; set; }
        public string WaybillID { get; set; }
        public string ProductID { get; set; }
        public decimal Quantity { get; set; }
        public string Origin { get; set; }
        public string DateCode { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public string Summary { get; set; }
    }
}
