using NtErp.Wss.Sales.Services.Utils.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly.Collections
{
    public enum AlterStatus
    {
        Unknown = 0,
        /// <summary>
        /// 正常
        /// </summary>
        [Naming("正常")]
        Normal = 200,
        /// <summary>
        /// 修改
        /// </summary>
        [Naming("修改")]
        Altered = 300,
        /// <summary>
        /// 移除
        /// </summary>
        [Naming("移除")]
        Cancel = 400,
    }

}
