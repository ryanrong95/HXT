using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 舱单创建委托定义
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ManifestCreatedHanlder(object sender, ManifestCreatedEventArgs e);

    /// <summary>
    /// 舱单创建委事件参数
    /// </summary>
    public class ManifestCreatedEventArgs : EventArgs
    {
        public Models.ManifestConsignment ManifestConsignment { get; private set; }


        public ManifestCreatedEventArgs(Models.ManifestConsignment entity)
        {
            this.ManifestConsignment = entity;
        }

        public ManifestCreatedEventArgs() { }
    }
}
