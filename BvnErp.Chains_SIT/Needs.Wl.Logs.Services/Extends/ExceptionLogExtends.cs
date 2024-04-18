using System;

namespace Needs.Wl.Logs.Services
{
    public static class ExceptionLogExtends
    {
        /// <summary>
        /// 异常日志
        /// </summary>
        /// <param name="exception"></param>
        public static void Log(this Exception exception)
        {
            ExceptionLog log = new ExceptionLog
            {
                Message = exception.Message,
                Source = exception.Source,
                StackTrace = exception.StackTrace
            };

            log.Enter();
        }
    }
}