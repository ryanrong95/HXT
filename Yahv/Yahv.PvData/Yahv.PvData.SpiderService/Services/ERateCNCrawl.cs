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
    /// 大陆汇率抓取
    /// </summary>
    public class ERateCNCrawl : Crawl
    {
        public override string Url
        {
            get
            {
                return "http://www.boc.cn/sourcedb/whpj/index.html";
            }
        }

        public ERateCNCrawl()
        {
            this.Crawled += ERateCN_Crawled;
        }

        private void ERateCN_Crawled(object sender, Handlers.CrawledEventArgs e)
        {
            using (var reponsitory = new PvDataReponsitory())
            {
                RegexOptions RegexOptions = RegexOptions.Singleline | RegexOptions.IgnoreCase;
                string floating = ExchangeType.Floating.ToString(); //浮动汇率
                string tenAmChineseBank = ExchangeType.TenAmChineseBank.ToString(); //中国银行上午10点后的第一个实时汇率

                foreach (Currency c in Enum.GetValues(typeof(Currency)))
                {
                    Regex regex = new Regex(@"<td>" + c.GetDescription() + "</td>.*?<td>(.*?)</td>.*?<td>(.*?)</td>.*?<td>(.*?)</td>.*?<td>(.*?)</td>.*?<td>(.*?)</td>.*?<td.*?>(.*?)</td>", RegexOptions);
                    var groups = regex.Match(e.Html).Groups;
                    if (groups.Count == 1)
                        continue;

                    #region 实时汇率

                    var floatingRate = reponsitory.ReadTable<Layers.Data.Sqls.PvData.ExchangeRates>()
                        .FirstOrDefault(item => item.Type == floating && item.District == (int)District.CN && item.From == (int)c && item.To == (int)Currency.CNY);
                    if (floatingRate == null)
                    {
                        //如果数据库中没有相关币种汇率，直接插入
                        reponsitory.Insert(new Layers.Data.Sqls.PvData.ExchangeRates
                        {
                            Type = floating,
                            District = (int)District.CN,
                            From = (int)c,
                            To = (int)Currency.CNY,
                            Value = decimal.Parse(groups[3].Value) / 100,
                            ModifyDate = DateTime.Now
                        });
                    }
                    else
                    {
                        decimal value = decimal.Parse(groups[3].Value) / 100;
                        //如果汇率有变化，则更新
                        if (value != floatingRate.Value)
                        {
                            reponsitory.Update<Layers.Data.Sqls.PvData.ExchangeRates>(new
                            {
                                Value = decimal.Parse(groups[3].Value) / 100,
                                ModifyDate = DateTime.Now
                            }, item => item.Type == floating
                                && item.District == (int)District.CN && item.From == (int)c && item.To == (int)Currency.CNY);
                        }
                    }

                    #endregion

                    #region 中国银行上午10点后的第一个实时汇率

                    DateTime todayTenAm = DateTime.Today.AddHours(10); //当天上午10点
                    DateTime publishTime;
                    DateTime.TryParse(groups[6].Value, out publishTime); //汇率发布时间
                    if (publishTime >= todayTenAm)
                    {
                        var tenAmRate = reponsitory.ReadTable<Layers.Data.Sqls.PvData.ExchangeRates>()
                            .FirstOrDefault(item => item.Type == tenAmChineseBank && item.District == (int)District.CN && item.From == (int)c && item.To == (int)Currency.CNY);
                        if (tenAmRate == null)
                        {
                            //如果数据库中没有相关币种汇率，直接插入
                            reponsitory.Insert(new Layers.Data.Sqls.PvData.ExchangeRates
                            {
                                Type = tenAmChineseBank,
                                District = (int)District.CN,
                                From = (int)c,
                                To = (int)Currency.CNY,
                                Value = decimal.Parse(groups[3].Value) / 100,
                                ModifyDate = DateTime.Now
                            });
                        }
                        else
                        {
                            var date = tenAmRate.ModifyDate?.Date;
                            //如果今天还没有更新过，则更新
                            if (date != DateTime.Today)
                            {
                                reponsitory.Update<Layers.Data.Sqls.PvData.ExchangeRates>(new
                                {
                                    Value = decimal.Parse(groups[3].Value) / 100,
                                    ModifyDate = DateTime.Now
                                }, item => item.Type == tenAmChineseBank
                                    && item.District == (int)District.CN && item.From == (int)c && item.To == (int)Currency.CNY);
                            }
                        }
                    }

                    #endregion
                }
            }
        }

        public override void Crawling()
        {
            base.Crawling();
        }
    }
}
