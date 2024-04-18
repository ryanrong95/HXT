using Needs.Linq;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models
{
    interface IOrder : IUnique, IPersistence, IFulError, IFulSuccess
    {
        string ClientID { get; }

        string CatalogueID { get; }
        CurrencyType Currency { get; }
        string BeneficiaryID { get; }
       string DeliveryAddress { get; }
        string Address { get; }

        string ConsigneeID { get; }
        string AdminID { get; }
        Status Status { get; }

        DateTime CreateDate { get; }

        DateTime UpdateDate { get; }
    }
}
