using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Utils.Descriptions;

namespace NtErp.Crm.Services.Enums
{
   public enum WarningStatus
    {
        [Description("未查看")]
        unread = 10,

        [Description("已查看")]
        read = 20,
    }
}
