using NtErp.Wss.Sales.Services.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NtErp.Wss.Sales.Services.Overalls.Rates
{
    [Serializable(), XmlRoot(nameof(FixedRates))]
    public class FloatRates : FixedRates
    {
        IEnumerable<ExchangeRate> source;

        public FloatRates()
        {
            using (var reponsitory = new Layer.Data.Sqls.BvOtherReponsitory() )
            {
                var linq = from rate in reponsitory.ReadTable<Layer.Data.Sqls.BvOthers.ExchangeRates>()
                           select new ExchangeRate()
                           {
                               District = (District)rate.District,
                               From = (Currency)rate.From,
                               To = (Currency)rate.To,
                               Value = (decimal)rate.Value
                           };

                this.source = linq.Select(item => item).ToArray();
            }
        }

        public FixedRates this[District index]
        {
            get
            {
                if (this.Any(item => item.District == index))
                {
                    return new FixedRates(this.Where(item => item.District == index));
                }

                return new FixedRates(this.Where(item => item.District == District.HK));
            }
        }

        override public IEnumerator<ExchangeRate> GetEnumerator()
        {
            return this.source.GetEnumerator();
        }
    }
}
