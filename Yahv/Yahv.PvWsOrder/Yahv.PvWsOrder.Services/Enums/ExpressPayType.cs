using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PvWsOrder.Services.Enums
{
    public enum ExpressPayType
    {
        [Description("寄付")]
        Deposit = 1,

        [Description("到付")]
        Collected = 2
    }
}
