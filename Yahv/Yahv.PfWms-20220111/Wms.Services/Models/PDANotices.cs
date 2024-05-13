using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Services.Models;

namespace Wms.Services.Models
{
    public class PDANotices : Yahv.Services.Models.Notice
    {
        public CenterProduct Product { get; set; }
        internal Sorting Sorting { get; set; }
        internal Storage Storage { get; set; }

        //internal Input Input { get; set; }
    }
}
