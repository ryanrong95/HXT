namespace Yahv.Systematic
{
    /// <summary>
    /// 系统操作
    /// </summary>
    interface IAction
    {
        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="log"></param>
        void Logs_Error(Yahv.Underly.Logs.Logs_Error log);
    }
}
