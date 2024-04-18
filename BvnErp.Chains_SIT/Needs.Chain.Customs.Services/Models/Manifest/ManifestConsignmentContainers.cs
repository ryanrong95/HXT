using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ManifestConsignmentContainers : BaseItems<ManifestConsignmentContainer>
    {
        internal ManifestConsignmentContainers(IEnumerable<ManifestConsignmentContainer> enums) : base(enums)
        {
        }

        internal ManifestConsignmentContainers(IEnumerable<ManifestConsignmentContainer> enums, Action<ManifestConsignmentContainer> action) : base(enums, action)
        {
        }

        public override void Add(ManifestConsignmentContainer item)
        {
            base.Add(item);
        }

        protected override IEnumerable<ManifestConsignmentContainer> GetEnumerable(IEnumerable<ManifestConsignmentContainer> ienums)
        {
            return base.GetEnumerable(ienums);
        }
    }
}
