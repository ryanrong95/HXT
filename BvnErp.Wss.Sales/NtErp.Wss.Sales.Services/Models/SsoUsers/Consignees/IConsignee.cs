using NtErp.Wss.Sales.Services.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Model
{
    public interface IConsignee : IUnique
    {
        string UserID { get; set; }
        string Country { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string Contact { get; set; }
        string Company { get; set; }
        string Tel { get; set; }
        string Email { get; set; }
        string Zipcode { get; set; }
        string Address { get; set; }
        DateTime CreateDate { get; set; }
        DateTime UpdateDate { get; set; }
    }
}
