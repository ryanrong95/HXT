using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService
{
    /// <summary>
    /// 通过接口获取/推送消息的处理结果
    /// </summary>
    public enum ResponseCode
    {
        /// <summary>
        /// 通过接口获取/推送消息成功
        /// </summary>
        [Description("成功")]
        Success = 200,

        /// <summary>
        /// 通过接口获取/推送消息异常
        /// </summary>
        [Description("失败")]
        Fail = 500
    }
}
