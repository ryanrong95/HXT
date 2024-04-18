using System;
using System.Collections.Generic;
using Needs.Utils.Descriptions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Enums
{
    public enum ActionMethord
    {
        [Description("接待")]
        reception = 10,
        [Description("拜访")]
        visit = 20,
        [Description("通讯")]
        communication =30,
        [Description("出差")]
        business = 40,
    }
}
