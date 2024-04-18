using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PsWms.DappApi.Services.Enums
{
    /// <summary>
    /// 费用类型
    /// </summary>
    public enum ChargeType
    {
        [Description("收入")]
        Income = 1,

        [Description("支出")]
        Pay = 2,
    }
}
