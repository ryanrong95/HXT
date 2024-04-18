using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.PvData.SpiderService.Services
{
    /// <summary>
    /// 香港汇率抓取
    /// </summary>
    public class ERateHKCrawl : Crawl
    {
        public override string Url
        {
            get
            {
                return "https://www.hsbc.com.cn/1/2/!ut/p/c5/04_SB8K8xLLM9MSSzPy8xBz9CP0os3gDd-NQv1BDg2AXA1-PEE_zICNDAwgAykdiyrsi5InQ7ezu6GFi7gPkh3m6GniaOJkYmPq6GRp4GhPQHVyVEe_sp-_nkZ-bql-QGxpR7qioCAAh9cxV/dl3/d3/L0lDU0lKSWdrbUNTUS9JUFJBQUlpQ2dBek15cXpHWUEhIS80QkVqOG8wRmxHaXQtYlhwQUh0Qi83XzBHM1VOVTEwU0QwTUhUSTdIRU8wMDAwMDAwL2hHRUd5NDgyOTAwMDcvc2Eu/";
            }
        }

        public ERateHKCrawl()
        {
            this.Crawled += ERateHK_Crawled;
        }

        private void ERateHK_Crawled(object sender, Handlers.CrawledEventArgs e)
        {
            using (var reponsitory = new PvDataReponsitory())
            {
                RegexOptions RegexOptions = RegexOptions.Singleline | RegexOptions.IgnoreCase;

                foreach (Currency c in Enum.GetValues(typeof(Currency)))
                {
                    Regex regex = new Regex(@"<td.*?>" + c.GetDescription() + "&nbsp;.*?</td>.*?<td.*?>.*?</td>.*?<td.*?>(.*?)</td>.*?<td.*?>.*?</td>.*?<td.*?>.*?</td>", RegexOptions);
                    var groups = regex.Match(e.Html).Groups;
                    if (groups.Count == 1)
                        continue;

                    bool isExist = reponsitory.ReadTable<Layers.Data.Sqls.PvData.ExchangeRates>()
                        .Any(item => item.Type == "Floating" && item.District == (int)District.HK && item.From == (int)c && item.To == (int)Currency.CNY);
                    if (!isExist)
                    {
                        reponsitory.Insert(new Layers.Data.Sqls.PvData.ExchangeRates
                        {
                            Type = "Floating",
                            District = (int)District.HK,
                            From = (int)c,
                            To = (int)Currency.CNY,
                            Value = decimal.Parse(groups[1].Value)
                        });
                    }
                    else
                    {
                        reponsitory.Update<Layers.Data.Sqls.PvData.ExchangeRates>(new
                        {
                            Value = decimal.Parse(groups[1].Value)
                        }, item => item.Type == "Floating" && item.District == (int)District.HK && item.From == (int)c && item.To == (int)Currency.CNY);
                    }
                }
            }
        }

        public override void Crawling()
        {
            base.Crawling();
        }
    }
}
