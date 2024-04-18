using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class HKOperationLogModel : IUnique
    {
        public string ID { get; set; }
        public string AdminName { get; set; }
        public string Model { get; set; }
        public string Brand { get; set; }
        public string Origin { get; set; }
        public decimal Qty { get; set; }
        public string BoxIndex { get; set; }
        public DateTime CreateDate { get; set; }
        public string OperationTime { get; set; }
    }
}
