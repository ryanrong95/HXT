using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 账期条款
    /// </summary>
    public class DebtTerm
    {
        public string Payer { get; internal set; }
        public string Payee { get; internal set; }
        public string Business { get; internal set; }
        public string Catalog { get; internal set; }
        public SettlementType SettlementType { get; set; }
        public int Months { get; set; }
        public int Days { get; set; }
        public ExchangeType ExchangeType { get; set; }
    }
}
