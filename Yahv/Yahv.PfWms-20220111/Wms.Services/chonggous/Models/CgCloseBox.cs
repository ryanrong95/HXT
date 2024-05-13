using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms.Services.chonggous.Models
{
    public class CgCloseBoxes
    {
        public List<CgCloseBox> Items { get; set; }
    }
    public class CgCloseBox
    {
        public string StorageID { get; set; }

        public string AdminID { get; set; }

        public string EnterCode { get; set; }

        public string BoxCode { get; set; }

        public string OrderID { get; set; }

        public string TinyOrderID { get; set; }

        public string ItemID { get; set; }

        public decimal? Quantity { get; set; }

        public decimal? Weight { get; set; }
    }
}
