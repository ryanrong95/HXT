using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// Site类型
    /// </summary>
    public enum SiteType
    {
        /// <summary>
        /// 进项
        /// </summary>
        [Description("进项")]
        Input = 1,

        /// <summary>
        /// 销项
        /// </summary>
        [Description("销项")]
        Output = 2
    }
}
