
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using Needs.Underly;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.IO;
using MvcApp.Buyer.Services.Models;

namespace MvcApp.Buyer.Services
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

        public decimal this[Currency index]
        {
            get
            {
                var arry = this.source.Where(item => item.Currency == index)
                    .Select(item => item.Value).ToArray();

                if (arry.Length == 0)
                {
                    return 1m;
                }

                return arry[0];
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
