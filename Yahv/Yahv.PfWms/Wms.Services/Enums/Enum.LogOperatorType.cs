using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Wms.Services.Enums
{
    public enum LogOperatorType
    {
        /// <summary>
        /// 新增
        /// </summary>
        [Description("新增")]
        Insert = 1,

        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        Delete = 2,

        /// <summary>
        /// 修改
        /// </summary>
        [Description("修改")]
        Update = 3,

        /// <summary>
        /// 查询
        /// </summary>
        [Description("查询")]
        Select = 4,
    }
}
