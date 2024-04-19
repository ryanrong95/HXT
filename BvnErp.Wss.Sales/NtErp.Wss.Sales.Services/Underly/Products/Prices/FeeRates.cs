using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NtErp.Wss.Sales.Services.Underly.Products.Prices
{
    public class Fee<T>
    {
        public T Target { get; set; }
        public decimal Value { get; set; }
    }

    /// <summary>
    /// 费率管理器
    /// </summary>
    /// <typeparam name="T">目标</typeparam>
    public class FeeRates<T> : IEnumerable<Fee<T>>
    {
        Dictionary<T, decimal> source;

        public FeeRates()
        {
            this.source = new Dictionary<T, decimal>();
        }

        public decimal this[T index]
        {
            get
            {
                decimal outs;
                if (this.source.TryGetValue(index, out outs))
                {
                    return outs;
                }
                return 1m;
            }
            set { this.source[index] = value; }
        }

        public IEnumerator<Fee<T>> GetEnumerator()
        {
            return this.source.Select(item => new Fee<T>
            {
                Target = item.Key,
                Value = item.Value
            }).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Add(Fee<T> fee)
        {
            this.source.Add(fee.Target, fee.Value);
        }
    }
}
