using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services.ClientModels
{
    public class CustomerServiceLink : Yahv.Linq.IUnique
    {
        public string ID { get; set; }

        public string Link { get; set; }
    }
}
