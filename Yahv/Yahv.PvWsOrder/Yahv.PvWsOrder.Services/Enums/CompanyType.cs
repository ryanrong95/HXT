using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services.Enums
{
    /// <summary>
    /// 公司类型
    /// </summary>
    public enum CompanyType
    {
        [Description("采购")]
        Purchase = 1,

        [Description("代销")]
        Consignment = 2,

        [Description("销售")]
        Sale = 3
    }

    /// <summary>
    /// 公司范围
    /// </summary>
    public enum CompanyRange
    {
        [Description("本地")]
        Local = 1,

        [Description("海外")]
        Overseas = 2,
    }
}
