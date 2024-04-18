using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    public class FlowAccountsStatistic
    {
        /// <summary>
        /// 付款人
        /// </summary>
        public string Payer { get; set; }
        /// <summary>
        /// 收款人
        /// </summary>
        public string Payee { get; set; }

        public string Business { get; set; }
        public Currency Currency { get; set; }
        public decimal Price { get; set; }
        public AccountType Type { get; set; }
    }
}
