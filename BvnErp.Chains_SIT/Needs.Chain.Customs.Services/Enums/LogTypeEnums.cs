using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 归类状态
    /// </summary>
    public enum LogTypeEnums
    {
        /// <summary>
        /// 锁定
        /// </summary>
        [Description("锁定")]
        Lock = 1,

        /// <summary>
        /// 归类
        /// </summary>
        [Description("归类")]
        Classify = 2,

       
    }
}
