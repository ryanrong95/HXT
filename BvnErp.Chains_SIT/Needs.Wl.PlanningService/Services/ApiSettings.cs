using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService
{
    [Serializable]
    public class ApiSettings : List<ApiSetting>
    {
        public ApiSetting this[string client]
        {
            get
            {
                return this.SingleOrDefault(item => item.Client == client);
            }
        }
    }

    [Serializable]
    public class Apis : List<Api>
    {
        public Api this[ApiType type]
        {
            get
            {
                return this.SingleOrDefault(item => item.Type == type);
            }
        }
    }
}