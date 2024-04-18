using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvData.SpiderService
{
    public class Plat
    {
        static object locker = new object();
        static Plat current;

        public static Plat Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new Plat();
                        }
                    }
                }

                return current;
            }
        }

        public Services.Crawl Feroboc
        {
            get
            {
                return new Services.FerobocCrawl();
            }
        }

        public Services.Crawl ExchangeRateCN
        {
            get
            {
                return new Services.ERateCNCrawl();
            }
        }

        public Services.Crawl ExchangeRateHK
        {
            get
            {
                return new Services.ERateHKCrawl();
            }
        }
    }
}
