using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    [Serializable]
    public class DecOtherPacks : BaseItems<DecOtherPack>
    {
        internal DecOtherPacks(IEnumerable<DecOtherPack> enums) : base(enums)
        {
        }

        internal DecOtherPacks(IEnumerable<DecOtherPack> enums, Action<DecOtherPack> action) : base(enums, action)
        {
        }

        public override void Add(DecOtherPack item)
        {
            base.Add(item);
        }

        protected override IEnumerable<DecOtherPack> GetEnumerable(IEnumerable<DecOtherPack> ienums)
        {
            return base.GetEnumerable(ienums);
        }
    }
}
