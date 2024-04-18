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
    public delegate void PreCategoryHanlder(object sender, PreCategoryEventArgs e);

    /// <summary>
    /// 报关成功事件参数参数
    /// </summary>
    public class PreCategoryEventArgs : EventArgs
    {
        public Needs.Ccs.Services.Models.PreProduct icgooPreProduct { get; private set; }

        public PreCategoryEventArgs(Needs.Ccs.Services.Models.PreProduct entity)
        {
            this.icgooPreProduct = entity;
        }

        public PreCategoryEventArgs() { }
    }
}