using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services.Models
{
    public class SFResponse
    {
        public bool Success { get; set; }
        public string WaybillNo { get; set; }
        public string FilePath { get; set; }

        public SFResponse()
        {
            this.Success = true;
        }
    }
}
