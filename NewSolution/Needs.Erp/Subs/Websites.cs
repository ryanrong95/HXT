using Needs.Erp.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Erp.Models
{
    /// <summary>
    /// Admin 
    /// </summary>
    public partial class Admin
    {
        public NtErp.Wss.Generic.ErpWebsite Websites
        {
            get
            {
                return new NtErp.Wss.Generic.ErpWebsite(this);
            }
        }
    }
}