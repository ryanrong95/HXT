using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService
{
    [Serializable]
    public class ApiOrderCompanies : List<ApiOrderCompany>
    {
        public ApiOrderCompany this[string Key]
        {
            get
            {
                return this.SingleOrDefault(item => item.ID == Key);
            }
        }
    }
}