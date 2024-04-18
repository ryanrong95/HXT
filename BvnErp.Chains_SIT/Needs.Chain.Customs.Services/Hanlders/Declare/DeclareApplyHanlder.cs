using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 报关申报（报文准备就绪）委托定义
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DeclareApplyHanlder(object sender, DeclareApplyEventArgs e);

    /// <summary>
    /// 报关申报（报文准备就绪）事件参数
    /// </summary>
    public class DeclareApplyEventArgs : EventArgs
    {
        public Models.DecHead DecHead { get; private set; }

        public DeclareApplyEventArgs(Models.DecHead entity)
        {
            this.DecHead = entity;
        }

        public DeclareApplyEventArgs() { }
    }
}
