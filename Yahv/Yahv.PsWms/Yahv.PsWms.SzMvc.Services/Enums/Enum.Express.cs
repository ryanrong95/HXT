using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PsWms.SzMvc.Services.Enums
{
    /// <summary>
    /// 快递
    /// </summary>
    public enum Express
    {
        /// <summary>
        /// 顺丰
        /// </summary>
        [Description("顺丰")]
        SF = 1,

        /// <summary>
        /// 跨越
        /// </summary>
        [Description("跨越")]
        KY = 2,

        /// <summary>
        /// 德邦
        /// </summary>
        [Description("德邦")]
        DB = 3,
    }
}
