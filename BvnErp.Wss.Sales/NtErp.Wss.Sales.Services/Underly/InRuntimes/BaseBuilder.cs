using NtErp.Wss.Sales.Services.Utils.Converters;
using NtErp.Wss.Sales.Services.Utils.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NtErp.Wss.Sales.Services.Underly.InRuntimes
{
    public abstract class BaseBuilder
    {
        public string SessionID { get; private set; }
        public BaseBuilder(string sessionID)
        {
            this.SessionID = sessionID;
        }

    }
}
