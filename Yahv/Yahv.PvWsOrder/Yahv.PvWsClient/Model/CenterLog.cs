using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Services;
using Yahv.Services.Enums;
using Yahv.Underly;
using Yahv.Underly.Erps;

namespace Yahv.PvWsClient.Model
{
    public class CenterLog : OperatingLogger
    {
        IUser User;
        internal CenterLog(IUser user) : base(user.ID)
        {
        }
    }
}
