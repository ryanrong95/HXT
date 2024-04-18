using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    ///修改箱号
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">封箱事件参数</param>
    public delegate void BoxIndexChangedHanlders(object sender, BoxIndexChangedEventArgs e);

    /// <summary>
    /// 事件参数
    /// </summary>
    public class BoxIndexChangedEventArgs : EventArgs
    {
        public dynamic Object { get; private set; }

        public string BoxIndex { get; private set; }

        public BoxIndexChangedEventArgs(object entity, string boxIndex)
        {
            this.Object = entity;
            this.BoxIndex = boxIndex;
        }

        public BoxIndexChangedEventArgs() { }
    }
}