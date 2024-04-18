using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models.Enums
{
    /// <summary>
    /// 付汇敏感地区类型
    /// </summary>
    public enum PayExchangeSensitiveAreaType
    {
        /// <summary>
        /// 所有
        /// </summary>
        [Description("所有")]
        All = 0,

        /// <summary>
        /// 禁止
        /// </summary>
        [Description("禁止")]
        Forbid = 1,

        /// <summary>
        /// 敏感
        /// </summary>
        [Description("敏感")]
        Sensitive = 2,
    }
}
