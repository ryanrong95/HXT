using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly.Translators
{
    public class Districts : NameTargetConllection<District>
    {
        internal Districts()
        {
            this["zh-CN"] = District.CN;
            this["zh"] = District.CN;

            this["en-US"] = District.HK;
            this["en"] = District.HK;
        }
    }
}
