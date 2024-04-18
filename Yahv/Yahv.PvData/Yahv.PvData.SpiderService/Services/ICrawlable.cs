using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yahv.PvData.SpiderService.Handlers;

namespace Yahv.PvData.SpiderService.Services
{
    public interface ICrawlable
    {
        string Url { get; }

        event CrawledHandler Crawled;

        /// <summary>
        /// 爬取
        /// </summary>
        void Crawling();
    }

    public abstract class Crawl : ICrawlable
    {
        public abstract string Url { get; }

        public event CrawledHandler Crawled;

        public virtual void Crawling()
        {
            string html = null;

            using (var wc = new WebClient())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                wc.Encoding = Encoding.UTF8;
                html = wc.DownloadString(this.Url);
            }

            this.Crawled?.Invoke(this, new Handlers.CrawledEventArgs(html));
        }
    }
}
