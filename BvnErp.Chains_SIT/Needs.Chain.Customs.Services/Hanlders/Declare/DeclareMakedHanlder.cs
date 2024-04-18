using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 报关制单成功委托定义
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DeclareMakedHanlder(object sender, DeclareMakedEventArgs e);

    /// <summary>
    /// 报关制单成功事件参数
    /// </summary>
    public class DeclareMakedEventArgs : EventArgs
    {
        public Models.DecHead DecHead { get; private set; }

        public string FileName { get; private set; }

        public DeclareMakedEventArgs(Models.DecHead entity, string fileName)
        {
            this.DecHead = entity;
            this.FileName = fileName;
        }

        public DeclareMakedEventArgs() { }
    }
}
