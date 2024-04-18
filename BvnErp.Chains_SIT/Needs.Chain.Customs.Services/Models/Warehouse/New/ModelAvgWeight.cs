using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{ 
    public  class ModelAvgWeight:IUnique
    {
        public string ID { get; set; }
        public string Model { get; set; }
        public decimal? Weight { get; set; }
    }
}
