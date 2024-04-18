using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Utils.Descriptions;

namespace NtErp.Crm.Services.Enums
{
    public enum ApplyStep
    {
        [Description("等待")]
        Waiting = 10,
        [Description("同意")]
        Allow = 20,
        [Description("否决")]
        Vote = 30
    }
}
