using Needs.Erp.Generic;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models
{
    public interface IDeclareProduct : Needs.Linq.IUnique, Needs.Linq.IPersistence
    {
        string Name { get; }

        string CatelogueID { get; }

        string StandardID { get; }


        string SupplierID { get; }

        int Amount { get; }

        CurrencyType Currency
        {
            get; set;
        }

        decimal UnitPrice { get; }
        
        string Delivery { get; }

        string Count { get; }

        ProductStatus Status { get; }

        decimal? Expect { get; }

        decimal? TotalPrice { get; }

        DateTime? ExpectDate { get; }
    }
}
