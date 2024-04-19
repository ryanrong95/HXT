using NtErp.Wss.Sales.Services.Utils.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Models.Orders
{
    public enum OrderOtherType
    {
        /// <summary>
        /// Invoice
        /// </summary>
        [Naming("Invoice")]
        Invoice = 1,
        /// <summary>
        /// Tax
        /// </summary>
        [Naming("Tax")]
        Tax = 2,
        /// <summary>
        /// consignee
        /// </summary>
        [Naming("Consignee")]
        Consignee = 3,
        /// <summary>
        /// beneficiary
        /// </summary>
        [Naming("Beneficiary")]
        Beneficiary = 4
    }
}
