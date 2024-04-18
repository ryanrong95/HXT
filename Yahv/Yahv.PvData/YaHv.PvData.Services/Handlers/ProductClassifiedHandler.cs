using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaHv.PvData.Services.Handlers
{
    /// <summary>
    /// 产品归类
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">事件参数</param>
    public delegate void ProductClassifiedHandler(object sender, ProductClassifiedEventArgs e);

    /// <summary>
    /// 状态改变事件参数
    /// </summary>
    public class ProductClassifiedEventArgs : EventArgs
    {
        public Interfaces.IClassifyProduct Product { get; private set; }

        public ProductClassifiedEventArgs(Interfaces.IClassifyProduct product)
        {
            this.Product = product;
        }

        public ProductClassifiedEventArgs() { }
    }
}
