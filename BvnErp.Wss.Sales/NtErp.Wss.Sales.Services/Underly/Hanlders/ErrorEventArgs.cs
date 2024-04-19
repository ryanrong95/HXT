using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly
{
    /// <summary>
    /// 错误句柄
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">错误参数</param>
    public delegate void ErrorHanlder(object sender, ErrorEventArgs e);

    /// <summary>
    /// 程序返回执行失败消息
    /// </summary>
    /// <param name="msg"></param>
    //public delegate void ErrorMsg(string msg);
    /// <summary>
    /// 错误类型
    /// </summary>
    public enum ErrorType
    {
        /// <summary>
        /// 未知错误
        /// </summary>
        Error,
        /// <summary>
        /// 命名性
        /// </summary>
        Naming,
        /// <summary>
        /// 重复性
        /// </summary>
        Repeated,
        /// <summary>
        /// 值范围
        /// </summary>
        Range,
        /// <summary>
        /// 系统定义
        /// </summary>
        System,
        /// <summary>
        /// 客户自定义
        /// </summary>
        Customer,
    }
    /// <summary>
    /// 错误事件参数
    /// </summary>
    public class ErrorEventArgs : EventArgs
    {
        /// <summary>
        /// 错误类型
        /// </summary>
        public ErrorType Type { get; internal set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">错误类型</param>
        public ErrorEventArgs(ErrorType type = ErrorType.Error)
        {
            this.Type = type;
        }

        /// <summary>
        /// 源异常
        /// </summary>
        public Exception Source { get; internal set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ex">源异常</param>
        public ErrorEventArgs(Exception ex) : this(ErrorType.System)
        {
            this.Source = ex;
        }

        /// <summary>
        /// 自定义消息
        /// </summary>
        public string Message { get; internal set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mesasge">自定义消息</param>
        public ErrorEventArgs(string mesasge) : this(ErrorType.Customer)
        {
            this.Message = mesasge;
        }
    }
}
