using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services
{
    /// <summary>
    /// 类型
    /// </summary>
    [Flags]
    public enum CompanyType
    {
        /// <summary>
        /// 供应商
        /// </summary>
        [Description("供应商")]
        Supplier = 1,

        /// <summary>
        /// 制造商
        /// </summary>
        [Description("制造商")]
        Manufactruer = 2,

        [Description("平台公司")]
        Plot = 2 << 1,

        [Description("公司")]
        Company = 2 << 2
    }
}
