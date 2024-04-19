using NtErp.Wss.Sales.Services.Models.Carts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Models.Orders
{
    /// <summary>
    /// 订单产品
    /// </summary>
    public class SaleProduct : CartProduct// Document, INaming, IProduct
    {
        //Document properties;
        public SaleProduct() : base()
        {
        }

        /// <summary>
        /// 分销商
        /// </summary>
        /// <example>
        /// 标识真实的平台来源供应商
        /// </example>
        public string Distributor
        {
            get { return this[nameof(this.Distributor)]; }
            set
            {
                this[nameof(this.Distributor)] = value;
            }
        }

        /// <summary>
        /// 采购商标识
        /// </summary>
        public string PurchaserSign
        {
            get { return this[nameof(this.PurchaserSign)]; }
            set
            {
                this[nameof(this.PurchaserSign)] = value;
            }
        }

        /// <summary>
        /// 折扣
        /// </summary>
        public decimal Discounts { get; set; }

        /// <summary>
        /// 是否已报价
        /// </summary>
        /// <example>
        /// 添加各种后续状态是本次服务的状态，而不是产品的
        /// 面向对象
        /// </example>
        public bool IsReported { get; set; }

    }
}
