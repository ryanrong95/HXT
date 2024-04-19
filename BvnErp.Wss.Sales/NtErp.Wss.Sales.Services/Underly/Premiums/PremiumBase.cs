using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly.Premiums
{
    public class PremiumBase
    {

        /// <summary>
        /// 交易币种
        /// </summary>
        public Currency Currency { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Summary { get; set; }

        public PremiumBase()
        {

        }

    }
}
