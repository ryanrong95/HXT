using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 香港交货方式
    /// </summary>
    public enum IcgooClassifyTypeEnums
    {
        /// <summary>
        /// 未确认
        /// </summary>
        [Description("未确认")]
        NotConfirm = 0,

        /// <summary>
        /// 商检
        /// </summary>
        [Description("商检")]
        Inspection = 1,

        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal = 2,

        /// <summary>
        /// 禁运(未核实)
        /// </summary>
        [Description("禁运(未核实)")]
        EmbargoNotyet = 3,

        /// <summary>
        /// CCC(未核实)
        /// </summary>
        [Description("CCC(未核实)")]
        CCCNotyet = 4,

        /// <summary>
        /// CCC
        /// </summary>
        [Description("CCC")]
        CCC = 5,

        /// <summary>
        /// 禁运
        /// </summary>
        [Description("禁运")]
        Embargo = 6,

        /// <summary>
        /// 香港管制
        /// </summary>
        [Description("香港管制")]
        HKLimit = 7,

        /// <summary>
        /// 高价值
        /// </summary>
        [Description("高价值")]
        HighValue =8,
    }


}