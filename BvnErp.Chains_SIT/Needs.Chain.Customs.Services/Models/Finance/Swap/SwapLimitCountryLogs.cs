using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class SwapLimitCountryLogs : BaseItems<SwapLimitCountryLog>
    {
        internal SwapLimitCountryLogs(IEnumerable<SwapLimitCountryLog> enums) : base(enums)
        {
        }

        internal SwapLimitCountryLogs(IEnumerable<SwapLimitCountryLog> enums, Action<SwapLimitCountryLog> action) : base(enums, action)
        {
        }

        public override void Add(SwapLimitCountryLog item)
        {
            base.Add(item);
        }
    }
}
