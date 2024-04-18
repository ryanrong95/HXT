using Needs.Erp.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models
{
    public interface IPreminum : Needs.Linq.IUnique,Needs.Linq.IPersistence
    {
        string CatalogueID { get; }

        string DeclareID { get; }

        string Name { get; }

        decimal Price { get; }

        Status Status { get; }

        DateTime CreateDate { get; }

        DateTime UpdateDate { get; }
    }
}
