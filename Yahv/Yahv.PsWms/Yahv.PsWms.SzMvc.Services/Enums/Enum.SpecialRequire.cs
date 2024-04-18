using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PsWms.SzMvc.Services.Enums
{
    /// <summary>
    /// 入库订单特殊要求
    /// </summary>
    public enum StorageInSpecialRequire
    {
        /// <summary>
        /// 扫描送货单
        /// </summary>
        [Description("扫描送货单")]
        SMSHD = 1,

        /// <summary>
        /// 拆箱清点
        /// </summary>
        [Description("拆箱清点")]
        CXQD = 2,

        /// <summary>
        /// 其他要求
        /// </summary>
        [Description("其他要求")]
        QTYQ = 3,
    }

    /// <summary>
    /// 出库订单特殊要求
    /// </summary>
    public enum StorageOutSpecialRequire
    {
        /// <summary>
        /// 发货单格式
        /// </summary>
        [Description("发货单格式")]
        FHD = 1,

        /// <summary>
        /// 客户标签格式
        /// </summary>
        [Description("客户标签格式")]
        KHBQ = 2,

        /// <summary>
        /// 其他要求
        /// </summary>
        [Description("其他要求")]
        QTYQ = 3,
    }
}
