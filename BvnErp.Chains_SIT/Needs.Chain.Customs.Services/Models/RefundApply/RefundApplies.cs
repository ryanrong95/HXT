using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    [Serializable]
    public class RefundApplies : BaseItems<RefundApply>
    {
        internal RefundApplies(IEnumerable<RefundApply> enums) : base(enums)
        {
        }

        internal RefundApplies(IEnumerable<RefundApply> enums, Action<RefundApply> action) : base(enums, action)
        {
        }

        public override void Add(RefundApply item)
        {
            base.Add(item);
        }

        public override void RemoveAll()
        {
            base.RemoveAll();
        }
    }
}
