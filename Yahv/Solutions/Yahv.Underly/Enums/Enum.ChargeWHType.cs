using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly.Enums
{
    /// <summary>
    /// 是否收取入仓费
    /// </summary>
    public enum ChargeWHType
    {
        /// <summary>
        /// 收取
        /// </summary>
        [Description("收取")]
        Charge = 1,

        /// <summary>
        /// 不收取
        /// </summary>
        [Description("不收取")]
        NotCharge = 2,
    }
}
