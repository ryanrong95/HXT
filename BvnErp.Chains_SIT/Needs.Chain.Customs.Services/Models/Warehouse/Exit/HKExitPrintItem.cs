using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    [Serializable]
    public class HKExitPrintItem : HKExitProduct
    {
        public HKExitNotice HKExitNotice { get; set; }
    }
}
