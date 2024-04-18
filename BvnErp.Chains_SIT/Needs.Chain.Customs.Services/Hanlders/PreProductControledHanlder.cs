using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 预归类产品管控事件
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">事件参数</param>
    public delegate void PreProductControledHanlder(object sender, PreProductControledEventArgs e);

    /// <summary>
    /// 预归类产品管控事件参数
    /// </summary>
    public class PreProductControledEventArgs : EventArgs
    {
        public Models.PreProductControl PreProductControl { get; private set; }

        public PreProductControledEventArgs(Models.PreProductControl orderControl)
        {
            this.PreProductControl = orderControl;
        }

        public PreProductControledEventArgs()
        {
        }
    }
}
