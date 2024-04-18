using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 舱单制单成功委托定义
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ManifestMakedHanlder(object sender, ManifestMakedEventArgs e);

    /// <summary>
    /// 舱单制单成功事件参数
    /// </summary>
    public class ManifestMakedEventArgs : EventArgs
    {
        public Models.ManifestConsignment ManifestConsignment { get; private set; }

        public string FileName { get; private set; }

        public ManifestMakedEventArgs(Models.ManifestConsignment entity, string fileName)
        {
            this.ManifestConsignment = entity;
            this.FileName = fileName;
        }

        public ManifestMakedEventArgs() { }
    }
}
