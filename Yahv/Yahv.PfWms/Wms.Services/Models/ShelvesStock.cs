using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms.Services.Models
{
    public class ShelvesStock
    {
        public string ID { get; set; }
        public string LeaseID { get; set; }
        public string EnterpriseID { get; set; }
        public decimal? Quantity { get; set; }
    }
}
