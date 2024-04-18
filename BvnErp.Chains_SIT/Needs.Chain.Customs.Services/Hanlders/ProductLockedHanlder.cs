using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 产品归类锁定
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">事件参数</param>
    public delegate void ProductLockedHanlder(object sender, ProductLockedEventArgs e);

    /// <summary>
    /// 状态改变事件参数
    /// </summary>
    public class ProductLockedEventArgs : EventArgs
    {
        public Interfaces.IProduct Product { get; private set; }

        public Enums.ClassifyStep Step { get; set; }

        public ProductLockedEventArgs(Interfaces.IProduct product)
        {
            this.Product = product;
        }

        public ProductLockedEventArgs(Interfaces.IProduct product, Enums.ClassifyStep step)
        {
            this.Product = product;
            this.Step = step;
        }
    }
}
