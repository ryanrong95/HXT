using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Utils.Descriptions;

namespace NtErp.Crm.Services.Enums
{
    public enum ProductStatus
    {

        /// <summary>
        /// 机会洽谈
        /// </summary>
        [Description("DO/机会洽谈")]
        DO = 10,

        /// <summary>
        /// 产品导入
        /// </summary>
        [Description("DI/产品导入")]
        DI = 50,

        /// <summary>
        /// 设计采纳
        /// </summary>
        [Description("DW/设计采纳")]
        DW = 80,

        /// <summary>
        /// 批量生产
        /// </summary>
        [Description("MP/批量生产")]
        MP = 100,

        /// <summary>
        /// 机会丢失
        /// </summary>
        [Description("DL/机会丢失")]
        DL = 0,
    }
}
