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
    public delegate void GetPreProductSuccessHanlders(object sender, GetPreProductSuccessEventArgs e);

    /// <summary>
    /// 报关成功事件参数参数
    /// </summary>
    public class GetPreProductSuccessEventArgs : EventArgs
    {
        public Models.IcgooPreProduct icgooPreProduct { get; private set; }

        public GetPreProductSuccessEventArgs(Models.IcgooPreProduct entity)
        {
            this.icgooPreProduct = entity;
        }

        public GetPreProductSuccessEventArgs() { }
    }
}