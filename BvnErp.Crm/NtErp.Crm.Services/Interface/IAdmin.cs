using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Linq;
using System.Collections;
using System.Linq.Expressions;
using NtErp.Crm.Services.Enums;

namespace NtErp.Crm.Services.Models
{
    public interface IAdmin : Needs.Linq.IUnique
    { 
        string Name { get;}

        string RealName { get; }

        DateTime CreateDate { get; }

        DateTime UpdateDate { get; }

        Status Status { get; }

        string Summary { get; }
    }
}
