using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class PvWmsTWaybillsViewModel:IUnique
    {
        public string ID { get; set; }
        public string OrderID { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
