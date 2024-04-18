using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.User.Plat
{
    /// <summary>
    /// 成功录入事件句柄
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">成功事件参数</param>
    public delegate void LoginSuccessHanlder(object sender, SuccessEventArgs e);

    /// <summary>
    /// 成功录入事件参数
    /// </summary>
    public class SuccessEventArgs : EventArgs
    {
        public dynamic Object { get; private set; }

        public SuccessEventArgs(object entity)
        {
            this.Object = entity;
        }

        public SuccessEventArgs() { }
    }
}
