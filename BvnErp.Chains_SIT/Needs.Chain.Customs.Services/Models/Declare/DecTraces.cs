using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    [Serializable]
     public class DecTraces : BaseItems<DecTrace>
    {
        internal DecTraces(IEnumerable<DecTrace> enums) : base(enums)
        {
        }

        internal DecTraces(IEnumerable<DecTrace> enums, Action<DecTrace> action) : base(enums, action)
        {
        }

        public override void Add(DecTrace item)
        {
            base.Add(item);
        }

        protected override IEnumerable<DecTrace> GetEnumerable(IEnumerable<DecTrace> ienums)
        {
            return base.GetEnumerable(ienums);
        }
    }
}
