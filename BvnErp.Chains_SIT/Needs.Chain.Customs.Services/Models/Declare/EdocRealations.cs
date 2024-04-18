using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    [Serializable]
    public class EdocRealations : BaseItems<EdocRealation>
    {
        internal EdocRealations(IEnumerable<EdocRealation> enums) : base(enums)
        {
        }

        internal EdocRealations(IEnumerable<EdocRealation> enums, Action<EdocRealation> action) : base(enums, action)
        {
        }

        public override void Add(EdocRealation item)
        {
            base.Add(item);
        }

        protected override IEnumerable<EdocRealation> GetEnumerable(IEnumerable<EdocRealation> ienums)
        {
            return base.GetEnumerable(ienums);
        }
    }
}
