using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ManifestConsignments : BaseItems<ManifestConsignment>
    {
        internal ManifestConsignments(IEnumerable<ManifestConsignment> enums) : base(enums)
        {
        }

        internal ManifestConsignments(IEnumerable<ManifestConsignment> enums, Action<ManifestConsignment> action) : base(enums, action)
        {
        }

        public override void Add(ManifestConsignment item)
        {
            base.Add(item);
        }

        protected override IEnumerable<ManifestConsignment> GetEnumerable(IEnumerable<ManifestConsignment> ienums)
        {
            return base.GetEnumerable(ienums);
        }
    }
}
