using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.XdtData.Import.Enums
{
    public enum ItemCategoryType
    {
        /// <summary>
        /// 普通
        /// </summary>
        [Description("普通")]
        Normal = 0,

        /// <summary>
        /// 商检
        /// </summary>
        [Description("商检")]
        Inspection = 1,

        /// <summary>
        /// 3C
        /// </summary>
        [Description("3C")]
        CCC = 2,

        /// <summary>
        /// 原产地证明
        /// </summary>
        [Description("原产地证明")]
        OriginProof = 4,

        /// <summary>
        /// 禁运
        /// </summary>
        [Description("禁运")]
        Forbid = 8,

        /// <summary>
        /// 检疫
        /// </summary>
        [Description("检疫")]
        Quarantine = 16,

        /// <summary>
        /// 高价值
        /// </summary>
        [Description("高价值")]
        HighValue = 32,

        /// <summary>
        /// 香港管制
        /// </summary>
        [Description("香港管制")]
        HKForbid = 64,
    }
}
