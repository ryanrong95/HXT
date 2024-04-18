using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    [Serializable]
    public class DecLists : BaseItems<DecList>
    {
        internal DecLists(IEnumerable<DecList> enums) : base(enums)
        {
        }

        internal DecLists(IEnumerable<DecList> enums, Action<DecList> action) : base(enums, action)
        {
        }

        public override void Add(DecList item)
        {
            base.Add(item);
        }

        protected override IEnumerable<DecList> GetEnumerable(IEnumerable<DecList> ienums)
        {
            return base.GetEnumerable(ienums);
        }
    }
}
