using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PsWms.PdaApi.Services.Enums
{
    /// <summary>
    /// 文件类型
    /// </summary>
    public enum FileType
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
        /// 出库文件
        /// </summary>
        [Description("出库送货文件")]
        OutDelivery = 4,

        /// <summary>
        /// 入库文件
        /// </summary>
        [Description("入库送货文件")]
        InDelivery = 5,

        /// <summary>
        /// 入库自提使用
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
