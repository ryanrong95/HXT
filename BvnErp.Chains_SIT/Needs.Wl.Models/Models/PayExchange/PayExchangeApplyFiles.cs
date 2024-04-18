using System;
using System.Collections.Generic;

namespace Needs.Wl.Models
{
    public class PayExchangeApplyFiles : BaseItems<PayExchangeApplyFile>
    {
        public PayExchangeApplyFiles(IEnumerable<PayExchangeApplyFile> enums) : base(enums)
        {
        }

        public PayExchangeApplyFiles(IEnumerable<PayExchangeApplyFile> enums, Action<PayExchangeApplyFile> action) : base(enums, action)
        {
        }

        public override void Add(PayExchangeApplyFile item)
        {
            base.Add(item);
        }

        protected override IEnumerable<PayExchangeApplyFile> GetEnumerable(IEnumerable<PayExchangeApplyFile> ienums)
        {
            return base.GetEnumerable(ienums);
        }
    }
}
