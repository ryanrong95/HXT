using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services.Models
{
    public class ExceptionLog
    {
        public string ID { get; set; } = string.Empty;

        /// <summary>
        /// 导致错误的应用程序或对象的名称
        /// </summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// 导致错误的操作来源
        /// </summary>
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// 调用堆栈
        /// </summary>
        public string StackTrace { get; set; } = string.Empty;

        /// <summary>
        /// 异常的消息
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// CreateDate
        /// </summary>
        public DateTime CreateDate { get; set; }

        public void Enter()
        {
            using (PvWsOrderReponsitory reponsitory = new PvWsOrderReponsitory())
            {
                reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.ExceptionLogs()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    Source = this.Source,
                    Action = this.Action,
                    StackTrace = this.StackTrace,
                    Message = this.Message,
                    CreateDate = DateTime.Now,
                });
            }
        }
    }
}
