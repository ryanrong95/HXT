using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models
{
    public interface IReport : Needs.Linq.IUnique, Needs.Linq.IPersistence, Needs.Linq.IFulError, Needs.Linq.IFulSuccess
    {
        string AdminID { get;}

        string ClientID { get; }

        string Summary { get;  }
        DateTime CreateDate { get; }

        DateTime UpdateDate { get; }       
          
        string Context { get; set; }  
        
        string ActionID { get; }   

        Status Status { get; }
    }
}
