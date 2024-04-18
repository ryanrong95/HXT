using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PsWms.PdaApi.Services.Enums
{
    /// <summary>
    /// 报告类型
    /// </summary>
    public enum ReportType
    {
        /// <summary>
        /// 入库
        /// </summary>
        [Description("入库")]
        Inbound = 1,

        /// <summary>
        /// 出库
        /// </summary>
        [Description("出库")]
        Outbound = 2,

        /// <summary>
        /// 即入即出
        /// </summary>
        [Description("即入即出")]
        InAndOut = 3,

        /// <summary>
        /// 盘点, 在库行为
        /// </summary>
        [Description("盘点")]
        Stocktaking = 101,

        /// <summary>
        /// 检测, 在库行为
        /// </summary>
        [Description("检测")]
        Check = 102,

        /// <summary>
        /// 其他
        /// </summary>
        [Description("其他")]
        Other = 200
    }
}
