using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    [Serializable]
    public class DecRequestCerts : BaseItems<DecRequestCert>
    {
        internal DecRequestCerts(IEnumerable<DecRequestCert> enums) : base(enums)
        {
        }

        internal DecRequestCerts(IEnumerable<DecRequestCert> enums, Action<DecRequestCert> action) : base(enums, action)
        {
        }

        public override void Add(DecRequestCert item)
        {
            base.Add(item);
        }

        protected override IEnumerable<DecRequestCert> GetEnumerable(IEnumerable<DecRequestCert> ienums)
        {
            return base.GetEnumerable(ienums);
        }
    }
}
