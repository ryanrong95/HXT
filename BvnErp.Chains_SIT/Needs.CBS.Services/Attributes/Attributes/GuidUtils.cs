using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services
{
    public static class ChainsGuid
    {
        public static string NewGuidUp()
        {
            return System.Guid.NewGuid().ToString("N").ToUpper();
        }
    }
}
