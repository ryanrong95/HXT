using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PsWms.SzMvc.Services.Enums
{
    /// <summary>
    /// PsOrder 库中, 文件类型
    /// </summary>
    public enum PsOrderFileType
    {
        /// <summary>
        /// 外观拍照
        /// </summary>
        [Description("外观拍照")]
        Exterior = 1,

        /// <summary>
        /// 单据拍照
        /// </summary>
        [Description("单据拍照")]
        Form = 2,

        /// <summary>
        /// 标签
        /// </summary>
        [Description("标签")]
        Label = 3,

        /// <summary>
        /// 送货文件(用于出库)
        /// </summary>
        [Description("送货文件")]
        OutDelivery = 4,

        /// <summary>
        /// 送货文件(用于入库)
        /// </summary>
        [Description("送货文件")]
        InDelivery = 5,

        /// <summary>
        /// 提货文件
        /// </summary>
        [Description("提货文件")]
        Taking = 6,

        /// <summary>
        /// 账单
        /// </summary>
        [Description("账单")]
        Bill = 7,
    }
}
