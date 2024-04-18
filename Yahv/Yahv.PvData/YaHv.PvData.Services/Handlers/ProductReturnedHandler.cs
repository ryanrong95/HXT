using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaHv.PvData.Services.Handlers
{
    /// <summary>
    /// 产品归类退回
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">事件参数</param>
    public delegate void ProductReturnedHandler(object sender, ProductReturnedEventArgs e);

    /// <summary>
    /// 状态改变事件参数
    /// </summary>
    public class ProductReturnedEventArgs : EventArgs
    {
        public Interfaces.IClassifyProduct Product { get; private set; }

        public ProductReturnedEventArgs(Interfaces.IClassifyProduct product)
        {
            this.Product = product;
        }

        public ProductReturnedEventArgs() { }
    }
}
