using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NtErp.Wss.Sales.Services.Utils.Structures;

namespace NtErp.Wss.Sales.Services.Underly
{
    /// <summary>
    /// 货物入库检测类型
    /// </summary>
    public enum TestType
    {
        /// <summary>
        /// 称重
        /// </summary>
        [Naming("称重")]
        Weigh = 100,
        /// <summary>
        /// 上线
        /// </summary>
        [Naming("上线")]
        Online = 101,
        /// <summary>
        /// 其它
        /// </summary>
        [Naming("其它")]
        Other = 200,
    }
}
