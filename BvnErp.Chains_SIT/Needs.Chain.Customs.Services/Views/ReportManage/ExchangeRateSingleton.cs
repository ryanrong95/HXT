using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ExchangeRateSingleton
    {
        static object locker = new object();
        static ExchangeRateSingleton instance;
        public List<DateExchangeRates> DateRates;
        private ExchangeRateSingleton()
        {
            DateRates = new List<DateExchangeRates>();
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var HKDID = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExchangeRates>()
                          .Where(t => t.Code == "HKD" && t.Type == (int)Enums.ExchangeRateType.RealTime)
                          .Select(t => t.ID).FirstOrDefault();

                var exchangeRateLogs = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExchangeRateLogs>().Where(t=>t.ExchangeRateID== HKDID).ToList();
                foreach(var item in exchangeRateLogs)
                {
                    DateExchangeRates rate = new DateExchangeRates();
                    rate.Date = item.CreateDate.Date;
                    rate.Rate = item.Rate;
                    DateRates.Add(rate);
                }
            }
        }

        public static ExchangeRateSingleton Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            instance = new ExchangeRateSingleton();
                        }
                    }
                }
                return instance;
            }
        }
    }

    public class DateExchangeRates
    {
        public DateTime Date { get; set; }
        public decimal Rate { get; set; }
    }

}
