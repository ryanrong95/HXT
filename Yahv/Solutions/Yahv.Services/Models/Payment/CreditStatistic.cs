using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    public class CreditStatistic
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
        public string Catalog { get; set; }
        public Currency Currency { get; set; }
        /// <summary>
        /// 信用
        /// </summary>
        public decimal Total { get; set; }
        /// <summary>
        /// 花费
        /// </summary>
        public decimal Cost { get; set; }
        /// <summary>
        /// 剩余
        /// </summary>
        public decimal Remains
        {
            get
            {
                return this.Total - this.Cost;
            }
        }
    }
}
