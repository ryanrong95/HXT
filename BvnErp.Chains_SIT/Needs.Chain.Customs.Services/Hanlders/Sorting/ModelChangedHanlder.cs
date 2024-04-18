using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 型号变更
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">状态改变事件参数</param>
    public delegate void ProductModelChangedHanlder(object sender, ProductModelChangedEventArgs e);

    /// <summary>
    /// 状态改变事件参数
    /// </summary>
    public class ProductModelChangedEventArgs : EventArgs
    {
        public dynamic Object { get; private set; }

        public string ProductModel { get; private set; }

        public ProductModelChangedEventArgs(object entity, string productmodel)
        {
            this.Object = entity;
            this.ProductModel = productmodel;
        }

        public ProductModelChangedEventArgs() { }
    }


}
