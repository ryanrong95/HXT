using NtErp.Wss.Sales.Services.Models.Orders;
using NtErp.Wss.Sales.Services.Models.Products;
using NtErp.Wss.Sales.Services.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Models
{
    public class Product : Underly.ProductBase
    {
        /// <summary>
        /// 进项服务ID
        /// </summary>
        public string ServiceInputID { get; set; }
        public Underly.Products.Prices.FeeRates<Currency> FeeRates { get; set; }

        Pricebreaks prices;
        //[System.Xml.Serialization.XmlIgnore]
        public Products.Pricebreaks Prices
        {
            get
            {
                if (this.prices == null)
                {
                    this.prices = new Products.Pricebreaks();
                }

                return Tutopo.From(this, this.prices);
            }
            set
            {
                this.prices = value;
            }
        }

        /// <summary>
        /// 获取单价
        /// </summary>
        /// <param name="quantity">数量</param>
        /// <returns></returns>
        internal decimal GetPrice(int quantity)
        {
            if (this.Prices.Count == 0)
            {
                throw new Exception("no prices");
            }
            if (quantity < 1)
            {
                quantity = 1;
            }
            var temps = this.Prices.Where(item => item.Moq <= quantity);
            if (temps.Count() == 0)
            {
                return this.Prices.OrderBy(item => item.Moq).First().Price;
            }
            else
            {
                return temps.OrderByDescending(item => item.Moq).First().Price;
            }
        }


        public Product() : base()
        {

        }
    }
}
