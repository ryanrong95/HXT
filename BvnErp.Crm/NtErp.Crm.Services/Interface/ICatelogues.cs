using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models
{
    public interface ICatelogues : Needs.Linq.IUnique,Needs.Linq.IPersistence
    {
        DateTime CreateDate { get; }

        DateTime UpdateDate { get; }

        String Summary { get; }
    }
}
