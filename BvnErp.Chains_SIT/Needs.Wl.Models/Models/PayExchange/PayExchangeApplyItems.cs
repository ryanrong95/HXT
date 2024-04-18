using System;
using System.Collections.Generic;

namespace Needs.Wl.Models
{
    public class PayExchangeApplyItems : BaseItems<PayExchangeApplyItem>
    {
        public PayExchangeApplyItems(IEnumerable<PayExchangeApplyItem> enums) : base(enums)
        {
        }

        internal PayExchangeApplyItems(IEnumerable<PayExchangeApplyItem> enums, Action<PayExchangeApplyItem> action) : base(enums, action)
        {
        }

        public override void Add(PayExchangeApplyItem item)
        {
            base.Add(item);
        }
    }
}
