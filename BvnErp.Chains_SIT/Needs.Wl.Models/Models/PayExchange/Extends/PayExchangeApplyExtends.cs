using Needs.Wl.Models.Views;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 付汇申请
    /// </summary>
    public static partial class PayExchangeApplyExtends
    {
        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="payExchangeApply"></param>
        /// <returns></returns>
        public static PayExchangeLogsView Logs(this PayExchangeApply payExchangeApply)
        {
            return new PayExchangeLogsView(payExchangeApply.ID);
        }

        /// <summary>
        /// 附件
        /// </summary>
        /// <param name="payExchangeApply"></param>
        /// <returns></returns>
        public static PayExchangeApplyFileView Files(this PayExchangeApply payExchangeApply)
        {
            return new PayExchangeApplyFileView(payExchangeApply.ID);
        }

        /// <summary>
        /// 付汇委托书
        /// TODO:未完成，需要同步完成系统环境的开发
        /// </summary>
        /// <param name="payExchangeApply"></param>
        /// <returns></returns>
        public static PayExchangeAgentProxy PayExchangeAgentProxy(this PayExchangeApply payExchangeApply)
        {
            return new PayExchangeAgentProxy(payExchangeApply);
        }
    }
}