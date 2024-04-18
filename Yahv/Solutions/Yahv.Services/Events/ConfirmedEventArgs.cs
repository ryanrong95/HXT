using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Services
{
    abstract public class ConfirmedEventArgs : EventArgs
    {

    }

    /// <summary>
    /// 确认成功事件句柄
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ConfirmedHandler<T>(object sender, ConfirmedEventArgs<T> e) where T : ConfirmedEventArgs;

    /// <summary>
    /// 确认成功事件参数
    /// </summary>
    public class ConfirmedEventArgs<T> : EventArgs where T : ConfirmedEventArgs
    {
        public T[] Status { get; private set; }

        public ConfirmedEventArgs(params T[] sender)
        {
            this.Status = sender;
        }
    }
}
