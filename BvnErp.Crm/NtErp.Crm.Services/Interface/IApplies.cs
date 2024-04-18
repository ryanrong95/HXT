using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Linq;
using System.Collections;
using System.Linq.Expressions;
using Needs.Erp.Generic;
using NtErp.Crm.Services.Enums;

namespace NtErp.Crm.Services.Models
{
    public interface IApplies : Needs.Linq.IUnique
    {
        ApplyType Type { get; set; }
        string MainID { get; set; }
        string AdminID { get; set; }
        ApplyStatus Status { get; set; }

        DateTime CreateDate { get; set; }

        DateTime UpdateDate { get; set; }

        string Summary { get; set; }
    }
}
