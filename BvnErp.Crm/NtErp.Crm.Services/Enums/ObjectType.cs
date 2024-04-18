using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Utils.Descriptions;

namespace NtErp.Crm.Services.Enums
{
    public enum ObjectType
    {
        [Description("代理商")]
        Agent = 100,
        [Description("供应商")]
        Supplier = 200
    }
}
