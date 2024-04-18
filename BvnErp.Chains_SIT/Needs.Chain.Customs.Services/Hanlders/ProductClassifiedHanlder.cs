using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 用于处理ClassifyProduct.Classify方法
    /// 订单产品归类
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">事件参数</param>
    public delegate void ProductClassifiedHanlder(object sender, ProductClassifiedEventArgs e);

    /// <summary>
    /// 状态改变事件参数
    /// </summary>
    public class ProductClassifiedEventArgs : EventArgs
    {
        public Interfaces.IProduct Product { get; private set; }

        public ProductClassifiedEventArgs(Interfaces.IProduct product)
        {
            this.Product = product;
        }

        public ProductClassifiedEventArgs() { }
    }
}
