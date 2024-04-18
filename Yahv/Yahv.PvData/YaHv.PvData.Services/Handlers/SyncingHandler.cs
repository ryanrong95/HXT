using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.PvData.Services.Models;

namespace YaHv.PvData.Services.Handlers
{
    /// <summary>
    /// 数据同步前调用
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">事件参数</param>
    public delegate void SyncingHandler(object sender, SyncingEventArgs e);

    /// <summary>
    /// 数据同步事件参数
    /// </summary>
    public class SyncingEventArgs : EventArgs
    {
        public IEnumerable<OrderedProduct> OrderedProducts { get; private set; }

        public SyncingEventArgs(IEnumerable<OrderedProduct> orderedProducts)
        {
            this.OrderedProducts = orderedProducts;
        }

        public SyncingEventArgs() { }
    }
}
