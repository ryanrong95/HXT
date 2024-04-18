using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Utils.Descriptions;

namespace NtErp.Crm.Services.Enums
{
    public enum CompanyType
    {
        [Description("公司")]
        company =10,
        [Description("平台公司")]
        plot = 20,
        [Description("供应商")]
        Supplier = 30,
        [Description("品牌")]
        Manufacture = 40
    }
}
