using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 信息推送状态
    /// </summary>
    public enum PushStatus
    {
        /// <summary>
        /// 信息未推送
        /// </summary>
        [Description("未推送")]
        Unpush = 100,

        /// <summary>
        /// 信息已推送
        /// </summary>
        [Description("已推送")]
        Pushed = 200,

        /// <summary>
        /// 信息已推送
        /// </summary>
        [Description("推送中")]
        Pushing = 300,

        /// <summary>
        /// 信息推送失败
        /// </summary>
        [Description("推送失败")]
        PushFailure = 400,
    }  
}
