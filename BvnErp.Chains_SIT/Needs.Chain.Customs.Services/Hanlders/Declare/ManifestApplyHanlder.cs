using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 舱单申报（报文准备就绪）委托定义
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ManifestApplyHanlder(object sender, ManifestApplyEventArgs e);

    /// <summary>
    /// 舱单取消 委托定义
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ManifestCancelHandler(object sender, ManifestApplyEventArgs e);

    /// <summary>
    /// 舱单申报（报文准备就绪）事件参数
    /// </summary>
    public class ManifestApplyEventArgs : EventArgs
    {
        public Models.ManifestConsignment ManifestConsignment { get; private set; }

        public ManifestApplyEventArgs(Models.ManifestConsignment entity)
        {
            this.ManifestConsignment = entity;
        }

        public ManifestApplyEventArgs() { }
    }
}
