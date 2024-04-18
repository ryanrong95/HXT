using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Utils.Descriptions;

namespace NtErp.Crm.Services.Enums
{
    public enum  FeeType
    {
        [Description("交通")]
        Sales = 10,
        [Description("住宿")]
        SalesManager = 20,
        [Description("餐费")]
        FAE = 30,
        [Description("礼品")]
        ProductManager = 40,
        [Description("其他")]
        GeneralManager = 50
    }
}
