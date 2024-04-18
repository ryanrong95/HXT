using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services;
using Yahv.Services.Enums;
using Yahv.Services.Models;

namespace Yahv.PvWsOrder.Services.Models
{
    public class CenterLog : OperatingLogger
    {
        public CenterLog(string adminId) : base(adminId)
        {
        }
    }
}
