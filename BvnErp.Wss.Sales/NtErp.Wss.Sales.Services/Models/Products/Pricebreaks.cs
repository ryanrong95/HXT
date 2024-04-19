using NtErp.Wss.Sales.Services.Models.Orders;
using NtErp.Wss.Sales.Services.Underly;
using NtErp.Wss.Sales.Services.Underly.Products.Prices;
using NtErp.Wss.Sales.Services.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Models.Products
{
    /// <summary>
    /// 阶梯价集合
    /// </summary>
    [Newtonsoft.Json.JsonConverter(typeof(EnumerableConverter))]
    sealed public class Pricebreaks : BasePricebreaks
    {

        Product father;

        Pricebreaks(Product father)
        {
            this.father = father;
        }
        public Pricebreaks()
        {
        }

        public Pricebreaks(IEnumerable<Pricebreak> ienums) : base(ienums)
        {
        }


        public IEnumerable<Pricebreak> this[Currency index]
        {
            get { return this.GetIEnumerable(index); }
        }

        IEnumerable<Pricebreak> GetIEnumerable(Currency currency)
        {
            District delivery;

            delivery = ((SaleProduct)father).District;

            //var current = InRuntime<WebBuilder>.Current;
            //if (current == null)
            //{
            //    delivery = District.HK;
            //}
            //else
            //{
            //    delivery = current.Delivery;
            //}


            IEnumerable<Pricebreak> currents = null;
            Currency quotation = Currency.Unkown;

            if (this.Any(item => item.Currency == currency))
            {
                quotation = currency;
                currents = this.GetIEnumerable().Where(item => item.Currency == currency);
            }
            else
            {
                Currency[] arry = { Currency.USD, Currency.GBP, Currency.EUR, Currency.JPY };

                for (int index = 0; index < arry.Length; index++)
                {
                    if (this.Any(item => item.Currency == arry[index]))
                    {
                        quotation = arry[index];
                        currents = this.Where(item => item.Currency == arry[index]);
                        break;
                    }
                }
                if (currents == null)
                {
                    throw new NotImplementedException($"No designated currency:{currency} is realized!");
                }
            }

            var taxrate = 1m;

            if (currency != quotation)
            {
                taxrate = Overalls.UnifyRates.Current.TaxRates[currency].Sum();
            }

            return currents.Select(item => new Pricebreak
            {
                Currency = currency,
                Moq = item.Moq,
                Price = (Overalls.UnifyRates.Current.FloatRates[delivery].Where(item.Currency, currency)
                    //* father.FeeRates[currency]
                    * taxrate
                    * item.Price).Fourh()
            });
        }

    }
}
