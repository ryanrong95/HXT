using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    class PvWsOrderItemViewModel:IUnique
    {
        public string ID { get; set; }
        public string OrderID { get; set; }
        public string TinyOrderID { get; set; }
        public string StorageID { get; set; }
    }
}
