using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    [Serializable]
    public class DecContainers : BaseItems<DecContainer>
    {
        internal DecContainers(IEnumerable<DecContainer> enums) : base(enums)
        {
        }

        internal DecContainers(IEnumerable<DecContainer> enums, Action<DecContainer> action) : base(enums, action)
        {
        }

        public override void Add(DecContainer item)
        {
            base.Add(item);
        }

        protected override IEnumerable<DecContainer> GetEnumerable(IEnumerable<DecContainer> ienums)
        {
            return base.GetEnumerable(ienums);
        }
    }
}
