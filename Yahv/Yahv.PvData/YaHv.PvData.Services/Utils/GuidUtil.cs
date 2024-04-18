using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaHv.PvData.Services.Utils
{
    public static class GuidUtil
    {
        public static string NewGuidUp()
        {
            return System.Guid.NewGuid().ToString("N").ToUpper();
        }
    }
}
