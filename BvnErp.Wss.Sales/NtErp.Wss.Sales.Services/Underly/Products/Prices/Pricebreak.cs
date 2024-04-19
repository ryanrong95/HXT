using NtErp.Wss.Sales.Services.Underly;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly.Products.Prices
{
    sealed public class Pricebreak
    {
        /// <summary>
        /// 交货币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 阶梯最小起订量
        /// </summary>
        public int Moq { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }
    }
}
