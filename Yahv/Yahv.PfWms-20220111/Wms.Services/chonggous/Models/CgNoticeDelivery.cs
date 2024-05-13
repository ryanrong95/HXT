using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms.Services.chonggous.Models
{
    public class StoreChange
    {
        public List<ChangeItem> List = new List<ChangeItem>();
    }

    public class ChangeItem
    {
        public string orderid { get; set; }
    }
}
