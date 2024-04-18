using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using System;

namespace Needs.Wl.Logs.Services
{
    public class ExceptionLog : ModelBase<Layer.Data.Sqls.ScCustoms.ExceptionLogs, ScCustomsReponsitory>, IUnique, IPersistence
    {
        /// <summary>
        /// 导致错误的应用程序或对象的名称
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 调用堆栈
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        /// 异常的消息
        /// </summary>
        public string Message { get; set; }

        public override void Enter()
        {
            this.Reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ExceptionLogs()
            {
                ID = Guid.NewGuid().ToString("N"),
                Source = this.Source,
                StackTrace = this.StackTrace,
                Message = this.Message,
                CreateDate = DateTime.Now
            });
        }
    }
}