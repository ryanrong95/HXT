using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models.Enums
{
    /// <summary>
    /// 运输批次-运输类型
    /// </summary>
    public enum VoyageType
    {
        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        Error = 0,

        /// <summary>
        /// 普通
        /// </summary>
        [Description("普通")]
        Normal = 1,

        /// <summary>
        /// 包车
        /// </summary>
        [Description("包车")]
        CharterBus = 2,
    }
}
