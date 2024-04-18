using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ManifestConsignmentItems : BaseItems<ManifestConsignmentItem>
    {
        internal ManifestConsignmentItems(IEnumerable<ManifestConsignmentItem> enums) : base(enums)
        {
        }

        internal ManifestConsignmentItems(IEnumerable<ManifestConsignmentItem> enums, Action<ManifestConsignmentItem> action) : base(enums, action)
        {
        }

        public override void Add(ManifestConsignmentItem item)
        {
            base.Add(item);
        }

        protected override IEnumerable<ManifestConsignmentItem> GetEnumerable(IEnumerable<ManifestConsignmentItem> ienums)
        {
            return base.GetEnumerable(ienums);
        }
    }
}
