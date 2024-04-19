using System;
using System.Collections.Generic;
using Needs.Utils.Descriptions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Vrs.Services.Enums
{
    public enum ComapnyType
    {
        [Description("制造商")]
        Manufacturer = 1,
        [Description("供应商")]
        Supplier = 2,
        [Description("代理")]
        Agent = 3
    }
}
