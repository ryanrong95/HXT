using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 报关单创建委托定义
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DeclareCreatedHanlder(object sender, DeclareCreatedEventArgs e);

    /// <summary>
    /// 报关单取消委托定义
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DeclareCancelHandler(object sender, DeclareCreatedEventArgs e);

    /// <summary>
    /// 报关单编辑委托定义
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DeclareEditedHandler(object sender, DeclareCreatedEventArgs e);

    /// <summary>
    /// 报关单创建委事件参数
    /// </summary>
    public class DeclareCreatedEventArgs : EventArgs
    {
        public Models.DecHead DecHead { get; private set; }


        public DeclareCreatedEventArgs(Models.DecHead entity)
        {
            this.DecHead = entity;
        }

        public DeclareCreatedEventArgs() { }
    }
}
