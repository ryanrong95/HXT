using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    [Serializable]
    public class DecLicenseDocus : BaseItems<DecLicenseDocu>
    {
        internal DecLicenseDocus(IEnumerable<DecLicenseDocu> enums) : base(enums)
        {
        }

        internal DecLicenseDocus(IEnumerable<DecLicenseDocu> enums, Action<DecLicenseDocu> action) : base(enums, action)
        {
        }

        public override void Add(DecLicenseDocu item)
        {
            base.Add(item);
        }

        protected override IEnumerable<DecLicenseDocu> GetEnumerable(IEnumerable<DecLicenseDocu> ienums)
        {
            return base.GetEnumerable(ienums);
        }
    }
}
