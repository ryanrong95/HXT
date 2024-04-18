using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services.Models
{
    public static class ExceptionLogExtends
    {
        /// <summary>
        /// 异常日志
        /// </summary>
        /// <param name="exception"></param>
        public static void CcsLog(this Exception exception, string attach = "")
        {
            ExceptionLog log = new ExceptionLog
            {
                Message = exception.Message,
                Source = exception.Source,
                Action = attach,
                StackTrace = exception.StackTrace
            };

            log.Enter();
        }
    }
}
