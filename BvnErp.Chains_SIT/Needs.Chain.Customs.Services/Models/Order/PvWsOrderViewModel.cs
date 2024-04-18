using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class PvWsOrderViewModel:IUnique
    {
        public string ID { get; set; }        
        public Enums.ClientOrderType OrderType { get; set; }
    }
}
