using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// BalanceQueue 业务类型
    /// </summary>
    public enum BalanceQueueBusinessType
    {
        /// <summary>
        /// 报关单
        /// </summary>
        [Description("报关单")]
        DecHead = 100,

        /// <summary>
        /// 舱单
        /// </summary>
        [Description("舱单")]
        Manifest = 200,
    }

    /// <summary>
    /// BalanceQueue 过程状态
    /// </summary>
    public enum BalanceQueueProcessStatus
    {
        /// <summary>
        /// 在Queue中
        /// </summary>
        [Description("在Queue中")]
        InQueue = 100,

        /// <summary>
        /// 在Circle中
        /// </summary>
        [Description("在Circle中")]
        InCircle = 200,

        /// <summary>
        /// 异常取出
        /// </summary>
        [Description("异常取出")]
        ExceptionOut = 300,

        /// <summary>
        /// 正常取出
        /// </summary>
        [Description("正常取出")]
        NormalOut = 400,

        /// <summary>
        /// 准备发送邮件
        /// </summary>
        [Description("准备发送邮件")]
        PreSendEmail = 401,

        /// <summary>
        /// 已经发送邮件
        /// </summary>
        [Description("已经发送邮件")]
        SentEmail = 402,

        /// <summary>
        /// 准备重启海关软件
        /// </summary>
        [Description("准备重启海关软件")]
        PreRestartCustoms = 403,

        /// <summary>
        /// 已经重启海关软件
        /// </summary>
        [Description("已经重启海关软件")]
        RestartedCustoms = 404,

        /// <summary>
        /// 需要提醒
        /// </summary>
        [Description("需要提醒")]
        NeedRemind = 405,
    }
}
