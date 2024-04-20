using Needs.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Generic.Models
{
    public class RechargeRecord
    {
        public Currency Currency { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreateDate { get; set; }

        public string Code { get; set; }

    }
}
