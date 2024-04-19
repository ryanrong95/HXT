using Needs.Linq;
using Needs.Utils.Descriptions;
using NtErp.Wss.Oss.Services.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Underly;

namespace NtErp.Wss.Oss.Services.Models
{
    public class OrderPay
    {
        public string OrderID { get; set; }
        public UserAccountType Type { get; set; }
        public decimal Price { get; set; }
    }

}
