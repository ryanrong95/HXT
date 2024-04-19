using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Models.Carts
{
    /// <summary>
    /// 购物车产品
    /// </summary>
    public class CartProduct : Product
    {
        public CartProduct() : base()
        {
        }

        /// <summary>
        /// 产品加入购物车方式
        /// </summary>
        public CartSource Source { get; set; }

        /// <summary>
        /// 客户自己编号
        /// </summary>
        public string CustomerCode { get; set; }

        public string ProductSign
        {
            get
            {
                return this["ProductSign"];
            }
            set
            {
                this["ProductSign"] = value;
            }
        }

    }
}
