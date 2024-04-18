using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.User.Plat
{
    /// <summary>
    /// 错误句柄
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">错误参数</param>
    public delegate void LoginFailedHanlder(object sender, ErrorEventArgs e);

    /// <summary>
    /// 错误事件参数
    /// </summary>
    public class ErrorEventArgs : EventArgs
    {
        /// <summary>
        /// 自定义消息
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ErrorEventArgs()
        {

        }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ex">源异常</param>
        public ErrorEventArgs(Exception ex) : this(ex.Message)
        {
          
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mesasge">自定义消息</param>
        public ErrorEventArgs(string mesasge)
        {
            this.Message = mesasge;
        }
    }
}
