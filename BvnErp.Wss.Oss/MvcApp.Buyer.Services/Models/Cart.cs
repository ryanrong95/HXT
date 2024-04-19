using MvcApp.Buyer.Services.Rates;
using Needs.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcApp.Buyer.Services.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Cart
    {
        public string ServiceOutputID { get; set; }
        public string ServiceInputID { get; set; }
        public string UserID { get; set; }
        public string CustomerCode { get; set; }
        public string ProductSign { get; set; }
        public int Quantity { get; set; }
        public System.Xml.Linq.XElement Xml { get; set; }

        public string GetLeadtime()
        {
            return this.Xml.Element("Product").Element("Leadtime")?.Value;
        }
        public string ToSupplier()
        {
            return this.Xml.Element("Product").Element("Supplier")?.Value;
        }
        /// <summary>
        /// 虚拟产品价格
        /// </summary>
        /// <returns></returns>
        public decimal GetVirtualPrice()
        {
            return decimal.Parse(this.Xml.Element("Price")?.Value);
        }
        /// <summary>
        /// 来源 ForCart=虚拟产品 WebSite=线上
        /// </summary>
        /// <returns></returns>
        public string GetFrom()
        {
            return this.Xml.Element("Product").Element("Source")?.Value;
        }

        public Pricebreak[] ToPrice()
        {
            var linq = from item in this.Xml.Element("Product").Element("Prices").Descendants("Pricebreak")
                       select new Pricebreak
                       {
                           Currency = (Needs.Underly.Currency)Enum.Parse(typeof(Needs.Underly.Currency), item.Element("Currency").Value),
                           Moq = int.Parse(item.Element("Moq").Value),
                           Price = decimal.Parse(item.Element("Price").Value)
                       };

            return linq.ToArray();
        }


        Pricebreak[] Prices
        {
            get { return this.ToPrice(); }
        }


        /// <summary>
        /// 获取单价
        /// </summary>
        /// <param name="quantity">数量</param>
        /// <returns></returns>
        public decimal GetPrice(int quantity, District district, Currency to)
        {
            if (this.Prices.Length == 0)
            {
                throw new Exception("no prices");
            }
            if (quantity < 1)
            {
                quantity = 1;
            }
            var temps = this.Prices.Where(item => item.Moq <= quantity);

            Pricebreak breaker;
            if (temps.Count() == 0)
            {
                breaker = this.Prices.OrderBy(item => item.Moq).First();
            }
            else
            {
                breaker = temps.OrderByDescending(item => item.Moq).First();
            }

            return breaker.Price
                * UnifyRates.Current.FloatRates[district].Where(breaker.Currency, to)
                * UnifyRates.Current.TaxRates[to];
        }


        public NtErp.Wss.Oss.Services.Models.StandardProduct ToProduct()
        {
            var xitems = this.Xml.Element("Product").Descendants("Item");
            var batchxml = xitems.Where(item => item.Element("Name").Value == "Batch").FirstOrDefault();
            var datecodexml = xitems.Where(item => item.Element("Name").Value == "DateCode").FirstOrDefault();

            string batch = "";
            string datecode = "";

            if (datecodexml == null)
            {
                datecode = batchxml?.Element("Value").Value;
            }
            else
            {
                datecode = datecodexml?.Element("Value").Value;
                batch = batchxml?.Element("Value").Value;
            }

            string name = xitems.Where(item => item.Element("Name").Value == "Name").FirstOrDefault()?.Element("Value").Value;
            string description = xitems.Where(item => item.Element("Name").Value == "Description").FirstOrDefault()?.Element("Value").Value;
            string Manufacturer = xitems.Where(item => item.Element("Name").Value == "Manufacturer").FirstOrDefault()?.Element("Value").Value;
            string dateCode = (xitems.Where(item => item.Element("Name").Value == "Batch").FirstOrDefault() ??
                    xitems.Where(item => item.Element("Name").Value == "DateCode").FirstOrDefault())?.Element("Value").Value;

            string packaging = xitems.Where(item => item.Element("Name").Value == "Packaging").FirstOrDefault()?.Element("Value").Value;
            string packageCase = xitems.Where(item => item.Element("Name").Value == "Package / Case").FirstOrDefault()?.Element("Value").Value;
            string productSign = xitems.Where(item => item.Element("Name").Value == "ProductSign").FirstOrDefault()?.Element("Value").Value;

            return new NtErp.Wss.Oss.Services.Models.StandardProduct
            {
                Name = name,
                Batch = string.IsNullOrWhiteSpace(batch) ? "" : batch,
                DateCode = string.IsNullOrWhiteSpace(datecode) ? "" : datecode,
                Description = string.IsNullOrWhiteSpace(description) ? "" : description,
                Manufacturer = new NtErp.Wss.Oss.Services.Models.Company
                {
                    Name = Manufacturer,
                    Type = NtErp.Wss.Oss.Services.CompanyType.Manufactruer
                },
                Packaging = string.IsNullOrWhiteSpace(packaging) ? "" : packaging,
                PackageCase = string.IsNullOrWhiteSpace(packageCase) ? "" : packageCase,
                SignCode = productSign,
            };
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Remove()
        {
            using (var reponsitory = new Layer.Data.Sqls.BvOrdersReponsitory())
            {
                reponsitory.Delete<Layer.Data.Sqls.BvOrders.Carts>(item => item.ServiceOutputID == this.ServiceOutputID);
            }
        }
    }
}
