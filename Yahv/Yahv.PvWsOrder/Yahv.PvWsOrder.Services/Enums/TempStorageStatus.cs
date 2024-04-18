using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PvWsOrder.Services.Enums
{
    public enum TempStorageStatus
    {
        /// <summary>
        /// 等待处理
        /// </summary>
        [Description("等待处理")]
        Waiting = 1,

        /// <summary>
        /// 已处理
        /// </summary>
        [Description("已处理")]
        Completed = 2,
    }
}
