using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services
{
    /// <summary>
    /// 1.对客户发货（Client）, 2.报关(Customs)  3. 供应商(Spplier)
    /// </summary>

    public enum WayItemSource
    {
        /// <summary>
        /// 对客户发货
        /// </summary>
        Client = 1,
        /// <summary>
        /// 报关
        /// </summary>
        Customs = 2,
        /// <summary>
        /// 供应商
        /// </summary>
        Spplier = 3,
    }

}
