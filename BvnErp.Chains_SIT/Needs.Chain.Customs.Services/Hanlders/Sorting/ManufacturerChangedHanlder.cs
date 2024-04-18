using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 品牌变更
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">状态改变事件参数</param>
    public delegate void ManufacturerChangedHanlder(object sender, ManufacturerChangedEventArgs e);

    /// <summary>
    /// 状态改变事件参数
    /// </summary>
    public class ManufacturerChangedEventArgs : EventArgs
    {
        public dynamic Object { get; private set; }

        public string Manufacturer { get; private set; }

        public ManufacturerChangedEventArgs(object entity, string manufacturer)
        {
            this.Object = entity;
            this.Manufacturer = manufacturer;
        }

        public ManufacturerChangedEventArgs() { }
    }
}
