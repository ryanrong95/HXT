using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Vrs.Services.Models
{
    public interface ICompany : Needs.Linq.IUnique
    {
        string Name { set; get; }
        Enums.ComapnyType Type { set; get; }
        string Code { set; get; }
        string Address { set; get; }
        string RegisteredCapital { get; set; }
        string CorporateRepresentative { get; set; }
        string Summary { set; get; }
        DateTime CreateDate { set; get; }
        DateTime UpdateDate { set; get; }
    }
}