using NtErp.Wss.Sales.Services.Underly;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services
{
    public class TaxRates : IEnumerable<TaxRate>
    {
        IEnumerable<TaxRate> source;

        public TaxRates()
        {
            List<TaxRate> list = (this.source = new List<TaxRate>()) as List<TaxRate>;
            list.Add(new TaxRate
            {
                Currency = Currency.CNY,
                Name = "VAT",
                Value = 1.17m
            });
        }

        public decimal[] this[Currency index]
        {
            get
            {
                var arry = this.source.Where(item => item.Currency == index)
                    .Select(item => item.Value).ToArray();

                if (arry.Length == 0)
                {
                    return new[] { 1m };
                }

                return arry;
            }
        }


        public IEnumerator<TaxRate> GetEnumerator()
        {
            return this.source.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
