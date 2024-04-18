using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Services.Models
{
    public class LockStoreParam
    {
        public LStoreParam [] Params { get; set; }
    }

    public class LStoreParam
    {
        public string ID { get; set; }
        public string StorageID { get; set; }
        public string SessionID { get; set; }
        public decimal Quantity { get; set; }
    }
}
