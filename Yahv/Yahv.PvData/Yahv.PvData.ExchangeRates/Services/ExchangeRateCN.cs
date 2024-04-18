using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Yahv.PvData.ExchangeRates.Extends;
using Yahv.PvData.ExchangeRates.Utils;
using Yahv.Underly;

namespace Yahv.PvData.ExchangeRates.Services
{
    /// <summary>
    /// 大陆汇率抓取服务
    /// </summary>
    public class ExchangeRateCN
    {
        Dictionary<Currency, DateTime> tenAmDic; //币种的上午10点汇率的发布时间

        public ExchangeRateCN()
        {
            tenAmDic = new Dictionary<Currency, DateTime>();
            foreach (Currency c in Enum.GetValues(typeof(Currency)))
            {
                if (c == Currency.Unknown || c == Currency.CNY)
                    continue;
                tenAmDic.Add(c, DateTime.Today);
            }
        }

        /// <summary>
        /// 抓取
        /// </summary>
        public void Crawling()
        {
            new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        #region 中国银行外汇牌价

                        string url = "http://www.boc.cn/sourcedb/whpj/index.html"; //请求地址
                        string indexHtml = null; //外汇牌价主页

                        using (var wc = new WebClient())
                        {
                            wc.Encoding = Encoding.UTF8;
                            indexHtml = wc.DownloadString(url);
                        }

                        #endregion

                        #region 汇率抓取

                        FloatingRateCrawl(indexHtml); //实时汇率抓取

                        DateTime todayTenAm = DateTime.Today.AddHours(10);
                        DateTime todayNineThirtyAm = DateTime.Today.AddHours(9).AddMinutes(30);

                        if(DateTime.Now >= todayNineThirtyAm) 
                        {
                            NineAmRateCrawl(indexHtml);
                        }

                        if (DateTime.Now >= todayTenAm)
                        {
                            TenAmRateCrawl(indexHtml); //上午10点汇率抓取
                        }

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"异常信息: {ex.Message}");
                        Console.WriteLine($"堆栈信息: {ex.StackTrace}");
                        //发送邮件
                        SmtpContext.Current.Send("抓取中国银行外汇牌价异常", $"异常信息: {ex.Message}\r\n 堆栈信息: {ex.StackTrace}");
                    }
                    finally
                    {
                        Thread.Sleep(3000);
                    }
                }
            })
            {
                IsBackground = true,
                Priority = ThreadPriority.Highest
            }.Start();
        }

        /// <summary>
        /// 实时汇率抓取
        /// </summary>
        /// <param name="indexHtml">中国银行外汇牌价主页</param>
        void FloatingRateCrawl(string indexHtml)
        {
            using (var reponsitory = new PvDataReponsitory())
            {
                RegexOptions regexOptions = RegexOptions.Singleline | RegexOptions.IgnoreCase;
                string floating = ExchangeType.Floating.ToString(); //浮动汇率
                var ferobocs = new List<Layers.Data.Sqls.PvData.Feroboc>();

                foreach (Currency c in Enum.GetValues(typeof(Currency)))
                {
                    if (c == Currency.Unknown || c == Currency.CNY)
                        continue;

                    //正则匹配
                    Regex regex = new Regex(@"<td>" + c.GetDescription() + "</td>.*?<td>(.*?)</td>.*?<td>(.*?)</td>.*?<td>(.*?)</td>.*?<td>(.*?)</td>.*?<td>(.*?)</td>.*?<td.*?>(.*?)</td>", regexOptions);
                    var groups = regex.Match(indexHtml).Groups;
                    if (groups.Count == 1)
                        continue;

                    //发布时间
                    DateTime publishDate;
                    DateTime.TryParse(groups[6].Value, out publishDate);

                    var feroboc = new Layers.Data.Sqls.PvData.Feroboc()
                    {
                        Type = floating,
                        Currency = (int)c,
                        Xhmr = decimal.Parse(groups[1].Value),
                        Xcmr = decimal.Parse(groups[2].Value),
                        Xhmc = decimal.Parse(groups[3].Value),
                        Xcmc = decimal.Parse(groups[4].Value),
                        Zhzsj = decimal.Parse(groups[5].Value),
                        PublishDate = publishDate,
                        CreateDate = DateTime.Now
                    };

                    //汇率
                    ExchangeRateEnter(feroboc, reponsitory);
                    //外汇牌价
                    if (!reponsitory.ReadTable<Layers.Data.Sqls.PvData.Feroboc>().Any(item => item.Type == floating && item.Currency == (int)c && item.PublishDate == publishDate))
                        ferobocs.Add(feroboc);
                }

                #region 新增外汇牌价/Feroboc
                if (ferobocs.Count() > 0)
                {
                    string[] ids = PKeyType.Feroboc.Pick(ferobocs.Count());
                    reponsitory.Insert(ferobocs.Select((item, index) => new Layers.Data.Sqls.PvData.Feroboc()
                    {
                        ID = ids[index],
                        Type = item.Type,
                        Currency = item.Currency,
                        Xhmr = item.Xhmr,
                        Xcmr = item.Xcmr,
                        Xhmc = item.Xhmc,
                        Xcmc = item.Xcmc,
                        Zhzsj = item.Zhzsj,
                        PublishDate = item.PublishDate,
                        CreateDate = item.CreateDate
                    }).ToArray());
                }
                #endregion
            }
        }

        /// <summary>
        /// 上午10点汇率抓取
        /// </summary>
        /// <param name="indexHtml">中国银行外汇牌价主页</param>
        void TenAmRateCrawl(string indexHtml)
        {
            using (var reponsitory = new PvDataReponsitory())
            using (var wc = new WebClient())
            {
                string searchUrl = "https://srh.bankofchina.com/search/whpj/search_cn.jsp"; //请求地址
                RegexOptions regexOptions = RegexOptions.Singleline | RegexOptions.IgnoreCase;
                string tenAmChineseBank = ExchangeType.TenAmChineseBank.ToString(); //中国银行上午10点后的第一个实时汇率

                foreach (Currency c in Enum.GetValues(typeof(Currency)))
                {
                    if (c == Currency.Unknown || c == Currency.CNY)
                        continue;

                    //正则匹配
                    Regex regex = new Regex(@"<td>" + c.GetDescription() + ".*?</td>.*?<td>(.*?)</td>.*?<td>(.*?)</td>.*?<td>(.*?)</td>.*?<td>(.*?)</td>.*?<td>(.*?)</td>.*?<td.*?>(.*?)</td>", regexOptions);
                    var groups = regex.Match(indexHtml).Groups;
                    if (groups.Count == 1)
                        continue;

                    DateTime todayTenAm = DateTime.Today.AddHours(10); //当天上午10点
                    if (tenAmDic[c] >= todayTenAm)
                        continue;

                    DateTime publishDate;
                    DateTime.TryParse(groups[6].Value, out publishDate); //发布时间

                    //如果发布时间已经超过上午10点，开始做汇率抓取
                    if (publishDate >= todayTenAm)
                    {
                        Layers.Data.Sqls.PvData.Feroboc feroboc = null; //上午10后的第1个外汇牌价

                        #region 查询币种的发布记录

                        string searchDate = DateTime.Today.ToString("yyyy-MM-dd"); //查询时间
                        string encodeCurrency = HttpUtility.UrlEncode(c.GetDescription()); //对币种做url编码
                        int page = 1;
                        string searchHtml = string.Empty; //查询界面

                        while (true)
                        {
                            bool isFound = false; //是否已经找到上午10点前的最后一次发布

                            string postStr = $"erectDate={searchDate}&nothing={searchDate}&pjname={encodeCurrency}&page={page}"; //参数
                            byte[] postData = Encoding.UTF8.GetBytes(postStr); //编码
                            wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded"); //提交请求
                            byte[] responseData = wc.UploadData(searchUrl, "POST", postData); //得到返回字符流  
                            searchHtml = Encoding.UTF8.GetString(responseData); //解码

                            var rows = regex.Matches(searchHtml);
                            foreach (var row in rows)
                            {
                                var searchGroups = regex.Match(row.ToString()).Groups;
                                if (DateTime.Parse(searchGroups[6].Value) >= todayTenAm)
                                {
                                    if (feroboc == null)
                                        feroboc = new Layers.Data.Sqls.PvData.Feroboc();
                                    feroboc.Type = tenAmChineseBank;
                                    feroboc.Currency = (int)c;
                                    feroboc.Xhmr = decimal.Parse(searchGroups[1].Value);
                                    feroboc.Xcmr = decimal.Parse(searchGroups[2].Value);
                                    feroboc.Xhmc = decimal.Parse(searchGroups[3].Value);
                                    feroboc.Xcmc = decimal.Parse(searchGroups[4].Value);
                                    feroboc.Zhzsj = decimal.Parse(searchGroups[5].Value);
                                    feroboc.PublishDate = DateTime.Parse(searchGroups[6].Value);
                                    feroboc.CreateDate = DateTime.Now;
                                }
                                else
                                {
                                    isFound = true;
                                    break;
                                }
                            }

                            if (isFound) break;
                            else page++;
                        }

                        #endregion

                        #region 特殊处理

                        if (feroboc == null)
                        {
                            //周末、节假日发布特别少的情况，如果到了10点10分查询界面还查询不到10点后的发布，则以外汇牌价主页的为准
                            DateTime todayTenAmTenMin = DateTime.Today.AddHours(10).AddMinutes(10);
                            if (DateTime.Now >= todayTenAmTenMin)
                            {
                                feroboc = new Layers.Data.Sqls.PvData.Feroboc()
                                {
                                    Type = tenAmChineseBank,
                                    Currency = (int)c,
                                    Xhmr = decimal.Parse(groups[1].Value),
                                    Xcmr = decimal.Parse(groups[2].Value),
                                    Xhmc = decimal.Parse(groups[3].Value),
                                    Xcmc = decimal.Parse(groups[4].Value),
                                    Zhzsj = decimal.Parse(groups[5].Value),
                                    PublishDate = publishDate,
                                    CreateDate = DateTime.Now
                                };
                            }
                            else
                            {
                                continue;
                            }
                        }

                        #endregion

                        #region 数据持久化
                        //汇率
                        ExchangeRateEnter(feroboc, reponsitory);
                        //外汇牌价
                        FerobocEnter(feroboc, reponsitory);
                        //外汇牌价页面记录
                        FerobocLogEnter(feroboc, indexHtml, searchHtml, reponsitory);
                        // 2022-07-20 LK 汇率不需要同步到创新恒远
                        //汇率同步 -> 创新恒远 
                        //ExchangeRateSync<ScCustomReponsitory>(feroboc);
                        //汇率同步 -> 芯达通
                        ExchangeRateSync<foricScCustomsReponsitory>(feroboc);
                        #endregion

                        //更新币种的上午10点汇率的发布时间
                        tenAmDic[c] = feroboc.PublishDate;
                    }
                }
            }
        }

        /// <summary>
        /// 上午9点半汇率抓取
        /// </summary>
        /// <param name="indexHtml">中国银行外汇牌价主页</param>
        void NineAmRateCrawl(string indexHtml)
        {
            using (var reponsitory = new PvDataReponsitory())
            using (var wc = new WebClient())
            {
                string searchUrl = "https://srh.bankofchina.com/search/whpj/search_cn.jsp"; //请求地址
                RegexOptions regexOptions = RegexOptions.Singleline | RegexOptions.IgnoreCase;
                string tenAmChineseBank = ExchangeType.NineAmChineseBank.ToString(); //中国银行上午10点后的第一个实时汇率

                foreach (Currency c in Enum.GetValues(typeof(Currency)))
                {
                    if (c == Currency.Unknown || c == Currency.CNY)
                        continue;

                    //正则匹配
                    Regex regex = new Regex(@"<td>" + c.GetDescription() + ".*?</td>.*?<td>(.*?)</td>.*?<td>(.*?)</td>.*?<td>(.*?)</td>.*?<td>(.*?)</td>.*?<td>(.*?)</td>.*?<td.*?>(.*?)</td>", regexOptions);
                    var groups = regex.Match(indexHtml).Groups;
                    if (groups.Count == 1)
                        continue;

                    DateTime todayNineThirtyAm = DateTime.Today.AddHours(9).AddMinutes(30); //当天上午10点
                    if (tenAmDic[c] >= todayNineThirtyAm)
                        continue;

                    DateTime publishDate;
                    DateTime.TryParse(groups[6].Value, out publishDate); //发布时间

                    //如果发布时间已经超过上午10点，开始做汇率抓取
                    if (publishDate >= todayNineThirtyAm)
                    {
                        Layers.Data.Sqls.PvData.Feroboc feroboc = null; //上午10后的第1个外汇牌价

                        #region 查询币种的发布记录

                        string searchDate = DateTime.Today.ToString("yyyy-MM-dd"); //查询时间
                        string encodeCurrency = HttpUtility.UrlEncode(c.GetDescription()); //对币种做url编码
                        int page = 1;
                        string searchHtml = string.Empty; //查询界面

                        while (true)
                        {
                            bool isFound = false; //是否已经找到上午10点前的最后一次发布

                            string postStr = $"erectDate={searchDate}&nothing={searchDate}&pjname={encodeCurrency}&page={page}"; //参数
                            byte[] postData = Encoding.UTF8.GetBytes(postStr); //编码
                            wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded"); //提交请求
                            byte[] responseData = wc.UploadData(searchUrl, "POST", postData); //得到返回字符流  
                            searchHtml = Encoding.UTF8.GetString(responseData); //解码

                            var rows = regex.Matches(searchHtml);
                            foreach (var row in rows)
                            {
                                var searchGroups = regex.Match(row.ToString()).Groups;
                                if (DateTime.Parse(searchGroups[6].Value) >= todayNineThirtyAm)
                                {
                                    if (feroboc == null)
                                        feroboc = new Layers.Data.Sqls.PvData.Feroboc();
                                    feroboc.Type = tenAmChineseBank;
                                    feroboc.Currency = (int)c;
                                    feroboc.Xhmr = decimal.Parse(searchGroups[1].Value);
                                    feroboc.Xcmr = decimal.Parse(searchGroups[2].Value);
                                    feroboc.Xhmc = decimal.Parse(searchGroups[3].Value);
                                    feroboc.Xcmc = decimal.Parse(searchGroups[4].Value);
                                    feroboc.Zhzsj = decimal.Parse(searchGroups[5].Value);
                                    feroboc.PublishDate = DateTime.Parse(searchGroups[6].Value);
                                    feroboc.CreateDate = DateTime.Now;
                                }
                                else
                                {
                                    isFound = true;
                                    break;
                                }
                            }

                            if (isFound) break;
                            else page++;
                        }

                        #endregion

                        #region 特殊处理

                        if (feroboc == null)
                        {
                            //周末、节假日发布特别少的情况，如果到了10点10分查询界面还查询不到10点后的发布，则以外汇牌价主页的为准
                            DateTime todayTenAmTenMin = DateTime.Today.AddHours(10).AddMinutes(10);
                            if (DateTime.Now >= todayTenAmTenMin)
                            {
                                feroboc = new Layers.Data.Sqls.PvData.Feroboc()
                                {
                                    Type = tenAmChineseBank,
                                    Currency = (int)c,
                                    Xhmr = decimal.Parse(groups[1].Value),
                                    Xcmr = decimal.Parse(groups[2].Value),
                                    Xhmc = decimal.Parse(groups[3].Value),
                                    Xcmc = decimal.Parse(groups[4].Value),
                                    Zhzsj = decimal.Parse(groups[5].Value),
                                    PublishDate = publishDate,
                                    CreateDate = DateTime.Now
                                };
                            }
                            else
                            {
                                continue;
                            }
                        }

                        #endregion

                        #region 数据持久化                       
                        //汇率同步 -> 芯达通
                        NineExchangeRateSync<foricScCustomsReponsitory>(feroboc);
                        #endregion

                        //更新币种的上午10点汇率的发布时间
                        tenAmDic[c] = feroboc.PublishDate;
                    }
                }
            }
        }

        /// <summary>
        /// 抓取的中国银行外汇牌价页面记录
        /// </summary>
        /// <param name="type">汇率类型</param>
        /// <param name="c">币种</param>
        /// <param name="indexHtml">外汇牌价主页</param>
        /// <param name="searchHtml">外汇牌价查询界面</param>
        /// <param name="reponsitory"></param>
        private void FerobocLogEnter(Layers.Data.Sqls.PvData.Feroboc feroboc, string indexHtml, string searchHtml, PvDataReponsitory reponsitory)
        {
            reponsitory.Insert(new Layers.Data.Sqls.PvData.Logs_Feroboc
            {
                ID = Layers.Data.PKeySigner.Pick(PKeyType.FerobocLog),
                Type = feroboc.Type,
                Currency = feroboc.Currency,
                IndexHtml = indexHtml,
                SearchHtml = searchHtml,
                CreateDate = DateTime.Now
            });
        }

        /// <summary>
        /// 汇率持久化
        /// </summary>
        /// <param name="feroboc"></param>
        /// <param name="reponsitory"></param>
        private void ExchangeRateEnter(Layers.Data.Sqls.PvData.Feroboc feroboc, PvDataReponsitory reponsitory)
        {
            var exchangeRate = reponsitory.ReadTable<Layers.Data.Sqls.PvData.ExchangeRates>()
                                .FirstOrDefault(item => item.Type == feroboc.Type && item.District == (int)District.CN && item.From == feroboc.Currency && item.To == (int)Currency.CNY);
            if (exchangeRate == null)
            {
                //如果数据库中没有相关币种汇率，直接插入
                reponsitory.Insert(new Layers.Data.Sqls.PvData.ExchangeRates
                {
                    Type = feroboc.Type,
                    District = (int)District.CN,
                    From = feroboc.Currency,
                    To = (int)Currency.CNY,
                    Value = feroboc.Xhmc.Value / 100,
                    ModifyDate = feroboc.PublishDate
                });
            }
            else
            {
                var date = exchangeRate.ModifyDate?.Date;
                //如果今天还没有更新过，则更新
                if (date != DateTime.Today)
                {
                    reponsitory.Update<Layers.Data.Sqls.PvData.ExchangeRates>(new
                    {
                        Value = feroboc.Xhmc.Value / 100,
                        ModifyDate = feroboc.PublishDate
                    }, item => item.Type == feroboc.Type
                        && item.District == (int)District.CN && item.From == feroboc.Currency && item.To == (int)Currency.CNY);
                }
            }
        }

        /// <summary>
        /// 中国银行外汇牌价持久化
        /// </summary>
        /// <param name="feroboc"></param>
        /// <param name="reponsitory"></param>
        private void FerobocEnter(Layers.Data.Sqls.PvData.Feroboc feroboc, PvDataReponsitory reponsitory)
        {
            var tenAmFeroboc = reponsitory.ReadTable<Layers.Data.Sqls.PvData.Feroboc>()
                                .FirstOrDefault(item => item.Type == feroboc.Type && item.Currency == feroboc.Currency);
            if (tenAmFeroboc == null)
            {
                //如果数据库中没有相关币种汇率，直接插入
                reponsitory.Insert(new Layers.Data.Sqls.PvData.Feroboc
                {
                    ID = Layers.Data.PKeySigner.Pick(PKeyType.Feroboc),
                    Type = feroboc.Type,
                    Currency = feroboc.Currency,
                    Xhmr = feroboc.Xhmr,
                    Xcmr = feroboc.Xcmr,
                    Xhmc = feroboc.Xhmc,
                    Xcmc = feroboc.Xcmc,
                    Zhzsj = feroboc.Zhzsj,
                    PublishDate = feroboc.PublishDate,
                    CreateDate = feroboc.CreateDate
                });
            }
            else
            {
                var date = tenAmFeroboc.PublishDate.Date;
                //如果今天还没有更新过，则更新
                if (date != DateTime.Today)
                {
                    reponsitory.Update<Layers.Data.Sqls.PvData.Feroboc>(new
                    {
                        Xhmr = feroboc.Xhmr,
                        Xcmr = feroboc.Xcmr,
                        Xhmc = feroboc.Xhmc,
                        Xcmc = feroboc.Xcmc,
                        Zhzsj = feroboc.Zhzsj,
                        PublishDate = feroboc.PublishDate,
                        CreateDate = feroboc.CreateDate
                    }, item => item.Type == feroboc.Type && item.Currency == feroboc.Currency);
                }
            }
        }

        /// <summary>
        /// 同步汇率，并记录日志
        /// </summary>
        /// <typeparam name="TReponsitory">数据库连接</typeparam>
        /// <param name="feroboc">中国银行外汇牌价</param>
        /// <param name="pKeyType"></param>
        private void ExchangeRateSync<TReponsitory>(Layers.Data.Sqls.PvData.Feroboc feroboc)
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
        {
            using (var reponsitory = new TReponsitory())
            {
                PKeyType pKeyType;
                if (typeof(TReponsitory) == typeof(ScCustomReponsitory))
                {
                    pKeyType = PKeyType.ExchangeRateLog_HY;
                }
                else
                {
                    pKeyType = PKeyType.ExchangeRateLog_XDT;
                }

                string code = ((Currency)feroboc.Currency).ToString();
                //查询当前币种的实时汇率
                var tenAmRate = reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.ExchangeRates>()
                                .FirstOrDefault(item => item.Code == code && item.Type == (int)ExchangeRateType.RealTime);
                if (tenAmRate == null)
                {
                    //如果数据库中没有相关币种汇率，直接插入
                    string id = string.Concat(code, ExchangeRateType.RealTime.GetHashCode()).MD5();
                    reponsitory.Insert(new Layers.Data.Sqls.ScCustoms.ExchangeRates
                    {
                        ID = id,
                        Code = code,
                        Type = (int)ExchangeRateType.RealTime,
                        Rate = feroboc.Xhmc.Value / 100,
                        CreateDate = feroboc.PublishDate,
                        UpdateDate = feroboc.PublishDate,
                        Summary = string.Empty
                    });
                    //记录日志
                    reponsitory.Insert(new Layers.Data.Sqls.ScCustoms.ExchangeRateLogs
                    {
                        ID = Layers.Data.PKeySigner.Pick(pKeyType),
                        ExchangeRateID = id,
                        Rate = feroboc.Xhmc.Value / 100,
                        CreateDate = DateTime.Now,
                        Summary = "管理员[Npc系统]新增了实时汇率"
                    });
                }
                else
                {
                    var date = tenAmRate.UpdateDate?.Date;
                    if (date != DateTime.Today)
                    {
                        //更新币种汇率
                        reponsitory.Update<Layers.Data.Sqls.ScCustoms.ExchangeRates>(new
                        {
                            Rate = feroboc.Xhmc.Value / 100,
                            UpdateDate = feroboc.PublishDate
                        }, item => item.Code == code && item.Type == (int)ExchangeRateType.RealTime);

                        //记录日志
                        reponsitory.Insert(new Layers.Data.Sqls.ScCustoms.ExchangeRateLogs
                        {
                            ID = Layers.Data.PKeySigner.Pick(pKeyType),
                            ExchangeRateID = tenAmRate.ID,
                            Rate = tenAmRate.Rate,
                            CreateDate = DateTime.Now,
                            Summary = "管理员[Npc系统]修改了实时汇率"
                        });
                    }
                }
            }
        }

        private void NineExchangeRateSync<TReponsitory>(Layers.Data.Sqls.PvData.Feroboc feroboc)
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
        {
            using (var reponsitory = new TReponsitory())
            {
                PKeyType pKeyType;
                if (typeof(TReponsitory) == typeof(ScCustomReponsitory))
                {
                    pKeyType = PKeyType.ExchangeRateLog_HY;
                }
                else
                {
                    pKeyType = PKeyType.ExchangeRateLog_XDT;
                }

                string code = ((Currency)feroboc.Currency).ToString();
                //查询当前币种的实时汇率
                var tenAmRate = reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.ExchangeRates>()
                                .FirstOrDefault(item => item.Code == code && item.Type == (int)ExchangeRateType.NineRealTime);
                if (tenAmRate == null)
                {
                    //如果数据库中没有相关币种汇率，直接插入
                    string id = string.Concat(code, ExchangeRateType.NineRealTime.GetHashCode()).MD5();
                    reponsitory.Insert(new Layers.Data.Sqls.ScCustoms.ExchangeRates
                    {
                        ID = id,
                        Code = code,
                        Type = (int)ExchangeRateType.NineRealTime,
                        Rate = feroboc.Xhmc.Value / 100,
                        CreateDate = feroboc.PublishDate,
                        UpdateDate = feroboc.PublishDate,
                        Summary = string.Empty
                    });
                    //记录日志
                    reponsitory.Insert(new Layers.Data.Sqls.ScCustoms.ExchangeRateLogs
                    {
                        ID = Layers.Data.PKeySigner.Pick(pKeyType),
                        ExchangeRateID = id,
                        Rate = feroboc.Xhmc.Value / 100,
                        CreateDate = DateTime.Now,
                        Summary = "管理员[Npc系统]新增了实时汇率"
                    });
                }
                else
                {
                    var date = tenAmRate.UpdateDate?.Date;
                    if (date != DateTime.Today)
                    {
                        //更新币种汇率
                        reponsitory.Update<Layers.Data.Sqls.ScCustoms.ExchangeRates>(new
                        {
                            Rate = feroboc.Xhmc.Value / 100,
                            UpdateDate = feroboc.PublishDate
                        }, item => item.Code == code && item.Type == (int)ExchangeRateType.NineRealTime);

                        //记录日志
                        reponsitory.Insert(new Layers.Data.Sqls.ScCustoms.ExchangeRateLogs
                        {
                            ID = Layers.Data.PKeySigner.Pick(pKeyType),
                            ExchangeRateID = tenAmRate.ID,
                            Rate = tenAmRate.Rate,
                            CreateDate = DateTime.Now,
                            Summary = "管理员[Npc系统]修改了实时汇率"
                        });
                    }
                }
            }
        }
    }
}
