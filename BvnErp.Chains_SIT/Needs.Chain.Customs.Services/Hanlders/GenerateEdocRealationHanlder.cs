using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 生成报关单电子单据委托定义
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void GenerateEdocRealationHanlder(object sender, GenerateEdocRealationEventArgs e);

    /// <summary>
    /// 生成报关单电子单据参数
    /// </summary>
    public class GenerateEdocRealationEventArgs : EventArgs
    {
        public dynamic Object { get; private set; }

        public GenerateEdocRealationEventArgs(object entity)
        {
            this.Object = entity;
        }

        public GenerateEdocRealationEventArgs() { }
    }
}
