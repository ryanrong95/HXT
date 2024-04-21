using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Erm.Services.Models.Rolls
{
    public class PayWageItem1
    {
        public string DateIndex { get; set; }
        public string PayID { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public WageItemType Type { get; set; }
        public int? Order { get; set; }
        public string Formula { get; set; }
    }
}
