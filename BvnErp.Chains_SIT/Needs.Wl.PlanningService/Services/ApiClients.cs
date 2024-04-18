using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService
{
    [Serializable]
    public class ApiClients : List<ApiClient>
    {
        public ApiClient this[string Key]
        {
            get
            {
                return this.SingleOrDefault(item => item.Key == Key);
            }
        }
    }
}