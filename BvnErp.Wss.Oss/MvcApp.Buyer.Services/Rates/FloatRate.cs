using System.Collections.Generic;
using System.Linq;
using Needs.Underly;
using Layer.Data.Sqls;
using MvcApp.Buyer.Services.Models;

namespace MvcApp.Buyer.Services.Rates
{
    public class FloatRates : FixedRates
    {
        IEnumerable<ExchangeRate> source;

        public FloatRates()
        {
            using (BvOtherReponsitory reponsitory = new BvOtherReponsitory())
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
