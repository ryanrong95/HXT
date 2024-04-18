using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.PvData.SpiderService.Services
{
    /// <summary>
    /// 中国银行外汇牌价抓取
    /// </summary>
    public class FerobocCrawl : Crawl
    {
        public override string Url
        {
            get
            {
                return "http://www.boc.cn/sourcedb/whpj/index.html";
            }
        }

        public FerobocCrawl()
        {
            this.Crawled += Feroboc_Crawled;
        }

        private void Feroboc_Crawled(object sender, Handlers.CrawledEventArgs e)
        {
            using (var reponsitory = new PvDataReponsitory())
            {
                RegexOptions RegexOptions = RegexOptions.Singleline | RegexOptions.IgnoreCase;
                var ferobocs = new List<Layers.Data.Sqls.PvData.Feroboc>();

                foreach (Currency c in Enum.GetValues(typeof(Currency)))
                {
                    Regex regex = new Regex(@"<td>" + c.GetDescription() + "</td>.*?<td>(.*?)</td>.*?<td>(.*?)</td>.*?<td>(.*?)</td>.*?<td>(.*?)</td>.*?<td>(.*?)</td>.*?<td.*?>(.*?)</td>", RegexOptions);
                    var groups = regex.Match(e.Html).Groups;
                    if (groups.Count == 1)
                        continue;

                    DateTime publishDate;
                    DateTime.TryParse(groups[6].Value, out publishDate);

                    if (!reponsitory.ReadTable<Layers.Data.Sqls.PvData.Feroboc>().Any(t => t.Currency == (int)c && t.PublishDate == publishDate))
                    {
                        var feroboc = new Layers.Data.Sqls.PvData.Feroboc()
                        {
                            ID = Layers.Data.PKeySigner.Pick(YaHv.PvData.Services.PKeyType.Feroboc),
                            Currency = (int)c,
                            Xhmr = decimal.Parse(groups[1].Value),
                            Xcmr = decimal.Parse(groups[2].Value),
                            Xhmc = decimal.Parse(groups[3].Value),
                            Xcmc = decimal.Parse(groups[4].Value),
                            Zhzsj = decimal.Parse(groups[5].Value),
                            PublishDate = publishDate,
                            CreateDate = DateTime.Now
                        };
                        ferobocs.Add(feroboc);
                    }
                }

                if (ferobocs.Count() > 0)
                {
                    reponsitory.Insert(ferobocs.ToArray());
                }
            }
        }

        public override void Crawling()
        {
            base.Crawling();
        }
    }
}
