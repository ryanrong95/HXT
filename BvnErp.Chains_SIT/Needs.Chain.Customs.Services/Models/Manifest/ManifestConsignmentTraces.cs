using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ManifestConsignmentTraces : BaseItems<ManifestConsignmentTrace>
    {
        internal ManifestConsignmentTraces(IEnumerable<ManifestConsignmentTrace> enums) : base(enums)
        {
        }

        internal ManifestConsignmentTraces(IEnumerable<ManifestConsignmentTrace> enums, Action<ManifestConsignmentTrace> action) : base(enums, action)
        {
        }

        public override void Add(ManifestConsignmentTrace item)
        {
            base.Add(item);
        }

        protected override IEnumerable<ManifestConsignmentTrace> GetEnumerable(IEnumerable<ManifestConsignmentTrace> ienums)
        {
            return base.GetEnumerable(ienums);
        }
    }
}
