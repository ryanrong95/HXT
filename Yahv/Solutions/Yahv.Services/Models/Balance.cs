using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 余额
    /// </summary>
    public class Balance
    {
        public string Payer { get; set; }
        public string Payee { get; set; }
        public Currency Currency { get; set; }
        public decimal Price { get; set; }
        public string Business { get; set; }
        public string Catalog { get; set; }
    }
}
