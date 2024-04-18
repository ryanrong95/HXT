using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 状态改变
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">状态改变事件参数</param>
    public delegate void StatusChangedEventHanlder(object sender, StatusChangedEventArgs e);

    /// <summary>
    /// 状态改变事件参数
    /// </summary>
    public class StatusChangedEventArgs : EventArgs
    {
        public dynamic Object { get; private set; }

        public StatusChangedEventArgs(object entity)
        {
            this.Object = entity;
        }

        public StatusChangedEventArgs() { }
    }
}