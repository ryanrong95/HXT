using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Models
{
    /// <summary>
    /// 订单支付信息
    /// </summary>
    public class Payments : BaseItems<UserOutput>
    {
        internal Payments(IEnumerable<UserOutput> enums) : base(enums)
        {
        }
        internal Payments(IEnumerable<UserOutput> enums, ItemStart<UserOutput> action) : base(enums, action)
        {
        }

        public decimal Total
        {
            get
            {
                return this.Sum(t => t.Amount);
            }
        }

        public decimal CashTotal
        {
            get
            {
                return this.Where(t => t.Type == UserAccountType.Cash).Sum(t => t.Amount);
            }
        }

        public decimal CreditTotal
        {
            get
            {
                return this.Where(t => t.Type == UserAccountType.Credit).Sum(t => t.Amount);
            }
        }

        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="item"></param>
        override public void Add(UserOutput item)
        {
            base.Add(item);
        }


    }
}
