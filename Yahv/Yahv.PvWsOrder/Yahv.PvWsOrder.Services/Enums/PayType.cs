using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PvWsOrder.Services.Enums
{
    public enum PayType
    {
        [Description("寄付")]
        DeliveryPay = 1,

        [Description("到付")]
        CollectPay = 2,

        [Description("月结")]
        MonthlyPay = 3
    }
}
