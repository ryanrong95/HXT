using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 产品管控类型
    /// </summary>
    public enum ProductControlType
    {
        /// <summary>
        /// 3C
        /// </summary>
        [Description("3C")]
        CCC = 100,

        /// <summary>
        /// 禁运
        /// </summary>
        [Description("禁运")]
        Forbid = 200
    }
}
