using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 产地变更
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">状态改变事件参数</param>
    public delegate void OriginChangedHanlder(object sender, OriginChangedEventArgs e);

    /// <summary>
    /// 状态改变事件参数
    /// </summary>
    public class OriginChangedEventArgs : EventArgs
    {
        public dynamic Object { get; private set; }

        public string Origin { get; private set; }

        public OriginChangedEventArgs(object entity,string origin)
        {
            this.Object = entity;
            this.Origin = origin;
        }

        public OriginChangedEventArgs() { }
    }
}