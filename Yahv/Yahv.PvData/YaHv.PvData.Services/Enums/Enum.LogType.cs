using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaHv.PvData.Services
{
    /// <summary>
    /// 归类日志类型
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// 锁定
        /// </summary>
        [Description("锁定")]
        Lock = 1,

        /// <summary>
        /// 归类
        /// </summary>
        [Description("归类")]
        Classify = 2,

        /// <summary>
        /// 退回
        /// </summary>
        [Description("退回")]
        Return = 3,
    }
}
