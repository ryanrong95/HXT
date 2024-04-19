using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Models
{
    public partial class ClientTop : Needs.Linq.IUnique
    {
        public string ID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }

        public ClientTop()
        {

        }

    }
}
