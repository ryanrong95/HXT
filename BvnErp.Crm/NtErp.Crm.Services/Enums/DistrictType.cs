using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Utils.Descriptions;

namespace NtErp.Crm.Services.Enums
{
    /// <summary>
    /// 区域的类型
    /// </summary>
    [Flags]
    public enum DistrictType
    {
        [Description("销售")]
        Sales = 10,
        [Description("市场")]
        Market = 20,
    }
}
