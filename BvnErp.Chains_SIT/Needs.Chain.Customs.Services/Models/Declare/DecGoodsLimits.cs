using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    [Serializable]
    public class DecGoodsLimits : BaseItems<DecGoodsLimit>
    {
        internal DecGoodsLimits(IEnumerable<DecGoodsLimit> enums) : base(enums)
        {
        }

        internal DecGoodsLimits(IEnumerable<DecGoodsLimit> enums, Action<DecGoodsLimit> action) : base(enums, action)
        {
        }

        public override void Add(DecGoodsLimit item)
        {
            base.Add(item);
        }

        protected override IEnumerable<DecGoodsLimit> GetEnumerable(IEnumerable<DecGoodsLimit> ienums)
        {
            return base.GetEnumerable(ienums);
        }
    }
}
