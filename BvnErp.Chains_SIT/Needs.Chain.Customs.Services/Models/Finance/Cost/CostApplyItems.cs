using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    [Serializable]
    public class CostApplyItems : BaseItems<CostApplyItem>
    {
        internal CostApplyItems(IEnumerable<CostApplyItem> enums) : base(enums)
        {
        }

        internal CostApplyItems(IEnumerable<CostApplyItem> enums, Action<CostApplyItem> action) : base(enums, action)
        {
        }

        public override void Add(CostApplyItem item)
        {
            base.Add(item);
        }

        public override void RemoveAll()
        {
            base.RemoveAll();
        }
    }
}
