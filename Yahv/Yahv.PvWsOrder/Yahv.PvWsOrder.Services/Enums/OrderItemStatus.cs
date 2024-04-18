using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PvWsOrder.Services.Enums
{
    public enum OrderItemStatus
    {
        /// <summary>
        /// 退回
        /// </summary>
        [Description("退回")]
        Returned = 100,


        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal = 200,

        /// <summary>
        /// 确认修改
        /// </summary>
        [Description("修改数量")]
        ConfirmUpdate = 210,

        /// <summary>
        /// 确认删除
        /// </summary>
        [Description("确认删除")]
        ConfirmDelete = 220,

        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        Deleted = 400
    }
}
