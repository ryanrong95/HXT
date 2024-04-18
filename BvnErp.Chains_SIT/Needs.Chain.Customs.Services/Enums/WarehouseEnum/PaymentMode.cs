using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    public enum PayType
    {
        [Description("寄付")]
        DeliveryPay = 1,

        [Description("到付")]
        CollectPay = 2,

        [Description("月结")]
        MonthlyPay = 3,

        [Description("第三方月结")]
        ThirdMonthlyPay = 4
    }
}
