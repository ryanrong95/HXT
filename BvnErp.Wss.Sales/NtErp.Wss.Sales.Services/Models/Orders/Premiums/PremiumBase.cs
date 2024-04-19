using Needs.Underly.Translators;
using NtErp.Wss.Sales.Services.Underly;
using NtErp.Wss.Sales.Services.Underly.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NtErp.Wss.Sales.Services.Model.Orders
{
    /// <summary>
    /// 附加价值体系 Base
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PremiumBase<T> where T : IProduct
    {
        /// <summary>
        /// 销售价
        /// </summary>
        virtual public decimal Price { get; set; }

        /// <summary>
        /// 交货地
        /// </summary>
        virtual public District District { get; set; }
        /// <summary>
        /// 交易货币
        /// </summary>
        virtual public Currency Currency { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        virtual public int Quantity { get; set; }

        public T Product { get; set; }

        [XmlIgnore]
        public decimal SubTotal
        {
            get
            {
                return this.Price * this.Quantity;
            }
        }
    }
}
