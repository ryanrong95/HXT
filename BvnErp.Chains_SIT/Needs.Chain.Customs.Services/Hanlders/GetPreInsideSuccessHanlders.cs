using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 报关成功委托定义
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void GetPreInsideSuccessHanlders(object sender, GetPreInsideSuccessEventArgs e);

    /// <summary>
    /// 报关成功事件参数参数
    /// </summary>
    public class GetPreInsideSuccessEventArgs : EventArgs
    {
        public Models.InsidePreProduct insidePreProduct { get; private set; }

        public GetPreInsideSuccessEventArgs(Models.InsidePreProduct entity)
        {
            this.insidePreProduct = entity;
        }

        public GetPreInsideSuccessEventArgs() { }
    }
}