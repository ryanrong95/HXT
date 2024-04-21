using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms.Services.Models_chenhan
{
    public class InputNotice
    {
        public string Sender { get; set; }
        public Waybill Waybill { get; set; }
        public Notice[] Notices { get; set; }
    }



    public class Notice
    {
        public string WarehouseID { get; set; }

        public string WaybillID { get; set; }
        public string ProductID { get; set; }
        /// <summary>
        /// 推荐库位
        /// </summary>
        public string ShelveID { get; set; }
        public string Type { get; set; }
        public object Pruduct { get; set; }
        public object Quantity { get; set; }
        public string Condition { get; set; }
        public string Source { get; set; }
        public int Target { get; set; }
        public string BoxCode { get; set; }


    }



    /// <summary>
    /// 物流系统中完成，直接发送运单的ID
    /// </summary>
    /// <remarks>
    /// 我们这里生成实际的运单Code，这就是我反反复复说的运单ID与运单Code的区别。如果能够做到提前预知，ID与Code可以一致
    /// </remarks>
    public class Waybill
    {
        public object Contact { get; set; }

        public string Address { get; set; }

        public string Carrier { get; set; }

        //或是其他
    }
}
