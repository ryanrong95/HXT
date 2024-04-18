using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Services.Enums;
using Yahv.Services.Models;

namespace Yahv.PvWsPortal.Services
{
    /// <summary>
    /// 错误日志
    /// </summary>
    public class Logger : Yahv.Services.OperatingLogger
    {
        public Logger(string UserID) : base(UserID)
        {

        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="ErrorMessage"></param>
        public void Error(string ErrorMessage)
        {
            base[LogType.Error].Log(new OperatingLog
            {
                MainID = string.Empty,
                Operation = ErrorMessage,
                Summary = "错误日志",
            });
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="ErrorMessage"></param>
        static public void Error(string UserID, string ErrorMessage)
        {
            var logger = new Logger(UserID);
            logger[LogType.Error].Log(new OperatingLog
            {
                MainID = string.Empty,
                Operation = ErrorMessage,
                Summary = "错误日志",
            });
        }

        //系统日志记录
        static public void log(string AdminID, OperatingLog log)
        {
            var logger = new Logger(AdminID);
            logger.Log(log);
        }
    }
}
