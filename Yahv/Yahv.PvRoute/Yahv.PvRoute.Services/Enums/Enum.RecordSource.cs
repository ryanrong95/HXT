using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PvRoute.Services.Enums
{
    /// <summary>
    /// 运单来源
    /// </summary>
    public enum RecordSource
    {
       
        /// <summary>
        /// 我方记录
        /// </summary>
        [Description("我方记录")]
        Ours = 10,

        /// <summary>
        /// 承运商记录
        /// </summary>
        [Description("承运商记录")]
        Carriers = 20
    }
}
