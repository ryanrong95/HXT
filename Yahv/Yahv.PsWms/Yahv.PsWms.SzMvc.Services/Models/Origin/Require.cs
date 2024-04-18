using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PsWms.SzMvc.Services.Models.Origin
{
    public class Require : IUnique
    {
        public string ID { get; set; }
        public string OrderID { get; set; }
        public string OrderTransportID { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
