using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PsWms.SzMvc.Services.Enums
{
    public enum NoticeType
    {
        /// <summary>
        /// 入库
        /// </summary>
        [Description("入库")]
        Inbound = 1,

        /// <summary>
        /// 出库
        /// </summary>
        [Description("出库")]
        Outbound = 2,

        /// <summary>
        /// 即入即出
        /// </summary>
        [Description("即入即出")]
        InAndOut = 3,
    }
}
