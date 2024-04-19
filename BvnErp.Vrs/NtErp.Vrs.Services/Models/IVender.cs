
using Needs.Underly;
using NtErp.Vrs.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Vrs.Services.Models
{
    public interface IVender : Needs.Linq.IUnique
    {
        string CompanyID { get; }
        Grade Grade { set; get; }
        ComapnyType Type { set; get; }
        Status Status { set; get; }
        string Properties { set; get; }
    }

}
