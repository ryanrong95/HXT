using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NtErp.Services.Models
{
    public interface IMapsAdminClient : IPersistence, IFulSuccess, IFulError
    {
        string AdminID { get; set; }
        string ClientID { get; set; }


    }
}
