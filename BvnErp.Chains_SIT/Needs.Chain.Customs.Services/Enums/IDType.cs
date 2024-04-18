using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    public enum IDType
    {
        [Description("身份证")]
        IDCard = 1,

        [Description("驾驶证")]
        IDDriver = 2
    }
}
