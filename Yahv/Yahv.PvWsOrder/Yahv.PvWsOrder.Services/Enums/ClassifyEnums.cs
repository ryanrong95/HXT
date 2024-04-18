using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services.Enums
{
    /// <summary>
    /// 归类阶段
    /// </summary>
    public enum ClassifyStep
    {
        /// <summary>
        /// 产品归类预处理一
        /// </summary>
        [Description("预处理一")]
        Step1 = 1,

        /// <summary>
        /// 产品归类预处理二
        /// </summary>
        [Description("预处理二")]
        Step2 = 2,

        /// <summary>
        /// 产品归类已完成
        /// </summary>
        [Description("已完成")]
        Done = 3,
    }
}
